using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Target target;
    [SerializeField] float failScaleThreshold = 0.25f;

    #region State
    public bool IsMovingToTarget { get; private set; }
    public bool IsDepleted => transform.localScale.x <= failScaleThreshold;
    public bool CanShoot => !IsDepleted && !IsMovingToTarget;
    #endregion

    public event Action OnScaleUpdated, OnDepleted, OnMovingToTarget;

    readonly RaycastHit[] raycastResuts = new RaycastHit[10]; // Used for non-alocating boxcast checks



    void OnEnable()
    {
        Obstacle.OnObstacleDesroyed += CheckPathIsClear;
    }

    void OnDisable()
    {
        Obstacle.OnObstacleDesroyed -= CheckPathIsClear;
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
            OnDepleted?.Invoke();

        return !IsDepleted;
    }

    void CheckPathIsClear()
    {
        if (IsMovingToTarget == true)
            return;

        int hitCount = Physics.BoxCastNonAlloc(transform.position, transform.localScale / 2f, transform.forward, raycastResuts, transform.rotation, 20f);
        if (hitCount == 0)
            TriggerForwardMovement();
    }

    public void TriggerForwardMovement()
    {
        IsMovingToTarget = true;
        // Animation and movement logic goes here
        Debug.Log("Path cleared, moving forward!");
    }
}