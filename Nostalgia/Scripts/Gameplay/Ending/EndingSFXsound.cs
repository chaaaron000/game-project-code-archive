using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSFXsound : MonoBehaviour
{
    public void SoundPlay(){
        SoundManager.Instance.SFX_Play("endingFootStep", this.gameObject);
    }
}
