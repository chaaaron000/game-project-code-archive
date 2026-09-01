using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaycastTest : MonoBehaviour
{
    private EventSystem eventSystem;
    private PointerEventData pointerData;

    private List<RaycastResult> uiResults = new List<RaycastResult>();
    // 씬 내 모든 GraphicRaycaster
    private List<GraphicRaycaster> raycasters = new List<GraphicRaycaster>();

    private GameObject m_previousRayResult; 

    private string currentHitName = "None";

    private void Awake()
    {
        eventSystem = EventSystem.current;
        // 씬에 있는 모든 GraphicRaycaster 컴포넌트를 찾아 저장
        raycasters.AddRange(FindObjectsOfType<GraphicRaycaster>());
    }

    private void Update()
    {
        pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        // 모든 Raycaster 순회하며 결과 집계
        bool hitFound = false;
        foreach (var rc in raycasters)
        {
            uiResults.Clear();
            rc.Raycast(pointerData, uiResults);
            if (uiResults.Count > 0)
            {
                currentHitName = $"[UI:{rc.gameObject.name}] {uiResults[0].gameObject.name}";
                hitFound = true;

                if (m_previousRayResult != uiResults[0].gameObject)
                {
                    Debug.Log(currentHitName, uiResults[0].gameObject);
                    m_previousRayResult = uiResults[0].gameObject;
                }

                break;
            }
        }

        if (hitFound)
        {
            return;
        }

        // 3D Raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentHitName = "[3D] " + hit.collider.gameObject.name;
            return;
        }

        currentHitName = "None";
    }
}
