using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Tile : MonoBehaviour
{
    [Header("Fall Settings")]
    public float fallSpeed = 7f;
    public string poolTag = "Tile";

    private bool isFailedTile = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        isFailedTile = false;
        transform.DOKill();
        spriteRenderer?.DOKill();

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0f, 0f, 0f, 1f);

        if (col != null) col.enabled = true;

        GameEvents.OnWorldRollback += Rollback;
    }

    void OnDisable()
    {
        GameEvents.OnWorldRollback -= Rollback;
    }

    void Update()
    {
        if (!GameManager.Instance.IsPlaying || GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
            return;

        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.IsGameOver) return;

        if (GameManager.Instance.IsPaused)
        {
            if (!isFailedTile) return;
            GameManager.Instance.ResumeGame();
            isFailedTile = false;
        }

        if (col != null) col.enabled = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            spriteRenderer.DOFade(0f, 0.15f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                SimplePooler.Instance.ReturnToPool(poolTag, gameObject);
            });
        }
        else
        {
            SimplePooler.Instance.ReturnToPool(poolTag, gameObject);
        }

        GameManager.Instance.ProcessTileHit(transform);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MissLine") && !GameManager.Instance.IsPaused)
        {
            isFailedTile = true;
            GameManager.Instance.ProcessTileMiss(transform.position);
            GameEvents.TriggerWorldRollback(-transform.position.y);
        }
    }

    private void Rollback(float distance)
    {
        transform.DOMoveY(transform.position.y + distance, 0.5f).SetEase(Ease.OutBack);
    }
}