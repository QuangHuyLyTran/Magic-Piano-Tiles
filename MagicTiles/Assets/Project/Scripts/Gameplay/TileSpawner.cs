using UnityEngine;
using DG.Tweening;

public class TileSpawner : MonoBehaviour
{
    public Transform[] laneSpawnPoints;
    public float spawnInterval = 0.5f;
    public string tileTag = "Tile";

    private float timer;

    void OnEnable() => GameEvents.OnWorldRollback += Rollback;
    void OnDisable() => GameEvents.OnWorldRollback -= Rollback;

    void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTile();
            timer = 0f;
        }
    }

    void SpawnTile()
    {
        if (laneSpawnPoints.Length == 0) return;
        int randomLane = Random.Range(0, laneSpawnPoints.Length);
        Transform spawnPoint = laneSpawnPoints[randomLane];
        SimplePooler.Instance.SpawnFromPool(tileTag, spawnPoint.position, Quaternion.identity);
    }

    private void Rollback(float distance)
    {
        transform.DOMoveY(transform.position.y + distance, 0.5f).SetEase(Ease.OutBack);
    }
}