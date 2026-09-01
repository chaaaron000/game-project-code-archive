using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct DiaryPageInfo
{
    public string PageName;

    public Sprite PageImage;

    //현재로썬 개발자가 순서를 보기 위함
    public int PageNum;
}

[CreateAssetMenu(fileName = "DiaryData", menuName = "Scriptable Object/DiaryData")]
public class DiaryData : ScriptableObject
{
    [SerializeField] private DiaryPageInfo DiaryPageInfo = new DiaryPageInfo();

    public Sprite LoadImage() {
        return DiaryPageInfo.PageImage;
    }
}
