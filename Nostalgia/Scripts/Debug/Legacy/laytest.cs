using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIRaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Q를 누르면 검사 실행
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            if (raycastResults.Count == 0)
            {
                Debug.Log("🔍 마우스 아래에 UI 없음");
            }
            else
            {
                Debug.Log("🔍 마우스 아래 UI 리스트:");
                foreach (var result in raycastResults)
                {
                    Debug.Log($"→ {result.gameObject.name} (Raycast Target 여부: {result.gameObject.GetComponent<UnityEngine.UI.Graphic>()?.raycastTarget})");
                }
            }
        }
    }
}
