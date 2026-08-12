using UnityEngine;

public class ShootingController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] PlayerController player;
    [SerializeField] TouchInputManager inputManager;

    [Header("Shooting Config")]
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] ProjectileConfig projectileConfig;
    [SerializeField] float spawnDistance;

    Projectile currentProjectile;
    bool IsCharging => currentProjectile != null;


    void OnEnable()
    {
        inputManager.OnPrimaryTouchStarted += HandleTouchStart;
        inputManager.OnPrimaryTouchEnded += HandleTouchEnd;
    }

    void OnDisable()
    {
        inputManager.OnPrimaryTouchStarted -= HandleTouchStart;
        inputManager.OnPrimaryTouchEnded -= HandleTouchEnd;
    }

    void HandleTouchStart()
    {
        if (!player.CanShoot)
            return;

        if (IsCharging)
        {
            Debug.LogWarning("Already charging a projectile. Cannot start a new one!");
            return;
        }

        currentProjectile = Instantiate(projectilePrefab, transform.position + Vector3.forward * spawnDistance, Quaternion.identity);
    }

    void HandleTouchEnd()
    {
        ReleaseShot();
    }

    void Update()
    {
        ScaleProjectile();
    }

    void ScaleProjectile()
    {
        if (!IsCharging)
            return;

        // Basing the scale calculation on a single axis to prevent non-uniform scale due to float drift over time
        float newProjectileScale = currentProjectile.transform.localScale.x + projectileConfig.ProjectileGrowthRate * Time.deltaTime;
        bool projectileLimitReached = newProjectileScale >= projectileConfig.MaxProjectileScale;
        currentProjectile.transform.localScale = projectileLimitReached ? Vector3.one * projectileConfig.MaxProjectileScale : Vector3.one * newProjectileScale;

        float playerScaleDelta = projectileConfig.PlayerShrinkRate * Time.deltaTime;
        if (!player.ConsumeScale(playerScaleDelta) || projectileLimitReached)
            ReleaseShot();
    }

    void ReleaseShot()
    {
        if (!IsCharging)
            return;

        currentProjectile.Launch(player.TargetPosition, projectileConfig);
        currentProjectile = null;
    }
}