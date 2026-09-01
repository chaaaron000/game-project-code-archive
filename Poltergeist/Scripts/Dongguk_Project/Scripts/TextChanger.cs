using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChanger : MonoBehaviour
{

    public Text textUI;
    public string[] textList = {
        " ",
        "\"탈출할 수 있을 줄 알았는데.\"",
        "\"그저 농락당한 거였어.\"",
        "\"젠장...\"",
        " ",
        "다음 날",
        "\"제발 저희 아들을 찾아주세요...\"",
        "\"최대한 노력해보겠습니다.\"",
        " ",
        "10일 후",
        "\"신이시여... 아들이 건강하기를...\"",
        " ",
        "20일 후",
        "\"신이시여...\"",
        " ",
        "1년 후",
        "\"......\"",
        " ",
        "10년 후",
        "어느 등산가가 쓰러지기 직전인 폐가가 발견한다.",
        "오랜 기간 사람이 살지 않아 보였지만,",
        "지하실 안에는 백골이 되어버린 시신이 있었다.",
        "- End -"
    };

    private int index = 0;
    private float timer = 0.0f;
    private float changeTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (index + 1 > textList.Length)
        {
            return;
        }
        
        timer += Time.deltaTime;
        if (timer >= changeTime)
        {
            textUI.text = textList[index];
            index += 1;
            timer = 0.0f;
        }
        
        
    }


}
