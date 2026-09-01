using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ChaseFatherHint : NetworkBehaviour
{
    [SerializeField] public MeshRenderer meshRenderer;

    public override void Spawned()
    {
        if(GameManager.Instance.IsLocalPlayerFather) {
            // Debug.Log("아빠 힌트 조명 켜기");
            meshRenderer.enabled = true;
        }
        else {
            // Debug.Log("아빠 힌트 조명 끄기");
            meshRenderer.enabled = false;
        }
    }
}
