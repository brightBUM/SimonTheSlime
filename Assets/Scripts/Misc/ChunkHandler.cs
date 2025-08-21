
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ChunkDirection
{
    RIGHT,
    UP,
    DOWN
}

public class ChunkHandler : MonoBehaviour
{
    public Tilemap tilemap;
    public BoxCollider2D camBounds;
    public ChunkDirection chunkDirection;
    public bool debugChunkSize;

    private Vector3 chunkShift = new Vector3(72,36);
    public void ClearChunkLeftOver()
    {
        //remove grid and tilemap gameobject all together
        Destroy(tilemap.transform.parent.gameObject);

        //remove chunk's camBound duplicate
        Destroy(camBounds.gameObject);
    }

    public Vector3 GetChunkShift()
    {
        switch (chunkDirection)
        {
            case ChunkDirection.RIGHT:
                return chunkShift.x*Vector3.right;
            case ChunkDirection.UP:
                return chunkShift.x * Vector3.right + chunkShift.y*Vector3.up;
            case ChunkDirection.DOWN:
                return chunkShift.x * Vector3.right + chunkShift.y * Vector3.down;
            default:
                return Vector3.zero;
        }
    }


    #region HelperGizmos
    private void OnDrawGizmos()
    {
        if(debugChunkSize)
        {
            Gizmos.color = Color.yellow;
            switch (chunkDirection)
            {
                case ChunkDirection.RIGHT:

                    var rightCentre = transform.position;
                    Gizmos.DrawWireCube(rightCentre, chunkShift);
                    DrawLine(rightCentre, 4);
                    DrawLine(rightCentre, 10);
                    break;
                case ChunkDirection.UP:
                    
                    var upCentre = Vector3.up * chunkShift.y / 2;
                    Gizmos.DrawWireCube(transform.position + upCentre, chunkShift+Vector3.up*chunkShift.y);
                    DrawLine(transform.position + 2*upCentre, 4);
                    DrawLine(transform.position + 2*upCentre, 10);
                    break;
                case ChunkDirection.DOWN:

                    var downCentre = Vector3.up * chunkShift.y / 2;
                    Gizmos.DrawWireCube(transform.position - downCentre, chunkShift + Vector3.up * chunkShift.y);
                    DrawLine(transform.position - 2*downCentre, 4);
                    DrawLine(transform.position - 2*downCentre, 10);
                    break;
            }

        }
        
    }

    private void DrawLine(Vector3 startPos,float tileSpace)
    {
        Gizmos.color = Color.white;

        //from
        var from = new Vector3(startPos.x - chunkShift.x/2, startPos.y - tileSpace, 0);

        //To
        var To = new Vector3(startPos.x + chunkShift.x/2, startPos.y - tileSpace, 0);

        Gizmos.DrawLine(from, To);
    }
    #endregion
}
