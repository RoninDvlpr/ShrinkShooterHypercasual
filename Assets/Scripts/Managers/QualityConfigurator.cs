using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityConfigurator : MonoBehaviour
{
    private void Awake()
    {
        LockFramerate(60);
        SetResolution(1080);
    }

    void LockFramerate(int targetFramerate)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFramerate;
    }

    /// <summary>
    /// Sets the screen resolution based on a target height while maintaining the aspect ratio of the device's screen.
    /// Set the height to 1080 for crisp picture, or to 720 optimal performance.
    /// </summary>
    /// <param name="targetHeight">The target height of the sceen in pixels</param>
    void SetResolution(int targetHeight)
    {
        float screenRatio = (float)Screen.width / Screen.height;
        int targetWidth = Mathf.RoundToInt(targetHeight * screenRatio);
        Screen.SetResolution(targetWidth, targetHeight, true);
    }
}
