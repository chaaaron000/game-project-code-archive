using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Slider _staminaVisualSlider;
    
    private static readonly float _visualTrackingRate  = 50f;
    private static readonly float _visualTrackingDelay = 0.5f;
    
    private Coroutine _staminaVisualCoroutine = null;

    private float _maxStamina = 100f; // 스테미너 최대값 (게임 설정에 맞게 조정)
    public GameObject staminaPenel;
    public Image chaseImage;

    public void Start() {
        GameObject camera = GameObject.Find("UICamera");
        GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();
    }

    public void Show() {
        gameObject.GetComponent<Canvas>().enabled = true;
    }

    public void Hide() {
        gameObject.GetComponent<Canvas>().enabled = false;
    }

    public void ShowStaminaUI() {
        if (staminaPenel != null)
        {
            staminaPenel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("staminaPenel is null (probably destroyed)");
        }
    }

    public void HideStaminaUI() {
        if (staminaPenel != null)
        {
            staminaPenel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("staminaPenel is null (probably destroyed)");
        }
    }

    public void ShowChaseImage()
    {
        chaseImage.enabled = true;
    }
    public void HideChaseImage()
    {
        chaseImage.enabled = false;
    }

    public void SetStamina(float stamina)
    {
        if (this == null) return; // 이미 Destroy됐을 경우 대비

         //_staminaValue.text = $"{stamina:N1}";
        if (_staminaSlider != null )
            _staminaSlider.value = stamina;

        // 스테미너 UI 표시/숨김 처리
        if (stamina >= _maxStamina)
        {
            HideStaminaUI();  // 스테미너가 가득 차면 스테미너 UI 숨김
        }
        else
        {
            ShowStaminaUI();  // 스테미너가 줄어들면 스테미너 UI 표시
        }

        if (_staminaVisualCoroutine != null)
        {
            StopCoroutine(_staminaVisualCoroutine);
            _staminaVisualCoroutine = null;
        }

        if (this != null)
            _staminaVisualCoroutine = StartCoroutine(StaminaVisualTracking(stamina));
    }

    
    IEnumerator StaminaVisualTracking(float stamina)
    {
        //Debug.Log(stamina);
        
        // 스테미나 사용의 경우
        if (_staminaVisualSlider.value > stamina)
        {
            // 딜레이
            yield return new WaitForSeconds(_visualTrackingDelay);
            
            while (_staminaVisualSlider.value >= stamina)
            {
                _staminaVisualSlider.value -= _visualTrackingRate * Time.deltaTime;
                _staminaVisualSlider.value = Mathf.Max(_staminaVisualSlider.value, stamina);
                yield return null;
            }
        }
        // 스테미나 회복의 경우
        else
        {
            _staminaVisualSlider.value = stamina;
        }
    }
}
