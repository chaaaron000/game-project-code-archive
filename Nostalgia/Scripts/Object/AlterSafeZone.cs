using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterSafeZone : MonoBehaviour
{
    public void OnTriggerEnter(Collider other) {
        Player player;
        other.TryGetComponent<Player>(out player);

        if(player != null)
            player.EnterSafeZoneRpc();
    }

    public void OnTriggerExit(Collider other) {
        Player player;
        other.TryGetComponent<Player>(out player);

        if(player != null)
            player.ExitSafeZoneRpc();
    }
}
