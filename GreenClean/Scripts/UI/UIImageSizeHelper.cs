using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIImageSizeHelper : MonoBehaviour
{
    private RectTransform parentRect;
    private RectTransform rect;
    private Image image;
    private Vector2 lastParentSize;

    private Vector2 currentParentSize => parentRect.rect.size;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        parentRect = rect.parent as RectTransform;
        UpdateSize();
    }

    private void Update()
    {
        if (!parentRect)
        {
            return;
        }

        Vector2 currentSize = currentParentSize;
        if (currentSize != lastParentSize)
        {
            UpdateSize();
            lastParentSize = currentSize;
        }
    }

    private void UpdateSize()
    {
        if (!image.sprite || !parentRect)
        {
            return;
        }

        Vector2 parentSize = currentParentSize;
        float parentAspect = parentSize.x / parentSize.y;
        Rect spriteRect = image.sprite.rect;
        float imageAspect = spriteRect.width / spriteRect.height;

        Vector2 targetSize = Vector2.zero;
        if (parentAspect >= imageAspect)
        {
            // 화면이 가로로 넓음
            targetSize.x = parentSize.x;
            targetSize.y = parentSize.x / imageAspect;
        }
        else
        {
            // 화면이 세로로 더 긺
            targetSize.x = parentSize.y * imageAspect;
            targetSize.y = parentSize.y;
        }

        rect.sizeDelta = targetSize;
        rect.anchoredPosition = Vector2.zero;
    }
}
