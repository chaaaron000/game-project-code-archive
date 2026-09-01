using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nostal.Single
{
    public class Diary : Interactable
    {
        [Header("Diary")]
        [SerializeField]
        private Collider m_collider;

        [Header("Audio Source")]
        [SerializeField]
        private AudioSource m_pickupSFX;

        [SerializeField]
        private AudioSource m_alarmSFX;

        [Header("Mesh Renderer")]
        [SerializeField]
        private Renderer m_cover;

        [SerializeField]
        private Renderer m_pile;

        [SerializeField]
        private Renderer m_uvula;

        [SerializeField]
        private ParticleSystem m_butterflyParticle;

        private readonly static string DESOLVE_PROP = "_desolve";
        private readonly static float DESOLVE_DURATION = 3f;
        private readonly static float DESOLVE_START_VALUE = 0f;
        private readonly static float DESOLVE_END_VALUE = 1f;

        private void OnEnable()
        {
            SetMaterialFloat(DESOLVE_PROP, 0f);

            m_alarmSFX.loop = true;
            m_alarmSFX.Play();
        }

        public override void Interact(SinglePlayer player)
        {
            // Diary 처리 

            m_collider.enabled = false;
            m_butterflyParticle.Play();
            m_pickupSFX.Play();

            StartCoroutine(DesolveCoroutine());
        }

        private IEnumerator DesolveCoroutine()
        {
            float elapsedTime = 0f;
            while (elapsedTime < DESOLVE_DURATION)
            {
                elapsedTime += Time.deltaTime;
                float newValue = Mathf.Lerp(DESOLVE_START_VALUE, DESOLVE_END_VALUE, elapsedTime / DESOLVE_DURATION);
                SetMaterialFloat(DESOLVE_PROP, newValue);
                yield return null;
            }

            // 최종적으로 0으로 설정
            SetMaterialFloat(DESOLVE_PROP, DESOLVE_END_VALUE);
            Destroy(gameObject);
        }

        private void SetMaterialFloat(string name, float value)
        {
            m_cover.material.SetFloat(name, value);
            m_pile.material.SetFloat(name, value);
            m_uvula.material.SetFloat(name , value);
        }
    }
}

