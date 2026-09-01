using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 1.0f;  // 흔들리는 시간
    public float magnitudeZ = 0.1f; // Z축(앞뒤) 흔들림 크기
    public float magnitudeXY = 0.01f; // X, Y축(좌우, 상하) 흔들림 크기

    public float moveForwardDistance; // 흔들림 후 당겨지는 거리
    public float moveForwardTime = 0.5f; // 당겨지는 시간

    private Vector3 originalPosition;

    public void StartShake(float shakeDuration = -1)
    {
        if (shakeDuration > 0) duration = shakeDuration;
        StartCoroutine(Shake());
    }

    public void StartMoveForward()
    {
        StartCoroutine(MoveForward());
    }

    private IEnumerator Shake()
    {
        originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float fadeFactor = 1 - (elapsed / duration); // 시간이 지날수록 1 -> 0으로 감소

            float x = Random.Range(-1f, 1f) * magnitudeXY * fadeFactor;
            float y = Random.Range(-1f, 1f) * magnitudeXY * fadeFactor;
            float z = Mathf.Sin(Time.time * 20f) * magnitudeZ * fadeFactor; // 앞뒤 흔들림 감소

            transform.localPosition = originalPosition + new Vector3(x, y, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition; // 흔들림 종료 후 원래 위치로 복귀
    }

    private IEnumerator MoveForward()
    {
        originalPosition = transform.localPosition;

        Debug.Log("카메라 당겨짐");
        Vector3 targetPosition = originalPosition + Vector3.forward * moveForwardDistance;
        Vector3 startPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < moveForwardTime)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / moveForwardTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition; // 최종 위치 보정

        yield return new WaitForSeconds(1f);

        transform.localPosition = originalPosition; // 당겨진 후 원래 위치로 복귀
    }
}
