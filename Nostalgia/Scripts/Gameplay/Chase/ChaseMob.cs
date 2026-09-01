using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nostal;
using Fusion;
using UnityEngine.AI;
using ExitGames.Client.Photon.StructWrapping;

public class ChaseMob : NetworkBehaviour
{
    public Transform[] target;
    public Transform[] wrongWay;
    
    [SerializeField] public NavMeshAgent agent;
    public bool isAttackable = true;
    
    public bool WrongWayFlag {
        get; set;
    } = false;

    private int wrongWayCnt = 0;

    public override void Spawned() {
        GameplayEventManager.ChaseMapClear += OnClear;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        GameplayEventManager.ChaseMapClear -= OnClear;
    }

    public void OnClear() {
        agent.enabled = false;
        agent.isStopped = true;
        isAttackable = false;
    }

    public void StartChase() {
        StartCoroutine(MobFunc());
    }

    public IEnumerator MobFunc() {
        agent.enabled = true;
        agent.isStopped = false;
        NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas);
        agent.SetDestination(target[0].position);
        int cnt = 0;
        yield return new WaitForSeconds(0.3f);
        while(true){
            if (agent.remainingDistance < 1.0f)
            {
                Debug.Log("agent remainingDistance: " + agent.remainingDistance);
                if(WrongWayFlag) {
                    agent.SetDestination(wrongWay[cnt].position);
                }
                else {
                    cnt++;
                    agent.SetDestination(target[cnt].position);
                    wrongWayCnt = 0;
                }
            }
            
            yield return null;
        }
    }

    public void SetWrongWayTrigger(bool flag) {
        Debug.Log("SetWrongWayTrigger: " + flag);
        if(flag == true) {
            wrongWayCnt++;
            if(wrongWayCnt > 0) {
                WrongWayFlag = true;
            }
        }
        else {
            wrongWayCnt--;
            if(wrongWayCnt <= 0) {
                WrongWayFlag = false;
                wrongWayCnt = 0;
            }
        }
        Debug.Log("WrongWayFlag: " + WrongWayFlag);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !isAttackable)
        {
            return;
        }

        isAttackable = false;
        
        StartCoroutine(ChaseMapManager.Instance.ResetChase(other.gameObject.GetComponent<NetworkObject>()));
    }
}
