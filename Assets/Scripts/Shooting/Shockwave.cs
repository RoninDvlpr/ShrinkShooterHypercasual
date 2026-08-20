using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shockwave : MonoBehaviour
{
    [SerializeField] Transform shockwavesParent;
    [Tooltip("The renderers that will be affected by the shockwave's alpha fade-out. The used shader is supposed to be the UnversalShader asset that has the '_GlobalAlpha' property.")]
    [SerializeField] List<Renderer> shockwaveRenderers;
    static readonly int GlobalAlphaPropertyID = Shader.PropertyToID("_GlobalAlpha");

    [Header("Animation Config")]
    [Tooltip("Duration of the fade-out phase relative to the expansion phase. The fade-out duration will be the expansion duration multiplied by this coefficient.")]
    [SerializeField] float fadeOutDurationCoefficient = 1.65f;
    [Tooltip("The overshoot coefficient determines how much the sphere will scale relative to the blast size during the fade-out phase. The final size will be the blast size multiplied by this coefficient.")]
    [SerializeField] float overshootCoefficient = 1.15f;

    [Header("Defaults")]
    [SerializeField] float defaultDuration = 0.15f;
    [Tooltip("The default expansion coefficient is used when the shockwave is played without specifying a blast radius. The final radius will be the current radius multiplied by this coefficient.")]
    [SerializeField] float defaultExpansionCoefficient = 3f;

    /// <summary>
    /// Used as a temporary storage for material property block to avoid GC allocations during runtime.
    /// </summary>
    MaterialPropertyBlock propBlock;


    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
    }

    /// <summary>
    /// Launches the shockwave animation with the default duration and expansion.
    /// </summary>
    /// <param name="onCompleteCallback">Gets called after the animation ends</param>
    public void Play(Action onCompleteCallback = null)
    {
        float defaultBlastSize = shockwavesParent.lossyScale.x * defaultExpansionCoefficient;
        Play(defaultBlastSize, onCompleteCallback);
    }

    /// <summary>
    /// Launches the shockwave animation with the default duration.
    /// </summary>
    /// <param name="targetBlastSize">The global size the shockwave should have at the end of the animation</param>
    /// <param name="onCompleteCallback">Gets called after the animation ends</param>
    public void Play(float targetBlastSize, Action onCompleteCallback = null)
    {
        Play(targetBlastSize, defaultDuration, onCompleteCallback);
    }

    /// <summary>
    /// Launches the shockwave animation with a specified starting size.
    /// Used to match the shockwave size to the projectile size when the projectile is destroyed.
    /// </summary>
    /// <param name="startingBlastSize">The starting size of the shockwave effect</param>
    /// <param name="targetBlastSize">The global size the shockwave should have at the end of the animation</param>
    /// <param name="duration">The explosion animation duration</param>
    /// <param name="onCompleteCallback">Gets called after the animation ends</param>
    public void Play(float startingBlastSize, float targetBlastSize, float duration, Action onCompleteCallback = null)
    {
        transform.localScale = Vector3.one * startingBlastSize;
        Play(targetBlastSize, duration, onCompleteCallback);
    }

    /// <summary>
    /// Launches the shockwave animation.
    /// </summary>
    /// <param name="targetBlastSize">The global size the shockwave should have at the end of the animation</param>
    /// <param name="duration">The explosion animation duration</param>
    /// <param name="onCompleteCallback">Gets called after the animation ends</param>
    public void Play(float targetBlastSize, float duration, Action onCompleteCallback = null)
    {
        float localizedTargetBlastSize = CalculateShockwaveLocalBlastSize(targetBlastSize);

        Sequence explosionSequence = DOTween.Sequence();

        // Phase A: Rapid explosive expansion
        explosionSequence.Append(
            shockwavesParent.DOScale(localizedTargetBlastSize, duration).SetEase(Ease.OutQuad)
        );

        // Phase B: Eased die-out (Slight scale over-shoot + alpha fade)
        float overshootSize = localizedTargetBlastSize * overshootCoefficient;
        float fadeDuration = duration * fadeOutDurationCoefficient;
        explosionSequence.Append(
            shockwavesParent.DOScale(overshootSize, fadeDuration).SetEase(Ease.OutCubic)
        );

        // Parallel Alpha Fadeout using MaterialPropertyBlock
        explosionSequence.Join(
            DOVirtual.Float(1f, 0f, fadeDuration, SetRenderersAlpha).SetEase(Ease.OutSine)
        );

        explosionSequence.OnComplete(() =>
        {
            onCompleteCallback?.Invoke();
            Destroy(gameObject); // Object pooling migh be implemented here in the future
        });
    }

    void SetRenderersAlpha(float alpha)
    {
        foreach (Renderer renderer in shockwaveRenderers)
            SetRendererAlpha(renderer, alpha);
    }

    void SetRendererAlpha(Renderer renderer, float alpha)
    {
        renderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(GlobalAlphaPropertyID, alpha);
        renderer.SetPropertyBlock(propBlock);
    }

    float CalculateShockwaveLocalBlastSize(float targetGlobalSize)
    {
        float scaleCoefficient = shockwavesParent.localScale.x / shockwavesParent.lossyScale.x;
        return targetGlobalSize * scaleCoefficient;
    }
}
