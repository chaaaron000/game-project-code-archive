using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using System.Collections;

public class WatchController : MonoBehaviour
{
    
    public GameObject explainUI;
    public TMP_InputField passwordInput;
    public string correctPassword;
    
    public TextMeshPro timeText;
    public TextMeshPro bgs; // 수정된 부분
    public GameObject key;
    
    private AudioSource audioSoure;

    private bool isLocked = true;

    void Start()
    {
    // 초기화
        audioSoure = GetComponent<AudioSource>();
        key.SetActive(false);
        explainUI.SetActive(false);
        passwordInput.gameObject.SetActive(false);
        bgs.gameObject.SetActive(true);
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

    void WatchClicked()
    {
        if (isLocked)
        {
            Debug.Log("WatchClicked");
            ShowLockUI();
        }
    }

    void ShowLockUI()
    {
        Debug.Log("WatchShowLockUI");
        explainUI.SetActive(true);
        Time.timeScale = 0f;
        passwordInput.gameObject.SetActive(true);
        passwordInput.ActivateInputField();
        passwordInput.text = "";
        passwordInput.onSubmit.AddListener(delegate { this.OnSubmitWatchClicked(); });
    }

    void CheckPassword()
    {
        string password = passwordInput.text;
        
        Debug.Log("패스워드 clock:" + password);
        if (password == correctPassword)
        {   
            Debug.Log("this is clock CHekckPAss");
            isLocked = false;
        
            explainUI.SetActive(false);
            passwordInput.gameObject.SetActive(false);
            Time.timeScale = 1f;


            // 정답 입력 후 time 텍스트 업데이트
        
            timeText.text = password;
            
            audioSoure.Play();
            bgs.text = "Check The table";
            key.SetActive(true);
        }
        else
        {
            Debug.Log("clock checkpass if false");
            passwordInput.text = "";
            
            Debug.Log("check");
        }
    }


    private void OnSubmitWatchClicked()
    {
        Debug.Log("WatchOnSubmitBUttonClicked");
        if (passwordInput.gameObject.activeSelf)
        {
            Debug.Log("Watch OnSubmitButtonClicked() ");
            CheckPassword();
        }
    }

    private void OnCancelButtonClicked()
    {
        
        explainUI.SetActive(false);
        passwordInput.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

}