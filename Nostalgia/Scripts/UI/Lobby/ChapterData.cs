using System.Collections;
using System.Collections.Generic;
using Nostal.Steam;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class ChapterData {
    public LocalizedString title;
    public LocalizedString stage;
    public Sprite image;
    public bool isLocked;
}
