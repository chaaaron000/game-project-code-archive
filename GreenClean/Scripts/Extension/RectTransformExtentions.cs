using UnityEngine;

public static class RectTransformExtentions
{
    public static void SetAnchoredXPos(this RectTransform rectTransform, float xPos)
    {
        rectTransform.anchoredPosition = new Vector2(xPos, rectTransform.anchoredPosition.y);
    }

    public static void SetAnchoredYPos(this RectTransform rectTransform, float yPos)
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, yPos);
    }
}
