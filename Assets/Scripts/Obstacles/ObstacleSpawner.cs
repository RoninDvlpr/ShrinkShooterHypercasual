using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] Obstacle obstaclePrefab;

    [Header("Spawn Settings")]
    [SerializeField] Vector2 spawnAreaSize = new Vector2(10f, 20f);
    [SerializeField] int targetObstacleCount = 30;

    [Tooltip("The minimum distance between the centers of two obstacles to prevent overlap.")]
    [SerializeField] float minDistanceBetweenObstacles = 1.5f;

    [Tooltip("How many times the algorithm should try to find a valid spot before giving up on an obstacle.")]
    [SerializeField] int maxPlacementAttempts = 30;

    [Tooltip("If enabled, each spawned obstacle will be given a random 90-degree step rotation around the Y-axis.")]
    [SerializeField] bool applyRandomRotation;



    void Start()
    {
        SpawnObstacles();
    }

    public void SpawnObstacles()
    {
        List<Vector3> placedPositions = new List<Vector3>();

        for (int i = 0; i < targetObstacleCount; i++)
        {
            Vector3? validPosition = TryFindValidPosition(transform.position, spawnAreaSize, placedPositions, minDistanceBetweenObstacles, maxPlacementAttempts);

            if (validPosition.HasValue)
            {
                placedPositions.Add(validPosition.Value);
                Vector3 spawnPosition = validPosition.Value + Vector3.up * obstaclePrefab.PivotYOffset;
                Quaternion rotation = applyRandomRotation ? Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f) : Quaternion.identity;
                Instantiate(obstaclePrefab.gameObject, spawnPosition, rotation, transform);
            }
            else
            {
                Debug.LogWarning($"Could only fit {i} obstacles out of {targetObstacleCount}, the area is too dense. You may try increasing the maxPlacementAttempts.");
                break;
            }
        }
    }

    /// <summary>
    /// Attempts to find a random position that doesn't overlap with already placed positions.
    /// </summary>
    /// <param name="areaCenter">A center of the searched area</param>
    /// <param name="areaSize">A size of the searched area</param>
    /// <param name="placedPositions">Positions that are already occupied</param>
    /// <param name="minDistance">Minimal distance from already placed positions</param>
    /// <param name="maxAttempts">How many times the algorithm will try to generate a random position</param>
    /// <returns>A found valid position or null</returns>
    Vector3? TryFindValidPosition(Vector3 areaCenter, Vector2 areaSize, List<Vector3> placedPositions, float minDistance, int maxAttempts)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 randomPos = GetRandomPosition(areaCenter, areaSize);
            bool hasOverlap = CheckPositionOverlap(randomPos, placedPositions, minDistance);
            if (!hasOverlap)
                return randomPos;
        }
        
        return null;
    }

    bool CheckPositionOverlap(Vector3 positionToCheck, List<Vector3> placedPositions, float minDistance)
    {
        float sqrMinDistance = minDistance * minDistance;   // using squared distance to avoid sqrt calculations for performance

        for (int i = 0; i < placedPositions.Count; i++)
        {
            float sqrDistance = (positionToCheck - placedPositions[i]).sqrMagnitude;
            if (sqrDistance < sqrMinDistance)
                return true;
        }

        return false;
    }

    float GetRandomCoordinate(float rangeCenter, float rangeSize)
    {
        return UnityEngine.Random.Range(rangeCenter - rangeSize / 2f, rangeCenter + rangeSize / 2f);
    }

    Vector3 GetRandomPosition(Vector3 areaCenter, Vector2 areaSize)
    {
        float randomX = GetRandomCoordinate(areaCenter.x, areaSize.x);
        float randomZ = GetRandomCoordinate(areaCenter.z, areaSize.y);
        return new Vector3(randomX, 0f, randomZ);
    }

    public void Reset()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        SpawnObstacles();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y));
    }
}