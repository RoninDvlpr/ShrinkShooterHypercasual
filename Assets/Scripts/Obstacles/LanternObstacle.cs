using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LanternObstacle : Obstacle
{
    [SerializeField] ParticleSystem lanternLight;
    [SerializeField] float preSinkDelay = 0.35f;

    protected override void PlayObstacleRemovalAnimation(Action onRemovalAnimationFinished = null, float animationDelay = 0f)
    {
        DOVirtual.DelayedCall(animationDelay, () => lanternLight.Play());
        float sinkAnimationDelay = preSinkDelay + animationDelay;

        base.PlayObstacleRemovalAnimation(onRemovalAnimationFinished, sinkAnimationDelay);

        float lightDuration = sinkAnimationDelay + sinkDuration * 0.325f;
        DOVirtual.DelayedCall(lightDuration, () => lanternLight.Stop(true, ParticleSystemStopBehavior.StopEmitting));
    }
}
