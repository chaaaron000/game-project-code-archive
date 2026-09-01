using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public CanvasGroup inventoryCanvasGroup;
    public CanvasGroup backgroundCanvasGroup;
    public float fadeTime = 0.5f;

    private bool isShowing = false;
    private float currentFadeTime = 0f;

    private void Start()
    {
        // 시작할 때는 InventoryUI와 BackgroundPanel을 숨긴다.
        inventoryCanvasGroup.alpha = 0f;
        backgroundCanvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isShowing = !isShowing;
            currentFadeTime = 0f;
        }

        if (isShowing && currentFadeTime < fadeTime)
        {
            float t = currentFadeTime / fadeTime;
            inventoryCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            backgroundCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            currentFadeTime += Time.deltaTime;
        }
        else if (!isShowing && currentFadeTime < fadeTime)
        {
            float t = currentFadeTime / fadeTime;
            inventoryCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            backgroundCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            currentFadeTime += Time.deltaTime;
        }
        else if (!isShowing)
        {
            inventoryCanvasGroup.alpha = 0f;
            backgroundCanvasGroup.alpha = 0f;
        }
    }
}


//inventory UI 만
/*using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public CanvasGroup inventoryCanvasGroup;
    public float fadeTime = 0.5f;

    private bool isShowing = false;
    private float currentFadeTime = 0f;

    private void Start()
    {
        // 시작할 때는 InventoryUI를 숨긴다.
        inventoryCanvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isShowing = !isShowing;
            currentFadeTime = 0f;
        }

        if (isShowing && currentFadeTime < fadeTime)
        {
            float t = currentFadeTime / fadeTime;
            inventoryCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            currentFadeTime += Time.deltaTime;
        }
        else if (!isShowing && currentFadeTime < fadeTime)
        {
            float t = currentFadeTime / fadeTime;
            inventoryCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            currentFadeTime += Time.deltaTime;
        }
        else if (!isShowing)
        {
            inventoryCanvasGroup.alpha = 0f;
        }
    }
}*/




