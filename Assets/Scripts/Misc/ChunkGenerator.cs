using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] Tilemap baseTilemap;

    [SerializeField] GameObject baseCamBounds;

    [SerializeField] List<GameObject> entryChunks;
    [SerializeField] List<GameObject> rightChunks;
    [SerializeField] List<GameObject> downChunks;
    [SerializeField] List<GameObject> upChunks;
    [SerializeField] List<GameObject> exitChunk;

    private List<GameObject> chunksToSpawn;

    [SerializeField] GameObject playerPrefab;

    [SerializeField] CinemachineConfiner2D camConfiner;

    [SerializeField] Transform generatorParent;

    [SerializeField] bool debugGeneration;
   
    Vector3 nextChunkPos = Vector3.zero;
    Vector3 spawnPos = Vector3.zero;

    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        //get randomized chunks to spawn
        int chunkSize = Random.Range(5, 9); // no. of chunks to be spawned 5-8 , excluding entry & exit chunk

        Debug.Log($"chunkSize - {chunkSize}");

        chunksToSpawn = new List<GameObject>();
        //entry chunk
        chunksToSpawn.Add(RandomItemFromListSize(entryChunks));

        for (int i = 0; i < chunkSize; i++)
        {
            var chunkDirection = Random.Range(0, 3); // 0,1,2 for chunk directions

            switch (chunkDirection)
            {
                case 0:
                    chunksToSpawn.Add(RandomItemFromListSize(rightChunks));
                    break;
                case 1:
                    chunksToSpawn.Add(RandomItemFromListSize(upChunks));
                    break;
                case 2:
                    chunksToSpawn.Add(RandomItemFromListSize(downChunks));
                    break;
            }
        }

        //exit chunk
        chunksToSpawn.Add(RandomItemFromListSize(exitChunk));

        //execute merge on those
        StartCoroutine(SpawnChunks());
    }
    [ContextMenu("Regenerate")]
    public void Regenerate()
    {
        StartCoroutine(ClearAndGenerate());
    }

    public IEnumerator ClearAndGenerate()
    {
        playerPrefab.SetActive(false);

        Debug.Log($"child count : {generatorParent.childCount}");
        //clear
        foreach(Transform child in generatorParent)
        {
            if (debugGeneration)
                Debug.Break();
            Destroy(child.gameObject);
        }

        baseTilemap.ClearAllTiles();
        chunksToSpawn.Clear();
        var colliders = baseCamBounds.GetComponents<BoxCollider2D>();
        foreach (var c in colliders)
        {
            Destroy(c); // Removes it at the end of the frame
        }

        nextChunkPos = Vector3.zero;
        spawnPos = Vector3.zero;

        //wait 1 frame for the chunks and colliders to be destroyed
        yield return null;

        //generate again
        Generate();
    }
    IEnumerator SpawnChunks()
    {
        foreach (var chunk in chunksToSpawn)
        {
            //spawn tilemap Prefabs
            var chunkObj = Instantiate(chunk,spawnPos,Quaternion.identity,generatorParent);

            if(debugGeneration)
                Debug.Break();

            yield return null; // wait 1 frame for the prefabs to spawn

            //get tilemap references from handler scripts
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

    public GameObject RandomItemFromListSize(List<GameObject> listItems)
    {
        return listItems[Random.Range(0, listItems.Count)];
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
