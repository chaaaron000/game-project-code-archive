using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CharacterSelectUIController : MonoBehaviour
{
    public TMP_Text FatherPlayerNameTmpText;
    public TMP_Text DaughterPlayerNameTmpText;
    public TMP_Text SessionNameText;
    
    public Button CharacterSwapButton;
    public Button ReadyButton;
    public Canvas canvas;
    public CanvasGroup canvasGroup;

    [SerializeField] private Button m_lobbySettingsButton;

    public float fadeDuration = 0.5f;

    void Awake()
    {    
        FatherPlayerNameTmpText.text   = "";
        DaughterPlayerNameTmpText.text = "";
    }

    private void OnEnable()
    {
        m_lobbySettingsButton.onClick.AddListener(ShowSettingsUI);
    }

    private void Start()
    {
        UIManager.Instance.CharacterSelectUIPrefab = gameObject;
        UIManager.Instance.CharacterSelectUIController = this;
    }

    private void OnDisable()
    {
        m_lobbySettingsButton.onClick.RemoveAllListeners();
    }

    public void AddButtonListener(bool isMaster)
    {
        ReadyButton.onClick.AddListener(() => SelectCharacterManager.Instance.ReadyRpc());
        CharacterSwapButton.onClick.AddListener(SelectCharacterManager.Instance.SwapCharacterRpc);
    }

    public void ShowCanvas(bool isShow)
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas is null");
            return;
        }
        
        canvas.enabled = isShow;
    }

    public IEnumerator FadeCanvas(bool isShow)
    {
        float startAlpha = canvasGroup.alpha;
        float endAlpha = isShow ? 1f : 0f;
        float elapsedTime = 0f;

        yield return new WaitForSeconds(1.0f);

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        // 완전히 사라졌을 경우 Canvas 비활성화
        if (!isShow)
        {
            canvas.enabled = false;
        }
    }

    public void SetPlayerName()
    {
        FatherPlayerNameTmpText.text   = GameManager.Instance.FatherPlayerName;
        DaughterPlayerNameTmpText.text = GameManager.Instance.DaughterPlayerName;
    }

    public void SetSessionName(string SessionName)
    {
        SessionNameText.text = SessionName;
    }

    public void Destroy()
    {
        RemoveAllListeners();
        Destroy(gameObject);
    }
    
    public void RemoveAllListeners()
    {
        CharacterSwapButton.onClick.RemoveAllListeners();
        ReadyButton.onClick.RemoveAllListeners();
    }

    private void ShowSettingsUI()
    {
        UIManager.Instance.Push("SettingUIController");
    }
}
