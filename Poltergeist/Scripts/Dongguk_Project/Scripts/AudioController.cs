using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void AudioPlay(){
        audioSource.enabled =true;
        //audioSource.Play();
    }
        
    
    
}
