using DG.Tweening;
using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Bounce Animation")]
    [SerializeField] float bounceHeight = 2.0f;
    public float BounceHeight => bounceHeight;
    [SerializeField] float movementDuration = 1.5f;
    [SerializeField] int bounceCount = 4;

    /// <summary>
    /// Calculate the total vertical space required for the player to travel.
    /// Used for the BoxCast to check if the path is clear.
    /// </summary>
    public float RequiredClearanceHeight => transform.localScale.y + bounceHeight;
    public bool IsPlayingMovementAnimation => DOTween.IsTweening(transform);


    public void MoveToTarget(Vector3 targetPosition, Action onMovementFinishedCallback = null)
    {
        transform.DOJump(targetPosition, bounceHeight, bounceCount, movementDuration).SetEase(Ease.Linear).
            OnComplete(() => onMovementFinishedCallback?.Invoke());
    }
}