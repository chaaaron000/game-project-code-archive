using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleSound : MonoBehaviour
{
    public void CandleSoundOn(){
        SoundManager.Instance.SFX_Play("candleOn");
    }
}
