using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapMerger : MonoBehaviour
{
    [SerializeField] Tilemap baseTilemap;

    [SerializeField] GameObject baseCamBounds;
 
    [SerializeField] GameObject[] chunks;

    [SerializeField] GameObject playerPrefab;

    [SerializeField] CinemachineConfiner2D camConfiner;

    [SerializeField] bool debugGeneration;
   
    Vector3 nextChunkPos = Vector3.zero;
    Vector3 spawnPos = Vector3.zero;
    private void Start()
    {
        //spawn tilemap Prefabs
        //get tilemap references from handler scripts

        //execute merge on those
        StartCoroutine(SpawnChunks());
    }
    IEnumerator SpawnChunks()
    {
        foreach (var chunk in chunks)
        {
            var chunkObj = Instantiate(chunk,spawnPos,Quaternion.identity);

            if(debugGeneration)
                Debug.Break();

            yield return null; // wait 1 frame for the prefabs to spawn

            var chunkHandler = chunkObj.GetComponent<ChunkHandler>();

            Merge(chunkHandler.tilemap);

            if (debugGeneration)
                Debug.Break();

            //merge cameraBounds
            MergeCamBounds(chunkHandler);

            if (debugGeneration)
                Debug.Break();

            //remove the chunk's leftover after merge
            chunkHandler.ClearChunkLeftOver();

            nextChunkPos = chunkHandler.GetChunkShift();
            spawnPos += nextChunkPos;
        }

        yield return null;

        //update the cinemachine camera bounds
        camConfiner.enabled = true;
        camConfiner.InvalidateCache();

        //spawn player
        var playerSpawnPos = FindAnyObjectByType<ChunkEntryPoint>().transform.position; 
        playerPrefab.transform.position = playerSpawnPos;
        playerPrefab.SetActive(true);
    }
    public void Merge(Tilemap targetTilemap)
    {
        //merge target Tilemap into Base Tilemap
        BoundsInt bounds = targetTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = targetTilemap.GetTile(pos);
            if (tile != null)
            {
                // Convert targetTilemap cell -> world -> baseTilemap cell
                Vector3 worldPos = targetTilemap.CellToWorld(pos);
                Vector3Int basePos = baseTilemap.WorldToCell(worldPos);

                // Copy the tile into baseTilemap at the correct shifted position
                baseTilemap.SetTile(basePos, tile);

                // Copy flags
                TileFlags flags = targetTilemap.GetTileFlags(pos);
                baseTilemap.SetTileFlags(basePos, flags);

                // Copy transform matrix
                Matrix4x4 matrix = targetTilemap.GetTransformMatrix(pos);
                baseTilemap.SetTransformMatrix(basePos, matrix);
            }
        }

    }

    private void MergeCamBounds(ChunkHandler chunkHandler)
    {
        // Create a new collider on the baseCamBounds
        BoxCollider2D newCollider = baseCamBounds.AddComponent<BoxCollider2D>();

        // Copy size and offset from the chunk’s collider
        newCollider.size = chunkHandler.camBounds.size;
        newCollider.offset = chunkHandler.camBounds.offset + (Vector2)spawnPos;

        //copy other settings like isTrigger, usedByComposite, etc.
        newCollider.isTrigger = chunkHandler.camBounds.isTrigger;
        newCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
    }
}
