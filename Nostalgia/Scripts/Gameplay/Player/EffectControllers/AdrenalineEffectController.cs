using System.Collections;
using Fusion;
using Item;
using Nostal.Util;
using UnityEngine;
using UnityEngine.VFX;

public class AdrenalineEffectController : EffectController
{
    [SerializeField] private AdrenalineItemSO m_adrenalineItemSO;
    [SerializeField] private VisualEffect m_adrenalineVFX;

    private Coroutine m_playEffectCoroutine;
    
    public override void PlayEffect()
    {
        if (m_playEffectCoroutine != null)
        {
            StopCoroutine(m_playEffectCoroutine);
        }
        
        SendVFXEventRPC("OnStop");
        m_playEffectCoroutine = StartCoroutine(PlayEffectCoroutine());
    }

    private IEnumerator PlayEffectCoroutine()
    {
        SendVFXEventRPC("OnPlay"); 
        yield return new WaitForSeconds(m_adrenalineItemSO.BoostDuration);
        SendVFXEventRPC("OnStop");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void SendVFXEventRPC(string eventName)
    {
        m_adrenalineVFX.SendEvent(eventName);
    }
}