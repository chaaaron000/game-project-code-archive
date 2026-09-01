using Fusion;
using Nostal.Util;
using UnityEngine;

public class TeleportEffectController : EffectController
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private ParticleSystem m_splashEffect;
    [SerializeField] private ParticleSystem m_portalEffect;

    public override void PlayEffect()
    {
        m_animator.SetTrigger("play");
        m_splashEffect.Play();
        m_portalEffect.Play();
    }
}
