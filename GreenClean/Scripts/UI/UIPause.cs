using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup pauseCanvas;

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Button titleButton;

    [Header("Blur")]
    [Header("References")]
    [SerializeField]
    private Camera targetCamera;

    [SerializeField]
    private RawImage blurImage;

    [SerializeField]
    private Material blurMaterial;

    [Header("Blur Settings")]
    [SerializeField, Range(1, 8)]
    private int downSample = 4;

    [SerializeField, Range(1, 10)]
    private int blurStrength = 2;

    [SerializeField, Range(0.5f, 5f)]
    private float blurSize = 1.5f;

    private RenderTexture captureRt;
    private RenderTexture tempRt1;
    private RenderTexture tempRt2;

    private int lastScreenWidth;
    private int lastScreenHeight;

    private static readonly int BlurSizeId = Shader.PropertyToID("_BlurSize");

    private void Start()
    {
        restartButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            GameSceneManager.Instance.ChangeScene(SceneType.GAME);
        });

        titleButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            GameSceneManager.Instance.ChangeScene(SceneType.TITLE);
        });

        Hide();
    }

    public void SetActiveUI(bool isPaused)
    {
        if (isPaused)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        ShowBlur();

        pauseCanvas.alpha = 1f;
        pauseCanvas.blocksRaycasts = true;
        pauseCanvas.interactable = true;
    }

    private void Hide()
    {
        pauseCanvas.alpha = 0f;
        pauseCanvas.blocksRaycasts = false;
        pauseCanvas.interactable = false;
    }

    private void ShowBlur()
    {
        EnsureRenderTextures();
        CaptureScreen();
        ApplyBlur();
        blurImage.texture = tempRt2;
    }

    private void EnsureRenderTextures()
    {
        if (
            captureRt == null
            || lastScreenWidth != Screen.width
            || lastScreenHeight != Screen.height
        )
        {
            ReleaseRenderTextures();
            CreateRenderTextures();
        }
    }

    private void ReleaseRenderTextures()
    {
        ReleaseRt(captureRt);
        ReleaseRt(tempRt1);
        ReleaseRt(tempRt2);

        captureRt = null;
        tempRt1 = null;
        tempRt2 = null;
    }

    private void CreateRenderTextures()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        int width = Mathf.Max(1, Screen.width / downSample);
        int height = Mathf.Max(1, Screen.height / downSample);

        captureRt = CreateRt(width, height);
        tempRt1 = CreateRt(width, height, 0);
        tempRt2 = CreateRt(width, height, 0);
    }

    private static void ReleaseRt(RenderTexture rt)
    {
        if (rt == null)
        {
            return;
        }

        rt.Release();
        Destroy(rt);
    }

    private static RenderTexture CreateRt(int width, int height, int depth = 24)
    {
        RenderTexture rt = new RenderTexture(width, height, depth, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            useMipMap = false,
            autoGenerateMips = false,
        };

        rt.Create();
        return rt;
    }

    private void CaptureScreen()
    {
        ClearRenderTexture(captureRt);

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        RenderTexture previousTarget = targetCamera.targetTexture;

        targetCamera.targetTexture = captureRt;
        targetCamera.Render();
        targetCamera.targetTexture = previousTarget;
    }

    private static void ClearRenderTexture(RenderTexture rt)
    {
        RenderTexture previous = RenderTexture.active;

        RenderTexture.active = rt;
        GL.Clear(true, true, Color.clear);

        RenderTexture.active = previous;
    }

    private void ApplyBlur()
    {
        blurMaterial.SetFloat(BlurSizeId, blurSize);

        // Pass 0: Horizontal Blur
        Graphics.Blit(captureRt, tempRt1, blurMaterial, 0);

        // Pass 1: Vertical Blur
        Graphics.Blit(tempRt1, tempRt2, blurMaterial, 1);

        for (int i = 1; i < blurStrength; i++)
        {
            Graphics.Blit(tempRt2, tempRt1, blurMaterial, 0);
            Graphics.Blit(tempRt1, tempRt2, blurMaterial, 1);
        }
    }
}
