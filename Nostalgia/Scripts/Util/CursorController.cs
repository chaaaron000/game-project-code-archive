using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nostal.Util
{
    public static class CursorController
    {
        /// <summary>
        /// 마우스 커서의 활성화 여부 및 화면 중앙 고정 여부를 설정하는 메소드
        /// </summary>
        /// <param name="value">true: 커서 활성화 및 고정 해제 / false: 커서 비활성화 및 화면 중앙 고정</param>
        public static void SetEnableCursor(bool value)
        {
            if ((int)NostalgiaGameLevel.MainMenu == SceneManager.GetActiveScene().buildIndex)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }
            
            Cursor.lockState = (value ? CursorLockMode.None : CursorLockMode.Locked);
            Cursor.visible = value;
            // Debug.Log($"CursorController.SetEnableCursor: {value}");
        }
    }
}