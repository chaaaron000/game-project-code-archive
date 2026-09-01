using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMonsterEye : MonoBehaviour
{
    // X축과 Z축의 회전 범위 설정
    public float xMin = -110f;
    public float xMax = -70f;
    public float zMin = -20f;
    public float zMax = 20f;

    public float fixedY; // 

    // 회전 속도 범위 설정
    public float minSpeed = 10f;
    public float maxSpeed = 50f;

    public Transform targetObject; // 바라볼 오브젝트

    private Quaternion targetRotation; // 목표 회전 값
    private float speed;               // 회전 속도
    public bool isLookingAtTarget = false; // 현재 특정 오브젝트를 바라보고 있는지 여부

    void Start()
    {
        // 초기 목표 회전값 설정
        SetNewTargetRotation();
    }

    void Update()
    {
        if (isLookingAtTarget)
        {
            // 스페이스바 입력 시 특정 오브젝트를 바라봄
            LookAtTarget();
        }
        else if (!isLookingAtTarget)
        {
            // 평소에는 랜덤하게 회전
            RandomEyeRotation();
        }
    }

    // 랜덤 회전 로직
    void RandomEyeRotation()
    {
        // 현재 회전 상태를 목표 회전값으로 천천히 변경
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * speed);

        // 목표 회전값에 거의 도달하면 새 목표 설정
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            SetNewTargetRotation();
        }
    }

    // 새 목표 회전값과 속도 설정
    void SetNewTargetRotation()
    {
        float randomX = Random.Range(xMin, xMax);
        float randomZ = Random.Range(zMin, zMax);

        targetRotation = Quaternion.Euler(randomX, fixedY, randomZ);
        speed = Random.Range(minSpeed, maxSpeed);
    }

    // 특정 오브젝트를 바라보는 함수
    void LookAtTarget()
    {
        if (targetObject != null)
        {
            // LookAt은 오브젝트를 바로 바라보게 하므로 부드럽게 이동하려면 Lerp 사용
            Quaternion lookRotation = Quaternion.LookRotation(targetObject.position - transform.position);
            
            // 기본 회전값 보정 추가
            Quaternion adjustedRotation = lookRotation * Quaternion.Euler(-90f, 0f, 0f);
            
            transform.rotation = Quaternion.Lerp(transform.rotation, adjustedRotation, Time.deltaTime * 10f);
        }
    }
}
