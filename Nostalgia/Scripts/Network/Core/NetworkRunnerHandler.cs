using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public Runner networkRunner;
    public List<SessionInfo> _sessionList;
    
    private void Awake()
    {
        NetworkRunner nr = FindObjectOfType<NetworkRunner>();
        
        if (nr == null)
            MakeNetworkRunner();
        else
            networkRunner = nr.GetComponent<Runner>();
    }

    private void OnEnable()
    {
        if ((networkRunner = FindObjectOfType<Runner>()) != null)
            networkRunner.RunnerLeaveGame();
    }

    public void MakeNetworkRunner() {
        networkRunner = Instantiate(networkRunnerPrefab).GetComponent<Runner>();
        // networkRunner.name = "Network runner";
    }
    
    /// <summary>
    /// Runner가 생성 시에 겹치는 경우가 발생하지 않도록 1프레임 뒤에 스폰하는 메소드
    /// </summary>
    public void ScheduleRunnerCreation()
    {
        StartCoroutine(CreateRunnerNextFrame());
    }

    private IEnumerator CreateRunnerNextFrame()
    {
        // 한 프레임 대기
        yield return null;
        MakeNetworkRunner();
    }

    //session을 생성하는 함수
    public void CreateGame()
    {
        //networkRunner가 없다면, networkRunnerPrefab을 이용해 생성.
        if(networkRunner == null)
        {
            MakeNetworkRunner();
        }

        networkRunner.RunnerCreateGame();
    }

    //session에 참가하는 함수
    public void JoinGame(string sessionName)//SessionInfo sessionInfo)
    {
        Debug.Log($"Try Join session {sessionName}");

        //networkRunner가 없다면, networkRunnerPrefab을 이용해 생성.
        if(networkRunner == null)
        {
            MakeNetworkRunner();
        }

        networkRunner.RunnerJoinGame(sessionName);
    }

    public void LeaveGame()
    {
        if (networkRunner == null)
            MakeNetworkRunner();
        
        networkRunner.RunnerLeaveGame();
    }
}

