using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayImage : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float defaultFadeDuration = 0.5f;
    [SerializeField] float defaultFadeCyclePause = 0.25f;


    public Tween PerformFade(float endValue, float duration, Action onCompleteCallback = null)
    {
        Tween tween = canvasGroup.DOFade(endValue, duration);

        if (onCompleteCallback != null)
            tween.OnComplete(() => onCompleteCallback?.Invoke());

        return tween;
    }

    #region Fade Out

    public void FadeOut(Action onCompleteCallback = null)
    {
        FadeOut(defaultFadeDuration, onCompleteCallback);
    }

    public void FadeOut(float duration, Action onCompleteCallback = null)
    {
        PerformFade(1f, duration, onCompleteCallback);
    }

    #endregion


    #region Fade In

    public void FadeIn(Action onCompleteCallback = null)
    {
        FadeIn(defaultFadeDuration, onCompleteCallback);
    }

    public void FadeIn(float duration, Action onCompleteCallback = null)
    {
        PerformFade(0f, duration, onCompleteCallback: onCompleteCallback);
    }

    #endregion


    #region Fade Cycle

    public void PerformDefaultFadeCycle(Action onScreenFadedCallback = null)
    {
        PerformFadeCycle(defaultFadeDuration, defaultFadeCyclePause, onScreenFadedCallback);
    }

    public void PerformFadeCycle(float fadeStageDuration, float pauseDuration = 0f, Action onScreenFadedCallback = null)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append( canvasGroup.DOFade(1f, fadeStageDuration) ); // Fade Out

        if (onScreenFadedCallback != null)
            sequence.AppendCallback(() => onScreenFadedCallback.Invoke()); // On screen faded callback

        if (pauseDuration > 0f)
            sequence.AppendInterval(pauseDuration); // Pause

        sequence.Append(canvasGroup.DOFade(0f, fadeStageDuration)); // Fade In
    }

    #endregion
}
