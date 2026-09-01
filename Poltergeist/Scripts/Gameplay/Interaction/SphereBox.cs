using UnityEngine;
using System.Collections;

public class SphereBox : MonoBehaviour
{
    public float rotationSpeed = 30f; // 회전 속도
    public Transform pivotPoint; // 회전축 위치
    public GameObject lockObject; // 자물쇠 오브젝트

    private bool spinState = false; // 회전 상태

    void Update()
    {
        // 자물쇠 오브젝트가 파괴되면 회전 시작
        if (lockObject == null && !spinState)
        {
            StartCoroutine(SpinObject());
            spinState = true;
        }
    }

    IEnumerator SpinObject()
    {
        float angleRemaining = 97f;
        while (angleRemaining > 0f)
        {
            float angleToRotate = rotationSpeed * Time.deltaTime;
            angleToRotate = Mathf.Min(angleToRotate, angleRemaining);
            transform.RotateAround(pivotPoint.position, Vector3.back, angleToRotate);
            angleRemaining -= angleToRotate;
            yield return null;
        }
    }
}

