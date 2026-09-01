using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileView : MonoBehaviour
{
    private static readonly Vector3 FlipAxis = new(1f, 1f, 0f);

    [Header("Sprites")]
    [SerializeField]
    private Sprite pollutedSprite;

    [SerializeField]
    private Sprite purifiedSprite;

    [Header("하이라이트")]
    [SerializeField]
    private Color previewColor = new Color(0f, 0.7f, 1f, 0.7f);

    [Header("애니메이션")]
    [SerializeField]
    [Range(0.01f, 2f)]
    private float changeDuration = 0.5f;

    [SerializeField]
    private AnimationCurve radiusCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField]
    private AnimationCurve softnessCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.2f, 0.2f),
        new Keyframe(0.7f, 0.2f),
        new Keyframe(1f, 0f)
    );

    [SerializeField]
    private float flipDelayMultiplier = 0.05f;

    [SerializeField]
    private float flipDuration = 0.5f;

    private Material material => spriteRenderer.material;

    private SpriteRenderer spriteRenderer;
    private Coroutine changeAnimationCoroutine = null;
    private Coroutine flipAnimationCoroutine = null;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer.sharedMaterial == null)
        {
            DebugConsole.LogError("[TileView] Missing material");
            return;
        }
    }

    public void SetHighlightColor(bool isHighlighted)
    {
        spriteRenderer.color = isHighlighted ? previewColor : Color.white;
    }

    public void PlayChangeAnimation(bool isPurified, bool immediately = false)
    {
        if (changeAnimationCoroutine != null)
        {
            StopCoroutine(changeAnimationCoroutine);
        }

        if (immediately)
        {
            CleanupChangeAnimation(isPurified);
            return;
        }

        changeAnimationCoroutine = StartCoroutine(ChangeAnimationCoroutine(isPurified));
    }

    public void PlayFlipAnimation(
        bool isShown,
        float delay,
        Action onComplete = null,
        bool immediately = false
    )
    {
        if (flipAnimationCoroutine != null)
        {
            StopCoroutine(flipAnimationCoroutine);
        }

        if (immediately)
        {
            CleanupFlipAnimation(isShown);
            return;
        }

        flipAnimationCoroutine = StartCoroutine(FlipAnimationCoroutine(isShown, delay, onComplete));
    }

    private IEnumerator ChangeAnimationCoroutine(bool isPurified)
    {
        CleanupChangeAnimation(!isPurified);

        float elapsed = 0f;
        while (elapsed < changeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / changeDuration);
            material.SetFloat(ShaderProps.CIRCLE_RADIUS, radiusCurve.Evaluate(t));
            material.SetFloat(ShaderProps.CIRCLE_SOFTNESS, softnessCurve.Evaluate(t));
            yield return null;
        }

        material.SetFloat(ShaderProps.CIRCLE_RADIUS, 1f);
        material.SetFloat(ShaderProps.CIRCLE_SOFTNESS, 0f);

        CleanupChangeAnimation(isPurified);
    }

    private void CleanupChangeAnimation(bool isPurified)
    {
        spriteRenderer.sprite = isPurified ? purifiedSprite : pollutedSprite;
        material.SetTexture(
            ShaderProps.CHANGE_TARGET_TEXTURE,
            isPurified ? pollutedSprite.texture : purifiedSprite.texture
        );
        material.SetFloat(ShaderProps.CIRCLE_RADIUS, 0f);
        material.SetFloat(ShaderProps.CIRCLE_SOFTNESS, 0f);
        changeAnimationCoroutine = null;
    }

    private IEnumerator FlipAnimationCoroutine(bool isShown, float delay, Action onComplete)
    {
        CleanupFlipAnimation(!isShown);
        yield return new WaitForSeconds(delay * flipDelayMultiplier);

        var coroutines = new List<Coroutine>
        {
            StartCoroutine(FlipAlphaCoroutine(isShown)),
            StartCoroutine(FlipRotateCoroutine()),
        };

        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        onComplete?.Invoke();
    }

    private IEnumerator FlipRotateCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / flipDuration);
            float angle = Mathf.Lerp(180f, 0f, t);
            transform.rotation = Quaternion.AngleAxis(angle, FlipAxis);
            yield return null;
        }
    }

    private IEnumerator FlipAlphaCoroutine(bool isShown)
    {
        float duration = flipDuration / 2;
        float startAlpha = isShown ? 0f : 1f;
        float endAlpha = isShown ? 1f : 0f;
        spriteRenderer.SetAlpha(startAlpha);

        if (isShown)
        {
            yield return new WaitForSeconds(duration);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            spriteRenderer.SetAlpha(Mathf.Lerp(startAlpha, endAlpha, t));
            yield return null;
        }

        spriteRenderer.SetAlpha(endAlpha);
    }

    private void CleanupFlipAnimation(bool isShown)
    {
        spriteRenderer.SetAlpha(isShown ? 1f : 0f);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
