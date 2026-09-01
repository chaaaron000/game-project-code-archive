using Nostal.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Nostal.Single
{
    [System.Serializable]
    public struct TileTransformCandidate
    {
        public Transform transform;
        public bool bCanRandomRotate;
    }

    public class TileShuffler : MonoBehaviour
    {
        [Header("Transform References")]
        [SerializeField]
        private List<TileTransformCandidate> m_fourWayTransforms = new List<TileTransformCandidate>();

        [SerializeField]
        private List<TileTransformCandidate> m_threeWayTransforms = new List<TileTransformCandidate>();

        [SerializeField]
        private List<TileTransformCandidate> m_twoWayTransforms = new List<TileTransformCandidate>();

        [Header("Target Tiles")]
        [SerializeField]
        private List<Transform> m_fourWayTiles = new List<Transform>();

        [SerializeField]
        private List<Transform> m_threeWayTiles = new List<Transform>();

        [SerializeField]
        private List<Transform> m_twoWayTiles = new List<Transform>();

        private List<float> m_randomRotation = new List<float> { 0f, 90f, 180f, 270f };
        
        private void Start()
        {
            ShuffleTile(m_fourWayTransforms, m_fourWayTiles);
            ShuffleTile(m_threeWayTransforms, m_threeWayTiles);
            ShuffleTile(m_twoWayTransforms, m_twoWayTiles);
        }

        private void ShuffleTile(List<TileTransformCandidate> candidates, List<Transform> shuffleTarget)
        {
            if (candidates.Count != shuffleTarget.Count)
            {
                Debug.LogError($"타일 위치 후보 리스트({candidates.Count} 개)와 타겟 타일 리스트({shuffleTarget.Count} 개)의 사이즈가 동일하지 않습니다.", this);
                return;
            }

            candidates.KnuthShuffle();

			for (int i = 0; i < shuffleTarget.Count; i++)
			{
                Quaternion rotation = candidates[i].bCanRandomRotate ? GetRandomRotate() : candidates[i].transform.rotation;
                shuffleTarget[i].rotation = rotation;
                shuffleTarget[i].position = candidates[i].transform.position;
			}
		}

        private Quaternion GetRandomRotate()
        {
            int index = UnityEngine.Random.Range(0, m_randomRotation.Count);
            Quaternion result = Quaternion.Euler(0f, m_randomRotation[index], 0f);
            return result;
        }
    }
}

