using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Amulet : NetworkBehaviour
{
    public MeshRenderer meshRenderer;
    // Start is called before the first frame update
    public override void Spawned()
    {
        StartCoroutine(LightOnToFather());
    }

    public IEnumerator LightOnToFather() {
        while(GameManager.Instance == null) {
            yield return null;
        }
        while(GameManager.Instance.GetLocalPlayer() == null || GameManager.Instance.FatherNetworkObject == null) {
            yield return null;
        }
        // Debug.Log("LocalPlayer: " + GameManager.Instance.GetLocalPlayer() + " FatherNetworkObject: " + GameManager.Instance.FatherNetworkObject);
        if(GameManager.Instance.GetLocalPlayer() == GameManager.Instance.FatherNetworkObject) {
            // Debug.Log("아빠 부적 조명 켜기");
            meshRenderer.enabled = true;
        }
    }
}
