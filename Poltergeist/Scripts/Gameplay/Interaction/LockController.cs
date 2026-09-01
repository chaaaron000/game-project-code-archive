using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LockController : MonoBehaviour
{
    public GameObject lockUI;
    public GameObject explainUI;
    public TMP_InputField passwordInput;
    public string correctPassword;
    public GameObject lockObject;
    public AudioController audioController;
    private bool isLocked = true;

    void Start()
    {
        // 초기화
        audioController = GetComponentInParent<AudioController>();
        lockUI.SetActive(false);
        explainUI.SetActive(false);
        passwordInput.gameObject.SetActive(false);
        passwordInput.onEndEdit.AddListener(OnSubmitButtonClicked);
        // Enter 키를 눌렀을 때 OnSubmitButtonClicked() 함수 호출
    }

    void Update()
    {
        // ESC 키를 누르면 자물쇠 UI를 닫고 게임을 재개한다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isLocked)
            {
                OnCancelButtonClicked();
            }
        }
    }

    void ObjectClicked()
    {
        if (isLocked)
        {
            ShowLockUI();
        }
    }

    void ShowLockUI()
    {
        isLocked = true;
        lockUI.SetActive(true);
        explainUI.SetActive(true);
        Time.timeScale = 0f;
        passwordInput.gameObject.SetActive(true);
        passwordInput.ActivateInputField();
        //passwordInput.text = "";
        Debug.Log("--------------------");
        passwordInput.onSubmit.AddListener(delegate { CheckPassword(); });
    }

    void CheckPassword()
    {
        string password = passwordInput.text;
        Debug.Log(gameObject.name);
        Debug.Log("패스워드 lock:" + password);
        if (password == correctPassword)
        {
            isLocked = false;
            lockUI.SetActive(false);
            explainUI.SetActive(false);
            passwordInput.gameObject.SetActive(false);
            Time.timeScale = 1f;

            // Unlock 사운드 재생

            password = "";
            passwordInput.text = "";
            audioController.AudioPlay();
            Destroy(lockObject);
        }
        else
        {
            passwordInput.text = "";
            passwordInput.ActivateInputField();
        }
    }

    private void OnSubmitButtonClicked(string arg0)
    {
        Debug.Log("LockOnSubmitBUttonClicked");
        if (passwordInput.gameObject.activeSelf)
        {
            Debug.Log("Lock OnSubmitButtonClicked() ");
            CheckPassword();
        }
    }

    private void OnCancelButtonClicked()
    {
        lockUI.SetActive(false);
        explainUI.SetActive(false);
        passwordInput.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
