using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class AnimationStop : NetworkBehaviour
{
    public Animator animator1;

    // Start is called before the first frame update
    public override void Spawned()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        StartCoroutine(stopAnimation());
    }

    IEnumerator stopAnimation()
    {
        //animator1.speed = 0f;
        RPC_set_speed(0f);
        Debug.Log("애니메이션 일시 정지");

        // 1~2초 랜덤 대기
        float pauseDuration = Random.Range(0f, 4f);
        yield return new WaitForSeconds(pauseDuration);

        float AnimationSpeed = Random.Range(0.7f, 1.5f);

        //animator1.speed = AnimationSpeed;
        RPC_set_speed(AnimationSpeed);
        // Debug.Log("AnimationSpeed:" + AnimationSpeed);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_set_speed(float speed)
    {
        animator1.speed = speed;
    }
}
