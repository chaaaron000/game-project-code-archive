using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Angry : Entity
{
    public Camera mainCamera;
    public LayerMask obstacleMask; // 장애물로 간주할 레이어
    private Coroutine gazeCoroutine = null;
    private bool despawnTrigger = false;
    [SerializeField] public DoorMonsterEye[] eyes;
    public int _spawnPositionIndex;
    public Player localPlayer;
    public AudioSource audioSource;

    public override void Spawned() {
        Init();
    }

    public void Init() {
        Debug.Log("Angry Init");
        mainCamera = Camera.main;
        for(int i=0; i<eyes.Length; i++) {
            eyes[i].fixedY = gameObject.transform.rotation.y;
        }
        localPlayer = GameManager.Instance.GetLocalPlayer().GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();
        gazeCoroutine = StartCoroutine(Gaze());
        Debug.Log("Angry Spawn complete");
    }

    // [Rpc(RpcSources.All, RpcTargets.All)]
    // public void GazeInitRpc() {
    //     isInitialFlag = true;
    // }

    IEnumerator Gaze() {
        int gazeCnt = 0;
        bool flag = false;

        //최초로 시야에 들어올 때까지 계산
        while(!flag) {
            flag = Gaze(mainCamera);
            Debug.Log("Not in view, waiting for gaze...");
            yield return new WaitForSeconds(0.5f);
        }

        SoundManager.Instance.SFX_loop_Play("angryWhisper", this.gameObject);
        SoundManager.Instance.SFX_Play("chased");
        GameManager.Instance.GetLocalPlayer().GetComponent<Player>().angryPanel.SetActive(true);

        Debug.Log("Init Gaze");
        for(int i=0; i<eyes.Length; i++) {
            eyes[i].isLookingAtTarget = true;
            eyes[i].targetObject = mainCamera.transform;
        }

        while(true) {
            yield return new WaitForSeconds(1.0f);
            flag = Gaze(mainCamera);
            Debug.Log("Gaze check: " + flag + ", gazeCnt: " + gazeCnt);
            if(flag && localPlayer._deathFlag == false) {
                gazeCnt += 1;
                Player targetPlayer = GameManager.Instance.GetLocalPlayer().GetComponent<Player>();

                //보고 있을 때 소리 1.0f
                SoundManager.Instance.SFX_Set_Volume("angryWhisper", this.gameObject, 1.0f);
                //죽는 상황일 때 미리 소리 중지
                if(targetPlayer.Health <= 10f) {
                    SoundManager.Instance.SFX_loop_Stop("angryWhisper", this.gameObject);
                }

                Attack(targetPlayer, 33, (int)mobID.angry);
            }
            else {
                gazeCnt -= 1;
                //안 보고 있을 때 소리 0.1f
                SoundManager.Instance.SFX_Set_Volume("angryWhisper", this.gameObject, 0.1f);
                //일정 시간 이상 안 보면 사라짐
                if(gazeCnt <= -5) {
                    DespawnRpc();
                }
            }
            flag = false; 
        }
    }

    bool Gaze(Camera camera) {
        bool flag = false;
        // 월드 좌표를 뷰포트 좌표로 변환
        Vector3 viewportPoint = camera.WorldToViewportPoint(gameObject.transform.position + new Vector3(0,2.2f,0));
        
        // 오브젝트가 카메라 앞에 있는지 확인 (z 좌표가 0보다 큰지 확인)
        bool isInView = viewportPoint.z > 0 &&
                        viewportPoint.x > 0 && viewportPoint.x < 1 &&
                        viewportPoint.y > 0 && viewportPoint.y < 1;
        Debug.Log("isInView = " + isInView);

        // 카메라와 오브젝트 사이의 방향 벡터
        Vector3 directionToTarget = gameObject.transform.position - camera.transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        if (isInView && distanceToTarget < 10f)
        {
            // Raycast를 사용하여 카메라에서 오브젝트까지의 경로에 장애물이 있는지 확인
            if (Physics.Raycast(camera.transform.position, directionToTarget, out RaycastHit hit, distanceToTarget, obstacleMask))
            {
                // 장애물이 있는 경우
                Debug.Log(gameObject + "/ Object is in view but obstructed by " + hit.collider.name);
            }
            else
            {
                // 장애물이 없는 경우
                Debug.Log(gameObject + "/ Object is in view and not obstructed");
                flag = true;
            }
        }
        else
        {
            //Debug.Log("Object is out of view");
        }
        return flag;
    }
    
    public override void FixedUpdateNetwork() {
        if(despawnTrigger) {
            Runner.Despawn(gameObject.GetComponent<NetworkObject>());
        }
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void DespawnRpc() {
        if(gazeCoroutine != null) {
            StopCoroutine(gazeCoroutine);
        }
        GameManager.Instance._mapCreator.ReclaimPositionAngryRpc(_spawnPositionIndex);
        despawnTrigger = true;
    }

}
