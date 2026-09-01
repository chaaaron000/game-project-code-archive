using UnityEngine;

namespace Nostal.Util
{
    public static class LayerUtility
    {
        /// <summary>
        /// LayerMask에서 첫 인덱스를 반환합니다.
        /// </summary>
        /// <param name="layerMask">인덱스를 추출하고 싶은 LayerMask</param>
        /// <returns>발견한 레이어의 인덱스를 반환합니다. 여러 레이어가 중복으로 선택되어 있으면 첫 레이어의 인덱스만 반환합니다. 선택된 레이어가 없으면 -1를 반환합니다.</returns>
        public static int GetFirstLayerIndex(LayerMask layerMask)
        {
            int bitMask = layerMask.value;
            
            for (int i = 0; i < 32; i++)
            {
                if ((bitMask & (1 << i)) != 0)
                {
                    // 처음 발견한 레이어 인덱스를 바로 반환
                    return i;
                }
            }

            // 실패 시 -1 반환
            return -1;
        }

        public static void SetLayerAllChildren(Transform root, int layer)
        {
            Transform[] children = root.GetComponentsInChildren<Transform>(includeInactive: true);

            foreach (Transform child in children)
            {
                child.gameObject.layer = layer;
            }
        }
    }
}