using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shockwave : MonoBehaviour
{
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
    public void Play(Action onCompleteCallback = null)
    {
        Play(transform.localScale.x * defaultExpansionCoefficient, onCompleteCallback);
    }
    public void Play(float blastSize, Action onCompleteCallback = null)
    {
        Play(blastSize, defaultDuration, onCompleteCallback);
    }
    public void Play(float startingBlastSize, float finalBlastSize, float duration, Action onCompleteCallback = null)
    {
        transform.localScale = Vector3.one * startingBlastSize;
        Play(finalBlastSize, duration, onCompleteCallback);
    }
    public void Play(float blastSize, float duration, Action onCompleteCallback = null)
    {
        Sequence explosionSequence = DOTween.Sequence();

        // Phase A: Rapid explosive expansion
        explosionSequence.Append(
            transform.DOScale(blastSize, duration).SetEase(Ease.OutQuad)
        );

        // Phase B: Eased die-out (Slight scale over-shoot + alpha fade)
        float overshootSize = blastSize * overshootCoefficient;
        float fadeDuration = duration * fadeOutDurationCoefficient;
        explosionSequence.Append(
            transform.DOScale(overshootSize, fadeDuration).SetEase(Ease.OutCubic)
        );

        // Parallel Alpha Fadeout using MaterialPropertyBlock
        explosionSequence.Join(
            DOVirtual.Float(1f, 0f, fadeDuration, SetRenderersAlpha).SetEase(Ease.OutSine)
        );

        explosionSequence.OnComplete(() =>
        {
            onCompleteCallback?.Invoke();
            gameObject.SetActive(false);
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
}
