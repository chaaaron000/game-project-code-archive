using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class MobStateBehaviour : StateBehaviour
{
    [Header("AI")] 
    [SerializeField] protected BaseMob m_mobAI;
    
    [FormerlySerializedAs("animator")]
    [Header("Animator")]
    [SerializeField] private Animator m_animator;
    
    // 타입 캐스팅된 몹에 접근할 수 있도록 프로퍼티 제공
    protected T GetMob<T>() where T : BaseMob
    {
        return m_mobAI as T;
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SetAnimatorTriggerRpc(string triggerName)
    {
        m_animator.SetTrigger(triggerName);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SetAnimatorIntRpc(string intName, int value)
    {
        m_animator.SetInteger(intName, value);
    }
}