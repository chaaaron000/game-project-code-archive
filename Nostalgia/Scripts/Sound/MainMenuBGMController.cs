using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBGMController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
