using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlay : MonoBehaviour
{
    public AudioSource audioSource;


    public void soundPlay()
    {
        audioSource.Play();
    }
}
