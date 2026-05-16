using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<int, string, Vector3> OnTileHit;
    public static Action<Vector3> OnTileMiss;
    public static Action<int> OnGameOver;
    public static Action<float> OnWorldRollback;

    public static void TriggerTileHit(int points, string rating, Vector3 pos) => OnTileHit?.Invoke(points, rating, pos);
    public static void TriggerTileMiss(Vector3 pos) => OnTileMiss?.Invoke(pos);
    public static void TriggerGameOver(int finalScore) => OnGameOver?.Invoke(finalScore);
    public static void TriggerWorldRollback(float distance) => OnWorldRollback?.Invoke(distance);
}