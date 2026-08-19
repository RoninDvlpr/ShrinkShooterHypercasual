using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static event Action ObstacleRemoved;

    static int obstaclesAwaitingRemoval = 0;
    public static bool HasObstaclesAwaitingRemoval => obstaclesAwaitingRemoval > 0;


    [SerializeField] List<Collider> colliders;
    [Tooltip("How high the obstacle pivot is from the bottom end of the obstacle. Used to push the obstacle up so it doesn't spawn inside the floor.")]
    [SerializeField] float pivotYOffset;
    public float PivotYOffset => pivotYOffset;
    [SerializeField] float obstacleSinkDistance = -2f;
    [SerializeField] protected float sinkDuration = 1f;
    


    /// <summary>
    /// Applies damage to the obstacle. An optional delay may be introduced before the damage is applied.
    /// </summary>
    /// <param name="delay">How much time to wait before applying damage</param>
    public void ApplyDamage(float delay = 0f)
    {
        obstaclesAwaitingRemoval++;
        PlayObstacleRemovalAnimation(OnObstacleRemoved, delay);
    }

    void OnObstacleRemoved()
    {
        SetCollidersState(false);
        obstaclesAwaitingRemoval--;
        ObstacleRemoved?.Invoke();
    }

    void SetCollidersState(bool enable)
    {
        foreach (Collider collider in colliders)
            collider.enabled = enable;
    }

    protected virtual void PlayObstacleRemovalAnimation(Action onRemovalAnimationFinished = null, float animationDelay = 0f)
    {
        ObstacleSinkingAnimation(onRemovalAnimationFinished, animationDelay);
    }

    void ObstacleSinkingAnimation(Action onRemovalAnimationFinished = null, float animationDelay = 0f)
    {
        Tween tween = transform.DOMoveY(transform.position.y + obstacleSinkDistance, sinkDuration).SetEase(Ease.InOutQuad);

        if (animationDelay > 0f)
            tween.SetDelay(animationDelay);

        if (onRemovalAnimationFinished != null)
            tween.OnComplete(() => onRemovalAnimationFinished.Invoke());
    }
}
