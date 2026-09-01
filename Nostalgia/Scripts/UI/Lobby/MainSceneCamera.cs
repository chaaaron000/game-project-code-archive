using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneCamera : MonoBehaviour
{
    public float speed = 2.0f; // 카메라가 움직이는 속도
    public float heightRange = 5.0f; // 카메라가 위아래로 움직이는 범위
    private Vector3 initialPosition; // 카메라의 초기 위치

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * speed) * heightRange;
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
