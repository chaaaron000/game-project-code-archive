using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionScript : MonoBehaviour
{
    private GameObject line;
    private GameObject line2;

    private void Start()
    {
        line = transform.Find("Line").gameObject;
        line2 = transform.Find("Line2").gameObject;

        line.SetActive(false);
        line2.SetActive(false);
        // 3초 후 wasd 오브젝트 활성화
        Invoke("EnableLine", 1f);
    }

    private void EnableLine()
    {
        // wasd 오브젝트 활성화
        line.SetActive(true);

        // 3초 후 wasd 오브젝트 비활성화
        Invoke("DisableLine", 3f);
    }

    private void DisableLine()
    {
        // wasd 오브젝트 비활성화
        line.SetActive(false);

       
        // 2초 후 wasd 오브젝트 다시 활성화
        Invoke("EnableLine2", 1f);
    }

    private void EnableLine2()
    {
        // wasd 오브젝트 활성화
        line2.SetActive(true);

        // 3초 후 wasd 오브젝트 비활성화
        Invoke("DisableLine2", 3f);
    }

    private void DisableLine2(){
        line2.SetActive(false);
    }
}
