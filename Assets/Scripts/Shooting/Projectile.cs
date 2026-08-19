using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    public static event Action ProjectileDestroyed;

    static int activeProjectiles = 0;
    public static bool HasActiveProjectiles => activeProjectiles > 0;


    [SerializeField] Shockwave shockwavePrefab;

    #region Config
    ProjectileConfig config;
    float moveSpeed;
    Vector3 moveDirection;
    #endregion

    #region State
    bool isLaunched;
    float totalTraveledDistance = 0f; // storing the distance instead of starting position is slightly more efficient
    public float Radius => transform.localScale.x / 2f;
    #endregion

    readonly RaycastHit[] raycastResuts = new RaycastHit[10]; // Used for non-alocating spherecast checks
    readonly Collider[] hitColliders = new Collider[100]; // Used for non-alocating sphere overlap checks



    void OnEnable()
    {
        activeProjectiles++;
    }

    void OnDisable()
    {
        activeProjectiles--;
        ProjectileDestroyed?.Invoke();
    }

    public void Launch(Vector3 direction, ProjectileConfig projectileConfig)
    {
        config = projectileConfig;
        moveDirection = direction.normalized;
        moveSpeed = config.CalculateProjectileSpeed(transform.localScale.x);

        if (moveDirection != Vector3.zero)  // Align visual orientation to match direction
            transform.rotation = Quaternion.LookRotation(moveDirection);

        isLaunched = true;
    }

    void Update()
    {
        PerformMovementStep();
    }

    void PerformMovementStep()
    {
        if (!isLaunched)
            return;

        float stepMovementDistance = moveSpeed * Time.deltaTime;

        if (TryGetClosestHit(stepMovementDistance, out RaycastHit foundClosestHit))
        {
            transform.position += moveDirection * foundClosestHit.distance;
            OnObstacleHit();
        }
        else
        {
            transform.position += moveDirection * stepMovementDistance;
            totalTraveledDistance += stepMovementDistance;
            if (totalTraveledDistance >= config.SelfDestructDistance)
                SelfDestruct();
        }
    }

    bool TryGetClosestHit(float distanceToCheck, out RaycastHit closestHit)
    {
        int hitCount = Physics.SphereCastNonAlloc(transform.position, Radius, moveDirection, raycastResuts, distanceToCheck, GameLayers.ProjectileCollisionMask);
        
        if (hitCount == 0)
        {
            closestHit = default;
            return false;
        }

        closestHit = FindClosestHit(raycastResuts, hitCount);
        return true;
    }

    /// <summary>
    /// Finds the closest hit in the given array of RaycastHit results.
    /// Using the custom method instead of LINQ allows to avoid unnecessary memory allocations,
    /// which is crucial when used inside the Update loop.
    /// </summary>
    /// <param name="hitsArray">An array containing hits. Presumed to have at least 1 valid hit.</param>
    /// <param name="hitCount">How many hits were found.
    /// Required because the length of the hits array used for storage of non-allocating casts isn't guaranteed to match the actual number of hits.</param>
    /// <returns>The found closest hit</returns>
    RaycastHit FindClosestHit(RaycastHit[] hitsArray, int hitCount)
    {
        RaycastHit closestHit = hitsArray[0];
        for (int i = 1; i < hitCount; i++)
            if (hitsArray[i].distance < closestHit.distance)
                closestHit = hitsArray[i];

        return closestHit;
    }

    void OnObstacleHit()
    {
        float explosionSize = config.CalculateExplosionSize(transform.localScale.x);
        ApplyDamage(explosionSize);
        SpawnExlosionFX(explosionSize);

        Destroy(gameObject); // Object pooling migh be implemented here in the future
    }

    void ApplyDamage(float explosionSize)
    {
        float explosionRadius = explosionSize / 2f;
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, hitColliders, GameLayers.ProjectileCollisionMask);

        // Calculate delays for every obstacle based on how fast the shockwave reaches them, and apply damage
        for (int i = 0; i < hitCount; i++)
        {
            Collider obstacleCollider = hitColliders[i];
            float distance = Vector3.Distance(transform.position, obstacleCollider.transform.position);
            float timeToReach = (distance / explosionRadius) * config.ExplosionDuration; // Shockwave is presumed to travel at constant speed

            if (obstacleCollider.TryGetComponent(out Obstacle obstacle))
                obstacle.ApplyDamage(timeToReach);
        }
    }

    void SpawnExlosionFX(float explosionSize)
    {
        Shockwave shockwave = Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        shockwave.Play(transform.localScale.x, explosionSize, config.ExplosionDuration);
    }

    void SelfDestruct()
    {
        Destroy(gameObject); // Object pooling migh be implemented here in the future
    }
}
