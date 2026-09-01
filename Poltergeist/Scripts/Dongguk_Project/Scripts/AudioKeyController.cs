using UnityEngine;

public class AudioKeyController : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    
    public void AudioStart(){
        audioSource.enabled =true;
    }
}
