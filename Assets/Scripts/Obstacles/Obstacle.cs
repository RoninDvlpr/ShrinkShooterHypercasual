using System;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static event Action OnObstacleDesroyed;

    /// <summary>
    /// Applies damage to the obstacle after a specified delay.
    /// </summary>
    /// <param name="delay">How much time to wait before applying damage</param>
    public void ApplyDamageDelayed(float delay)
    {
        Destroy(gameObject, delay); // Object pooling migh be implemented here in the future
        GetComponent<Collider>().enabled = false;
        OnObstacleDesroyed?.Invoke();
    }
}
