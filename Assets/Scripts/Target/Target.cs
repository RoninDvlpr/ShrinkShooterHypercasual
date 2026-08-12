using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] Transform doorTransform;
    [SerializeField] float doorOpenDistanceThreshold = 5f;
    [SerializeField] float doorSlideDistance = 3.5f;
    [SerializeField] float doorSlideDuration = 0.5f;

    Vector3 initialDoorPosition;


    void Awake()
    {
        initialDoorPosition = doorTransform.position;
    }

    /// <summary>
    /// Start watching the distance to the player.
    /// Used to prevent checking the distance every frame in Update.
    /// </summary>
    /// <param name="playerTransform"></param>
    public void StartWatchingPlayer(Transform playerTransform)
    {
        StartCoroutine(WatchDistanceRoutine(playerTransform));
    }

    IEnumerator WatchDistanceRoutine(Transform playerTransform)
    {
        float sqrThreshold = doorOpenDistanceThreshold * doorOpenDistanceThreshold;

        while ((transform.position - playerTransform.position).sqrMagnitude > sqrThreshold)
            yield return null;

        OpenDoor();
    }

    void OpenDoor()
    {
        doorTransform.DOMoveY(doorTransform.position.y + doorSlideDistance, doorSlideDuration).SetEase(Ease.InOutSine);
    }

    public void Reset()
    {
        doorTransform.position = initialDoorPosition;
    }
}
