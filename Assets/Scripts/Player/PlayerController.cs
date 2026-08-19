using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerAnimationController animationController;
    [SerializeField] Target target;
    public Vector3 TargetPosition => target.transform.position;
    [SerializeField] float failScaleThreshold = 0.25f;
    Vector3 initialPlayerScale;
    Vector3 initialPlayerPosition;

    #region State
    public bool IsMovingToTarget => animationController.IsPlayingMovementAnimation;
    public bool IsDepleted => transform.localScale.x <= failScaleThreshold;
    public bool CanShoot => !IsDepleted && !IsMovingToTarget;
    #endregion

    public event Action OnScaleUpdated, OnDepleted, OnTargetReached, OnLevelFailed;

    readonly RaycastHit[] raycastResuts = new RaycastHit[10]; // Used for non-alocating boxcast checks



    void Awake()
    {
        initialPlayerPosition = transform.position;
        initialPlayerScale = transform.localScale;
    }

    void OnEnable()
    {
        Obstacle.ObstacleRemoved += CheckLevelCompletion;
        Projectile.ProjectileDestroyed += CheckLevelCompletion;
    }

    void OnDisable()
    {
        Obstacle.ObstacleRemoved -= CheckLevelCompletion;
        Projectile.ProjectileDestroyed -= CheckLevelCompletion;
    }

    /// <summary>
    /// Decreases the player's scale by a specified amount. Returns true if the player is still above the fail threshold, false if depleted. 
    /// </summary>
    /// <param name="amountToConsume">How much the player scale should be decreased</param>
    /// <returns>If the scale was consumed succesfully</returns>
    public bool ConsumeScale(float amountToConsume)
    {
        // Basing the scale calculation on a single axis to prevent non-uniform scale due to float drift over time
        transform.localScale = Vector3.one * (transform.localScale.x - amountToConsume);
        OnScaleUpdated?.Invoke();

        if (IsDepleted)
        {
            OnDepleted?.Invoke();
            CheckLevelCompletion();
        }

        return !IsDepleted;
    }

    void CheckLevelCompletion()
    {
        if (IsMovingToTarget == true)
            return;

        if (CheckPathIsClear())
            TravelToTarget();
        else if (IsDepleted && !Projectile.HasActiveProjectiles && !Obstacle.HasObstaclesAwaitingRemoval)
            OnLevelFailed?.Invoke();
    }

    bool CheckPathIsClear()
    {
        float height = animationController.RequiredClearanceHeight;
        Vector3 center = transform.position + Vector3.up * height / 2f;
        Vector3 halfExtents = (transform.localScale + Vector3.up * height) / 2f;
        Vector3 directionToTarget = (TargetPosition - transform.position).normalized;
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
        float distance = Vector3.Distance(transform.position, TargetPosition);

        int hitCount = Physics.BoxCastNonAlloc(center, halfExtents, directionToTarget, raycastResuts, rotationToTarget, distance, GameLayers.ObstacleMask);
        return hitCount == 0;
    }

    public void TravelToTarget()
    {
        target.StartWatchingPlayer(transform);
        animationController.MoveToTarget(TargetPosition, () => OnTargetReached?.Invoke());
    }

    public void Reset()
    {
        transform.position = initialPlayerPosition;
        transform.localScale = initialPlayerScale;
        OnScaleUpdated?.Invoke();
    }
}