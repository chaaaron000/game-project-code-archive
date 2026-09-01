using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ChaseDaughterHint : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(GameManager.Instance.GetLocalPlayer() == GameManager.Instance.DaughterNetworkObject)
            {
                //딸이 들어오면 오디오 재생
                audioSource.Play();
            }
        }
    }
}
