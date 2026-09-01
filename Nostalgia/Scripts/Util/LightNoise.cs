using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightNoise : MonoBehaviour
{
    public Light flickerLight;  // 깜빡일 라이트
    public float minIntensity = 0.5f; // 최소 밝기
    public float maxIntensity = 2.0f; // 최대 밝기
    public float flickerSpeed = 0.1f; // 깜빡이는 속도

    private float targetIntensity; // 목표 밝기 값
    private float timer; // 타이머

    void Start()
    {
        if (flickerLight == null)
        {
            flickerLight = GetComponent<Light>(); // 자동으로 라이트 가져오기
        }
        targetIntensity = flickerLight.intensity;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity); // 새로운 목표 밝기 설정
            timer = flickerSpeed; // 다음 변화까지 대기 시간
        }

        // 현재 밝기를 목표 밝기로 부드럽게 변화
        flickerLight.intensity = Mathf.Lerp(flickerLight.intensity, targetIntensity, Time.deltaTime * 10);
    }
}
