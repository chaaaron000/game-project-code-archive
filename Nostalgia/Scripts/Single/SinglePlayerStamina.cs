using UnityEngine;

namespace Nostal.Single
{
    public class SinglePlayerStamina : MonoBehaviour
    {
        [field: SerializeField]
        public float m_currentStamina
        {
            private set;
            get;
        }

        private const float MAX_STAMINA = 100f;
        private const float STAMINA_REGEN_RATE = 20f;
        private const float STAMINA_REGEN_DELAY = 1.5f;

        /// <summary>
        /// 스테미나 재충전 딜레이를 위한 타이머 변수. 스타미나를 사용하면 STAMINA_REGEN_DELAY로 값이 할당된다. 0f보다 아래로 내려가면 다시 충전된다.
        /// </summary>
        private float m_regenDelayTimer;

        private void OnEnable()
        {
            m_currentStamina = MAX_STAMINA;
        }

        // Update is called once per frame
        private void Update()
        {
            // 스테미나가 꽉 찼으면 return
            if (m_currentStamina >= MAX_STAMINA)
            {
                return;
            }

            // 스테미나 타이머 작동
            if (m_regenDelayTimer > 0f)
            {
                m_regenDelayTimer -= Time.deltaTime;
                return;
            }

            m_currentStamina += STAMINA_REGEN_RATE * Time.deltaTime;
            m_currentStamina = Mathf.Min(m_currentStamina, MAX_STAMINA);
        }

        public float ConsumeStamina(float amount)
        {
            float preStamina = m_currentStamina;
            m_currentStamina -= amount;

            // 스테미너는 0 이상
            m_currentStamina = Mathf.Max(m_currentStamina, 0);

            m_regenDelayTimer = STAMINA_REGEN_DELAY;

            return preStamina - m_currentStamina;
        }
    }
}
