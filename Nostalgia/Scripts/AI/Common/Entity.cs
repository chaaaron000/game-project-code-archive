using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using ExitGames.Client.Photon.StructWrapping;

public class Entity : NetworkBehaviour//, IPlayerJoined
{
    public enum mobID {
        angry = 0,
        expressionless = 1,
        sad = 2,
        smile = 3,
        female = 4
    };
    
    public void Attack(Player targetPlayer, int damage, int mobID)
    {
        //몹이 플레이어를 바라보며 공격하도록 함 (angry 제외)
        if(mobID != 0)
            gameObject.transform.LookAt(targetPlayer.gameObject.transform);

        targetPlayer.DealDamageRpc(damage, mobID);
    }
}
