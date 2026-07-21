
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class PipePlatform : MonoBehaviour, IPoundable
{
    [Header("Tile Setup")]
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] GameObject breakableTileChunk;
    [SerializeField] SpriteRenderer[] spriteRenderers;
    [SerializeField] Transform snapPoint;
    [SerializeField] Transform targetPos;
    [SerializeField] BoxCollider2D poundCollider;
    [SerializeField] BoxCollider2D triggerCollider;
    [SerializeField] GameObject lightSprite;
    [SerializeField] GameObject arrowSprite;
    [SerializeField] GameObject inventoryFullText;
    [Header("Break Settings")]
    [SerializeField] float explosionForce = 6f;
    [SerializeField] float upwardBias = 1.5f;
    [SerializeField] float spinForce = 250f;
    [SerializeField] float breakRowDelay = 0.06f;
    [Header("Rise Settings")]
    [SerializeField] float snapDuration = 0.2f;   // time to snap to pipe X
    [SerializeField] float pullDuration = 0.8f;   // time to rise to targetPos
    [SerializeField] float riseRowDelay = 0.5f;
    [SerializeField] int width = 3;
    [SerializeField] int maxDepth = 5;

    // ✅ Store BOTH tile + sprite (fixes rebuild issue)
    Dictionary<Vector3Int, (TileBase tile, Sprite sprite)> cachedTiles
        = new Dictionary<Vector3Int, (TileBase, Sprite)>();

    PlayerController playerController;
    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SnapToGrid();
        }
