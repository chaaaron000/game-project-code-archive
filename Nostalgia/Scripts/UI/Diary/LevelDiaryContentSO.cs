using UnityEngine;
using UnityEngine.Localization;

namespace _Scripts.UI.Diary
{
    [CreateAssetMenu(fileName = "SO_DiaryContent_", menuName = "Scriptable Object/Diary Content", order = 0)]
    public class LevelDiaryContentSO : ScriptableObject
    {
        [SerializeField] private LocalizedString[] m_diaryContents;
        [SerializeField] private Sprite[] m_diaryPageSprites;

        public int GetContentsSize()
        {
            return m_diaryContents.Length;
        }

        public string GetDiaryContent(int index)
        {
            if (index < 0 || index >= m_diaryContents.Length)
            {
                return "";
            }

            return m_diaryContents[index].GetLocalizedString();
        }

        public Sprite GetDiaryPageSprite(int index)
        {
            if (index < 0 || index >= m_diaryPageSprites.Length)
            {
                return m_diaryPageSprites[0];
            }

            return m_diaryPageSprites[index];
        }
    }
}