using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileConfig", menuName = "Gameplay/Projectile Config")]
public class ProjectileConfig : ScriptableObject
{
    [Header("Movement & Range")]
    [SerializeField] float minSpeed = 5f;
    public float MinSpeed => minSpeed;
    [SerializeField] float maxSpeed = 10f;
    public float MaxSpeed => maxSpeed;
    [Tooltip("How far the projectile travels before auto-destroying")]
    [SerializeField] float selfDestructDistance = 50f;
    public float SelfDestructDistance => selfDestructDistance;


    [Header("Charging Mechanics")]
    [Tooltip("How fast the projectile scales up per second while charging")]
    [SerializeField] float projectileGrowthRate = 0.5f;
    public float ProjectileGrowthRate => projectileGrowthRate;
    [Tooltip("Multiplier for how much scale it drains from the player. 1 = 1:1 ratio with projectile growth.")]
    [SerializeField] float playerScaleCostMultiplier = 0.2f;
    public float PlayerScaleCostMultiplier => playerScaleCostMultiplier;
    public float PlayerShrinkRate => projectileGrowthRate * playerScaleCostMultiplier;


    [Header("Size Constraints")]
    [SerializeField] float minProjectileScale = 0.1f;
    public float MinProjectileScale => minProjectileScale;
    [SerializeField] float maxProjectileScale = 1f;
    public float MaxProjectileScale => maxProjectileScale;


    [Header("Explosion")]
    [Tooltip("Defines the diameter of the explosion depending on the projectile scale")]
    [SerializeField] AnimationCurve explosionSizeMultiplierCurve = AnimationCurve.Linear(0f, 3f, 1f, 3f);
    [SerializeField] float explosionDuration = 0.15f;
    public float ExplosionDuration => explosionDuration;



    /// <summary>
    /// The Projectile uses this method to get the explosion size corresponding to its current scale.
    /// </summary>
    /// <param name="currentProjectileScale">A scale of the exploding projectile</param>
    /// <returns>An explosion size for the given projectile scale</returns>
    public float CalculateExplosionSize(float currentProjectileScale)
    {
        float scaleRatio = GetCurrentScaleRatio(currentProjectileScale);
        float sizeMultiplier = explosionSizeMultiplierCurve.Evaluate(scaleRatio);
        return currentProjectileScale * sizeMultiplier;
    }

    /// <summary>
    /// Calculate the projectile speed based on its current scale.
    /// </summary>
    /// <param name="currentProjectileScale">A projectile scale to calculate the speed for</param>
    /// <returns>Speed for the given projectile scale</returns>
    public float CalculateProjectileSpeed(float currentProjectileScale)
    {
        float scaleRatio = GetCurrentScaleRatio(currentProjectileScale);
        return Mathf.Lerp(MaxSpeed, MinSpeed, scaleRatio);
    }

    /// <summary>
    /// Calculates the scale ratio between the minimum and maximum scale as a normalized [0, 1] value.
    /// </summary>
    /// <param name="currentProjectileScale">A projectile scale to calculate the ratio for</param>
    /// <returns>A normalized ratio between 0 and 1</returns>
    public float GetCurrentScaleRatio(float currentProjectileScale)
    {
        return Mathf.InverseLerp(MinProjectileScale, MaxProjectileScale, currentProjectileScale);
    }
}
