using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class soundTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Alpha1은 숫자 1 키를 의미합니다.
        {
            SoundManager.Instance.SFX_loop_Play("running", this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) //
        {
            SoundManager.Instance.SFX_loop_Play("testSound", this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) // 
        {
            SoundManager.Instance.SFX_loop_Play("testSound2", this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) // 
        {
            SoundManager.Instance.SFX_loop_Stop("running", this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5)) //
        {
            SoundManager.Instance.SFX_loop_Stop("testSound", this.gameObject);
        }
    }
}
