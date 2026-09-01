using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nostal.Single
{
    public class SinglePlayer : MonoBehaviour
    {
        public bool m_bIsHidding;

        [field: SerializeField]
        public SinglePlayerMovement Movement { private set; get; }

        [field: SerializeField]
        public SinglePlayerRotation Rotation { private set; get; }

        private void OnEnable()
        {
            m_bIsHidding = false;
        }
    }
}
