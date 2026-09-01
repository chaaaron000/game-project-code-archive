using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMonsterEyes2 : MonoBehaviour
{
    // 각 축별 최대/최소 회전 제한 값
    public Vector3 maxRotationLimit = new Vector3(45f, 45f, 45f);
    public Vector3 minRotationLimit = new Vector3(-45f, -45f, -45f);

    // 랜덤 회전 속도 범위
    public float randomSpeedRange = 20f;

    private Vector3 rotationSpeed; // 현재 회전 속도

    void Start()
    {
        // 초기 회전 속도를 랜덤하게 설정
        SetNewRandomSpeed();
    }

    void Update()
    {
        // 현재 회전 값을 가져오기 (로컬 기준)
        Vector3 currentRotation = transform.localEulerAngles;

        // 오일러 각도를 -180~180도로 변환
        currentRotation = NormalizeAngles(currentRotation);

        // 각 축별로 제한 검사 후 방향 전환
        for (int i = 0; i < 3; i++)
        {
            if (currentRotation[i] > maxRotationLimit[i] && rotationSpeed[i] > 0)
                rotationSpeed[i] = -Random.Range(5f, randomSpeedRange); // 방향 반전 (음의 방향)
            else if (currentRotation[i] < minRotationLimit[i] && rotationSpeed[i] < 0)
                rotationSpeed[i] = Random.Range(5f, randomSpeedRange); // 방향 반전 (양의 방향)
        }

        // 회전 적용
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // 랜덤하게 일정 시간마다 속도 갱신
        if (Random.Range(0f, 1f) < 0.01f) // 1% 확률로 갱신
        {
            SetNewRandomSpeed();
        }
    }

    // 랜덤 회전 속도를 설정하는 함수
    private void SetNewRandomSpeed()
    {
        rotationSpeed = new Vector3(
            Random.Range(-randomSpeedRange, randomSpeedRange),
            Random.Range(-randomSpeedRange, randomSpeedRange),
            Random.Range(-randomSpeedRange, randomSpeedRange)
        );
    }

    // 오일러 각도를 -180~180도로 변환하는 함수
    private Vector3 NormalizeAngles(Vector3 angles)
    {
        for (int i = 0; i < 3; i++)
        {
            if (angles[i] > 180f)
                angles[i] -= 360f;
        }
        return angles;
    }
}
