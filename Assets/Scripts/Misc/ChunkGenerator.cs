using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

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

    private List<Transform> cagePodPositions;

    [SerializeField] GameObject playerPrefab;

    [SerializeField] CinemachineConfiner2D camConfiner;

    [SerializeField] Transform generatorParent;

    [SerializeField] WeightedRNG weightedRNG;

    [SerializeField] Transform[] parallaxCeiling;
    [SerializeField] Transform[] parallaxFloor;

    [SerializeField] bool debugGeneration;
   
    Vector3 nextChunkPos = Vector3.zero;
    Vector3 spawnPos = Vector3.zero;

    int highestChunkIndex = 0;
    int lowestChunkIndex = 0;
    float highestY = float.MinValue;
    float lowestY = float.MaxValue;
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
        cagePodPositions = new List<Transform>();
        //entry chunk
        chunksToSpawn.Add(Utility.RandomItemFromList(entryChunks));

        // make working copies of the lists so inspector lists remain intact
        List<GameObject> availableRight = new List<GameObject>(rightChunks);
        List<GameObject> availableUp = new List<GameObject>(upChunks);
        List<GameObject> availableDown = new List<GameObject>(downChunks);

        for (int i = 0; i < chunkSize; i++)
        {
            // collect directions that still have something left
            List<int> validDirections = new List<int>();
            if (availableRight.Count > 0) validDirections.Add(0);
            if (availableUp.Count > 0) validDirections.Add(1);
            if (availableDown.Count > 0) validDirections.Add(2);

            if (validDirections.Count == 0)
            {
                Debug.LogWarning("No chunks left to spawn!");
                break;
            }

            // pick a random valid direction
            int chunkDirection = validDirections[Random.Range(0, validDirections.Count)];

            switch (chunkDirection)
            {
                case 0:
                    chunksToSpawn.Add(Utility.RandomUniqueItemFromList(availableRight));
                    break;
                case 1:
                    chunksToSpawn.Add(Utility.RandomUniqueItemFromList(availableUp));
                    break;
                case 2:
                    chunksToSpawn.Add(Utility.RandomUniqueItemFromList(availableDown));
                    break;
            }
        }

        //exit chunk
        chunksToSpawn.Add(Utility.RandomItemFromList(exitChunk));

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

        //clear
        foreach(Transform child in generatorParent)
        {
            Destroy(child.gameObject);
        }

        baseTilemap.ClearAllTiles();
        chunksToSpawn.Clear();
        cagePodPositions.Clear();
        weightedRNG.ClearItems();
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
        //set Background
        TileAssetHandler.Instance.SetBackground();

        foreach (var (chunk, index) in chunksToSpawn.Select((c, i) => (c, i)))
        {
            //spawn tilemap Prefabs
            var chunkObj = Instantiate(chunk,spawnPos,Quaternion.identity,generatorParent);

            if(debugGeneration)
                Debug.Break();

            yield return null; // wait 1 frame for the prefabs to spawn

            //get tilemap references from handler scripts
            var chunkHandler = chunkObj.GetComponent<ChunkHandler>();

            //change chunk's tile theme after spawn
            chunkHandler.Init();

            Merge(chunkHandler.tilemap);

            // Track highest and lowest chunk Y
            float chunkY = spawnPos.y;
            if (chunkY > highestY) { highestY = chunkY; highestChunkIndex = index; }
            if (chunkY < lowestY) { lowestY = chunkY; lowestChunkIndex = index; }

            if (debugGeneration)
                Debug.Break();

            //get cagePod Position
            cagePodPositions.Add(chunkHandler.podSpawnPoint);

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

        //spawn CagePods b/w chunks
        weightedRNG.SpawnPods(cagePodPositions, chunksToSpawn.Count - 2); // -2 exclude entry & exit chunks

        //spawn player
        var playerSpawnPos = FindAnyObjectByType<ChunkEntryPoint>().transform.position;
        playerPrefab.transform.position = playerSpawnPos;
        playerPrefab.SetActive(true);

        //set transforms at height and lowest for FG parallax
        foreach (var ceilingTransform in parallaxCeiling)
        {
            ceilingTransform.position = new Vector3(0, highestY, 0);
        }
        foreach (var floorTransform in parallaxFloor)
        {
            floorTransform.position = new Vector3(0, lowestY, 0);
        }
        

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
