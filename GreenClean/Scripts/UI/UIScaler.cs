using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class UIScaler : MonoBehaviour
{
    [Header("RectTransform")]
    [SerializeField]
    private RectTransform targetRect;

    [SerializeField]
    private RectTransform criteriaRect;

    [Header("Scaler Settings")]
    [SerializeField]
    private Vector2 referenceSize = new(512f, 512f);

    [SerializeField]
    private float minScale = 0f;

    [SerializeField]
    private float maxScale = 100f;

    [SerializeField]
    private bool setTargetSizeToReference = true;

    private bool HasChanged
    {
        get
        {
            if (criteriaRect == null)
            {
                return true;
            }

            return lastCriteriaSize != criteriaRect.rect.size || lastReferenceSize != referenceSize;
        }
    }

    private bool CheckRectTransforms
    {
        get
        {
            if (targetRect == null)
            {
                targetRect = transform as RectTransform;
            }

            if (criteriaRect == null)
            {
                criteriaRect = transform.parent as RectTransform;
            }

            if (targetRect == null || criteriaRect == null)
            {
                DebugConsole.LogWarning("[UIScaler] targetRect or criteriaRect is null", this);
                return false;
            }

            return true;
        }
    }

    private Vector3 CalculatedScale
    {
        get
        {
            Vector2 criteriaSize = criteriaRect.rect.size;
            float scaleX = criteriaSize.x / this.referenceSize.x;
            float scaleY = criteriaSize.y / this.referenceSize.y;

            return Vector3.one * ClampScale(Mathf.Min(scaleX, scaleY));
        }
    }

    private Vector2 lastCriteriaSize;
    private Vector2 lastReferenceSize;

    private void OnEnable()
    {
        Apply();
    }

    private void OnRectTransformDimensionsChange()
    {
        Apply();
    }

    private void Update()
    {
        if (HasChanged)
        {
            Apply();
        }
    }

    public void Apply()
    {
        if (!CheckRectTransforms)
        {
            return;
        }

        if (referenceSize.x <= 0f || referenceSize.y <= 0f)
        {
            // DebugConsole.LogError("[UIScaler] referenceSize is invalid", this);
            return;
        }

        Vector2 criteriaSize = criteriaRect.rect.size;

        if (criteriaSize.x <= 0f || criteriaSize.y <= 0f)
        {
            // DebugConsole.LogError("[UIScaler] criteriaSize is invalid", this);
            return;
        }

        if (setTargetSizeToReference)
        {
            targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, referenceSize.x);
            targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, referenceSize.y);
        }

        targetRect.localScale = CalculatedScale;
        CacheState(criteriaSize);
    }

    private float ClampScale(float scale)
    {
        return Mathf.Clamp(scale, minScale, maxScale);
    }

    private void CacheState(Vector2 criteriaSize)
    {
        lastCriteriaSize = criteriaSize;
        lastReferenceSize = referenceSize;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        if (targetRect == null)
        {
            targetRect = transform as RectTransform;
        }

        if (criteriaRect == null)
        {
            criteriaRect = transform.parent as RectTransform;
        }
    }

    private void OnValidate()
    {
        minScale = Mathf.Max(0f, minScale);
        maxScale = Mathf.Max(minScale, maxScale);
        Apply();
    }
#endif
}
