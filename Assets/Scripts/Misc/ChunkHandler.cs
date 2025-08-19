using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkHandler : MonoBehaviour
{
    public Tilemap tilemap;
    public Vector3 chunkShift;
    public BoxCollider2D camBounds;
    public void ClearChunkLeftOver()
    {
        //remove grid and tilemap gameobject all together
        Destroy(tilemap.transform.parent.gameObject);

        //remove chunk's camBound duplicate
        Destroy(camBounds.gameObject);
    }
}
