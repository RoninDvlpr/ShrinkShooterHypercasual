using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using System;


/// <summary>
/// Implements touch input handling for mobile devices using the new input system's Enhanced Touch API.
/// Enhanced Touch ensures a more robust multi-touch handling compared to other methods.
/// NOTE: Simulate Touch Input by Mouse or Pen should be enabled in the Window -> Analysis > Input Debugger > Options menu for testing in the Editor.
/// </summary>
public class TouchInputManager : MonoBehaviour
{
    public event Action OnPrimaryTouchStarted;
    public event Action OnPrimaryTouchEnded;

    Finger activeFinger = null;


    void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        Touch.onFingerDown += HandleFingerDown;
        Touch.onFingerUp += HandleFingerUp;
    }

    void OnDisable()
    {
        Touch.onFingerDown -= HandleFingerDown;
        Touch.onFingerUp -= HandleFingerUp;

        EnhancedTouchSupport.Disable();
    }

    /// <summary>
    /// Tracks a primary touch to invoke the OnPrimaryTouchStarted event.
    /// </summary>
    /// <param name="finger">The finger that triggered the handled onFingerDown event</param>
    void HandleFingerDown(Finger finger)
    {
        // If we are already tracking a finger, ignore new ones
        if (activeFinger != null) return;

        // UI click-through protection
        //if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(finger.index))
        //    return;

        activeFinger = finger;
        OnPrimaryTouchStarted?.Invoke();
    }

    /// <summary>
    /// Tracks if the primary touch has ended to invoke the OnPrimaryTouchEnded event.
    /// </summary>
    /// <param name="finger">The finger that triggered the handled onFingerUp event</param>
    void HandleFingerUp(Finger finger)
    {
        // Only fire the release event if the tracked finger is lifted
        if (finger == activeFinger)
        {
            activeFinger = null;
            OnPrimaryTouchEnded?.Invoke();
        }
    }
}