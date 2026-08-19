using UnityEngine;

public static class GameLayers
{
    static readonly int ObstacleIndex;
    static readonly int TargetIndex;

    public static readonly LayerMask ObstacleMask;
    public static readonly LayerMask TargetMask;
    public static readonly LayerMask ProjectileCollisionMask;

    static GameLayers()
    {
        ObstacleIndex = LayerMask.NameToLayer("Obstacles");
        TargetIndex = LayerMask.NameToLayer("Target");

        ObstacleMask = 1 << ObstacleIndex;
        TargetMask = 1 << TargetIndex;

        ProjectileCollisionMask = ObstacleMask | TargetMask;
    }
}