#endif
    }
    private void Start()
    {
        //prevent entry to dungeon , if no vacant inventory slot
        if(!SaveLoadManager.Instance.IsInventorySlotAvailable())
        {
            lightSprite.SetActive(false);
            arrowSprite.SetActive(false);
            inventoryFullText.SetActive(true);
            this.enabled = false;
            Debug.Log("No inventory slot available - disable pipe platform");
        }
        else
        {
            Debug.Log("inv slot available - pipe platform");

        }
        
    }
    public void OnPlayerPounded(Action<IPoundable> ContinuePound)
    {
        if (!this.enabled)
            return;

        if (TryGetComponent<Collider2D>(out var col))
            col.enabled = false;

        ContinuePound(this);

        foreach (var sr in spriteRenderers)
            sr.sortingOrder = 10;

        
        LevelManager.Instance.startLevelTimer = false; //pause level timer when entering dungeon
        StartCoroutine(BreakTilesRowByRow(transform.position));
        //StartCoroutine(PlayTileBreakAudio());
    }
    IEnumerator PlayTileBreakAudio()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator BreakTilesRowByRow(Vector3 impactWorldPos)
    {
       

        //Debug.Break();
        Vector3Int baseCell = groundTilemap.WorldToCell(
            impactWorldPos + Vector3.down * groundTilemap.cellSize.y
        );

        int y = 0;

        while (y < maxDepth)
        {
            bool anyTileInRow = false;

            for (int x = -width / 2; x <= width / 2; x++)
            {
                Vector3Int pos = new Vector3Int(baseCell.x + x, baseCell.y - y, 0);

                TileBase tile = groundTilemap.GetTile(pos);
                if (tile == null) continue;

                anyTileInRow = true;

                //  Cache tile + sprite
                if (!cachedTiles.ContainsKey(pos))
                {
                    Sprite sprite = groundTilemap.GetSprite(pos);
                    cachedTiles[pos] = (tile, sprite);
                }

                // Remove tile
                groundTilemap.SetTile(pos, null);

                // Spawn chunk
                Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);
                GameObject chunk = Instantiate(breakableTileChunk, worldPos, Quaternion.identity);

                var data = cachedTiles[pos];
                if (data.sprite != null)
                    chunk.GetComponent<SpriteRenderer>().sprite = data.sprite;

                Rigidbody2D rb = chunk.GetComponent<Rigidbody2D>();

                // Direction + spin
                Vector2 dir;

                if (x < 0)
                {
                    dir = new Vector2(-1f, 1f);
                    rb.AddTorque(spinForce);
                }
                else if (x > 0)
                {
                    dir = new Vector2(1f, 1f);
                    rb.AddTorque(-spinForce);
                }
                else
                {
                    dir = new Vector2(0f, 1.2f);
                    rb.AddTorque(UnityEngine.Random.Range(-spinForce, spinForce));
                }

                dir += new Vector2(
                    UnityEngine.Random.Range(-0.2f, 0.2f),
                    UnityEngine.Random.Range(0f, 0.3f)
                );

                rb.AddForce(dir.normalized * explosionForce, ForceMode2D.Impulse);

                Destroy(chunk, 2f);

                yield return new WaitForSeconds(0.01f);

            }

            if (!anyTileInRow)
                break;

            y++;
            SoundManager.Instance.PlayPipeTileBreakSFx();

            yield return new WaitForSeconds(breakRowDelay);
        }

        groundTilemap.GetComponent<TilemapCollider2D>()?.ProcessTilemapChanges();
    }

    [ContextMenu("TriggerRebuild")]
    public void TriggerRebuild()
    {
        if(playerController!=null)
            StartCoroutine(RebuildTiles(playerController));
    }
    public void RepositionPlayer()
    {
        playerController.transform.position = snapPoint.position;
    }
    IEnumerator RebuildTiles(PlayerController player)
    {
        foreach (var sr in spriteRenderers)
            sr.sortingOrder = -1;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        // Sort bottom → top
        List<Vector3Int> cells = new List<Vector3Int>(cachedTiles.Keys);
        cells.Sort((a, b) => a.y.CompareTo(b.y));

        // Group rows
        Dictionary<int, List<Vector3Int>> rows = new Dictionary<int, List<Vector3Int>>();

        int lowestY = int.MaxValue;

        foreach (var cell in cells)
        {
            if (!rows.ContainsKey(cell.y))
                rows[cell.y] = new List<Vector3Int>();

            rows[cell.y].Add(cell);

            if (cell.y < lowestY)
                lowestY = cell.y;
        }

        // Snap player to bottom row start
        //Vector3 playerStart = groundTilemap.GetCellCenterWorld(
        //    new Vector3Int(groundTilemap.WorldToCell(player.transform.position).x, lowestY, 0)
        //);
        Vector3 playerStart = snapPoint.position;

        player.transform.position = playerStart;

        // Process rows
        foreach (var row in rows)
        {
            //Debug.Break();
            // Spawn tiles
            foreach (var cellPos in row.Value)
            {
                var data = cachedTiles[cellPos];

                Vector3 targetPos = groundTilemap.GetCellCenterWorld(cellPos);
                Vector3 spawnPos = targetPos + Vector3.down * 20f+ Vector3.right * UnityEngine.Random.Range(-10f,10f);

                GameObject chunk = Instantiate(breakableTileChunk, spawnPos, Quaternion.identity);

                if (data.sprite != null)
                    chunk.GetComponent<SpriteRenderer>().sprite = data.sprite;

                StartCoroutine(MoveChunkToPosition(chunk, targetPos, cellPos, data.tile));
            }

            // Move player UP one tile (THIS is the key)
            Vector3 nextPos = player.transform.position + Vector3.up * (groundTilemap.cellSize.y + 1f);
            //Vector3 nextPos = targetPos.position;
            StartCoroutine(MovePlayer(player.transform, nextPos));

            yield return new WaitForSeconds(riseRowDelay);
        }

        StartCoroutine(MovePlayer(player.transform, targetPos.position));
        cachedTiles.Clear();

        lightSprite.SetActive(false);
        arrowSprite.SetActive(false);

        playerController.enabled = true;
        var playerInput = playerController.GetComponent<PlayerInput>();
        playerInput.enabled = true;
        playerInput.UnFreeze?.Invoke();
        playerInput.CancelHorizontal?.Invoke();
        rb.gravityScale = 1f;

        var playerAnimation = playerInput.GetComponentInChildren<PlayerAnimation>();
        playerAnimation.ToggleSpriteRenderer(true); //if disable by death effect in dungeon
        playerAnimation.ToggleSpriteOrder(1);
        playerAnimation.ToggleTrailRenderer(true);
        playerController.GetComponent<CreatureChain>().SpriteSortChain(1);


        poundCollider.enabled = true;
        this.enabled = false;

        LevelManager.Instance.ToggleLevelParallaxLayers(Camera.main.transform);
        LevelManager.Instance.InDungeon = false;
        LevelManager.Instance.startLevelTimer = true; //continue level timer
    }
    IEnumerator MovePlayer(Transform player, Vector3 targetPos)
    {
        float duration = 0.12f;
        float t = 0f;

        Vector3 start = player.position;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float eased = Mathf.SmoothStep(0, 1, t);

            player.position = Vector3.Lerp(start, targetPos, eased);

            yield return null;
        }

        player.position = targetPos;
    }
    IEnumerator MoveChunkToPosition(GameObject chunk, Vector3 targetPos, Vector3Int cellPos, TileBase tile)
    {
        float duration = UnityEngine.Random.Range(0.2f,0.3f);
        float t = 0f;

        Vector3 startPos = chunk.transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float eased = Mathf.SmoothStep(0, 1, t);

            chunk.transform.position = Vector3.Lerp(startPos, targetPos, eased);

            // Rotate while moving (nice polish)
            chunk.transform.Rotate(0, 0, 720 * Time.deltaTime);

            float scale = Mathf.Lerp(0.8f, 1.1f, eased);
            chunk.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        chunk.transform.position = targetPos;

        // Restore tile
        groundTilemap.SetTile(cellPos, tile);
        SoundManager.Instance.PlayTileRegroupSFx();

        Destroy(chunk);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out var player))
        {
            Debug.Log("transition triggered");
            playerController = player;
            player.ResetPound();
            player.GetComponent<Rigidbody2D>().gravityScale = 0f;
            LevelManager.Instance.sceneTransitionManager.TriggerSecretRoomTransition();
            LevelManager.Instance.InDungeon = true;
            triggerCollider.enabled = false;
            poundCollider.enabled = false;

        }
    }
    

#if UNITY_EDITOR
    void SnapToGrid()
    {
        if (!groundTilemap) return;

        Vector3Int cell = groundTilemap.WorldToCell(transform.position);
        Vector3 snapped = groundTilemap.GetCellCenterWorld(cell);

        if (transform.position != snapped)
            transform.position = snapped;
    }
#endif
}
