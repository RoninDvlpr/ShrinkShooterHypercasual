using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] float defaultDuration = 0.15f;
    [Tooltip("The default expansion coefficient is used when the shockwave is played without specifying a blast radius. The final radius will be the current radius multiplied by this coefficient.")]
    [SerializeField] float defaultExpansionCoefficient = 3f;
    
    Vector3 startingScale;
    Vector3 targetScale;
    float duration;
    float elapsedTime;
    bool isAnimating;


    public void Play()
    {
        Play(transform.localScale.x * defaultExpansionCoefficient);
    }

    public void Play(float blastSize)
    {
        Play(blastSize, defaultDuration);
    }

    public void Play(float startingBlastSize, float finalBlastSize, float duration)
    {
        transform.localScale = Vector3.one * startingBlastSize;
        Play(finalBlastSize, duration);
    }

    /// <summary>
    /// Starts the shockwave animation.
    /// NOTE: This method stores the current transform scale as the starting scale value, so it should be called after setting the initial scale if needed.
    /// </summary>
    /// <param name="blastSize">A size that the shockwave will have at the animation end</param>
    /// <param name="duration">How much time the whole animation takes</param>
    public void Play(float blastSize, float duration)
    {
        startingScale = transform.localScale;
        targetScale = Vector3.one * blastSize;
        this.duration = duration;

        elapsedTime = 0f;
        isAnimating = true;
    }

    void Update()
    {
        UpdateShockwaveAnimation();
    }

    void UpdateShockwaveAnimation()
    {
        if (!isAnimating)
            return;

        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / duration;
        float easeCurve = Mathf.Sqrt(progress);

        if (progress >= 1f)
        {
            transform.localScale = targetScale;
            isAnimating = false;

            Destroy(gameObject); //object pooling migh be implemented here in the future
        }
        else
            transform.localScale = Vector3.Lerp(startingScale, targetScale, easeCurve);
    }
}
