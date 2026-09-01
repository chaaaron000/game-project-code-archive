using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "SO_TeleportItem", menuName = "Scriptable Object/Items/Teleport", order = 2)]
    public class TeleportItemSO : ConsumableItemSO
    {
        public override void Use(Player usingPlayer)
        {
            MobSpawner mobSpawner = FindObjectOfType<MobSpawner>();
            if (mobSpawner == null)
            {
                Debug.LogError(mobSpawner + "가 null입니다. Teleport Item을 사용할 수 없습니다.");
                return;
            }

            List<Transform> mobSpawnPositions = mobSpawner.MobPositions;
            Vector3 teleportTarget = default;
            
            // 플레이어가 한명 죽어있거나 탈출했으면
            // 가장 먼 Mob 스폰 포지션으로
            if (GameManager.Instance._deathFlag || GameManager.Instance._clearFlag)  
            {
				return;

				// TODO: 맵 탈출 버그 수정 때까지 봉인
				//Vector3 playerPosition = usingPlayer.gameObject.transform.position;
				//float maxDist = 0f;

				//foreach (Transform mobSpawnPosition in mobSpawnPositions)
				//{
				//    float dist = Vector3.Distance(playerPosition, mobSpawnPosition.position);
				//    // Debug.Log("dist = " + dist);
				//    if (dist > maxDist)
				//    {
				//        maxDist = dist;
				//        teleportTarget = mobSpawnPosition.position;
				//    }
				//}
            }
            else  // 플레이어가 살아있으면 플레이어 위치로
            {
                NetworkObject otherPlayerObject =
                    GameManager.Instance.GetOtherPlayer(usingPlayer.GetComponent<NetworkObject>());
                if (otherPlayerObject == null)
                {
                    Debug.LogError(otherPlayerObject + "가 null입니다. Teleport Item을 사용할 수 없습니다.");
                    return;
                }
                
                teleportTarget = otherPlayerObject.transform.position;

                if (otherPlayerObject.TryGetComponent(out Player otherPlayer) &&
                    otherPlayer.isHidden                                      &&
                    otherPlayer._cabinet != null                           &&
                    otherPlayer._cabinet.TryGetComponent(out Cabinet cabinet))
                {
                    teleportTarget = cabinet.cabinetOutPosition.position;
                }
            }

            teleportTarget.y += 0.2f;  // 바닥에 빠지는 것 예방 차원
            usingPlayer.Movement.TeleportByItemRPC(teleportTarget);
        }
    }
}
