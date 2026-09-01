using System.Collections;
using Item;
using Nostal;
using Nostal.Settings;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeController : MonoBehaviour
{
    [Header("Graphics Settings Scriptable Object")]
    [SerializeField] private GraphicsSettingsSO m_graphicsSettingsSO;
    
    [Header("Global Volume Controller")]
    [SerializeField] private Volume m_volume;
    [SerializeField] private VolumeProfile m_volumeProfile;

    private void OnEnable()
    {
        m_graphicsSettingsSO.OnBrightnessChanged += ChangeGammaOffsetValue;
        GameplayEventManager.StoperItemStarted += ActivateStoperEffect;
        GameplayEventManager.StoperItemEnded += DeactivateStoperEffect;
    }

    private void Start()
    {
        StartCoroutine(InitialBrightness());
    }

    private void OnDestroy()
    {
        m_graphicsSettingsSO.OnBrightnessChanged -= ChangeGammaOffsetValue;
        GameplayEventManager.StoperItemStarted -= ActivateStoperEffect;
        GameplayEventManager.StoperItemEnded -= DeactivateStoperEffect;
    }

    /// <summary>
    /// 밝기 조절을 위한 감마 오프셋 조절 메소드
    /// </summary>
    /// <param name="offset">오프셋 값</param>
    public void ChangeGammaOffsetValue(float offset)
    {
        if (!m_volumeProfile.TryGet(out LiftGammaGain lgg))
        {
            return;
        }
        
        Vector4 gammaValue = lgg.gamma.value;
        gammaValue.w = offset;
        // Vector4 newGammaValue = new Vector4(gammaValue.x, gammaValue.y, gammaValue.z, offset);
        
        lgg.gamma.value = gammaValue;
        
        m_volume.enabled = false;
        m_volume.enabled = true;
    }

    private IEnumerator InitialBrightness()
    {
        yield return null;
        ChangeGammaOffsetValue(m_graphicsSettingsSO.Brightness);
        
        // 왠지는 모르겠지만 Volume 컴포넌트를 껐다 키면 값 변경이 정상적으로 됨(?)
        m_volume.enabled = false;
        m_volume.enabled = true;
    }

    private void ActivateStoperEffect()
    {
        if (m_volumeProfile.TryGet(out ColorAdjustments colorAdjustments))
        {
            colorAdjustments.colorFilter.overrideState = true;
        }
    }

    private void DeactivateStoperEffect()
    {
        if (m_volumeProfile.TryGet(out ColorAdjustments colorAdjustments))
        {
            colorAdjustments.colorFilter.overrideState = false;
        }
    }
}
