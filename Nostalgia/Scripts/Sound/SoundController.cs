using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.Mathematics;
using System;
using Nostal.Network;

public class SoundController : NetworkBehaviour
{
    private NetworkRunner m_runner => NetworkManager.Instance.Runner;

    [SerializeField] private GameObject audioPrefab; // 자식 오브젝트로 생성할 오브젝트 프리팹
    [SerializeField] private bool is3DSound;    //3d 사운드인지
    [SerializeField] private int maxAudioNum;   //이 오브젝트가 가질수 있는 오디오프리팹 오브젝트 갯수

    private AudioSource audioSource; // 오디오 소스 컴포넌트
    private AudioSource loopAudioSource; //루프 전용 오디오 소스 컴포넌트
    private Dictionary<string, GameObject> audioDic = new Dictionary<string, GameObject>(); // 오디오 오브젝트 관리 딕셔너리
    private Queue<string> audioQueue = new Queue<string>(); // 오디오 이름 관리 큐 (가장 오래된 오디오 소스 판정을 위해)
    private Queue<string> loopAudioQueue = new Queue<string>();
    private Dictionary<string, GameObject> loopAudioDic = new Dictionary<string, GameObject>(); //루프 오디오 오브젝트 관리 딕셔너리

    private Camera m_camera;

    private GameObject newAudio;

    private void createAudio()
    {
        m_camera = Camera.main;

        if (!m_runner.IsRunning)
        {
            Debug.LogError("NetworkRunner가 아직 실행되지 않았습니다!");
            return;
        }

        //newAudio = Runner.Spawn(audioPrefab, this.transform.position, Quaternion.identity, Runner.LocalPlayer, OnBeforeSpawned).gameObject;
        newAudio = Instantiate(audioPrefab, this.transform.position, Quaternion.identity, transform);
    }

    // public void OnBeforeSpawned(NetworkRunner runner, NetworkObject obj) {
    //     obj.GetComponent<AudioPrefab>().parent = gameObject;
    // }

    // 클립 재생
    public void audioPlay(AudioClip audioClip, float maxDistance)
    {
        if (audioDic.ContainsKey(audioClip.name))
        {
            // 이미 존재하는 오디오 재생
            audioSource = audioDic[audioClip.name].GetComponent<AudioSource>();
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                if (is3DSound)
                {
                    audioSource.maxDistance = maxDistance;
                    if (CheckDistanceY())
                    {
                        audioSource.volume = 0.1f; // 카메라와 Y축 거리 차이가 3 이상이면 소리 안나게
                    }
                    else
                    {
                        audioSource.volume = 1.0f;
                    }
                }
            }
        }
        else
        {
            if (audioDic.Count >= maxAudioNum)
            {
                // 최대 용량 초과 시, 가장 오래된 소리 제거 및 재사용
                string oldestAudioName = audioQueue.Dequeue();
                GameObject oldestAudioObj = audioDic[oldestAudioName];

                // 딕셔너리에서 제거 후, 재사용 설정
                audioDic.Remove(oldestAudioName);

                // 새 오디오 설정
                audioSource = oldestAudioObj.GetComponent<AudioSource>();
                audioSource.Stop();
                audioSource.clip = audioClip;
                if (is3DSound)
                {
                    audioSource.maxDistance = maxDistance;    //만약 3d 사운드라면 최대거리 설정
                    if (CheckDistanceY())
                    {
                        audioSource.volume = 0.1f; // 카메라와 Y축 거리 차이가 3 이상이면 소리 안나게
                    }
                    else
                    {
                        audioSource.volume = 1.0f;
                    }
                }
                audioSource.Play();

                // 딕셔너리에 다시 추가
                audioDic.Add(audioClip.name, oldestAudioObj);
                audioQueue.Enqueue(audioClip.name);
            }
            else
            {
                // 새로운 오디오 생성 및 추가
                createAudio();
                audioDic.Add(audioClip.name, newAudio);
                audioQueue.Enqueue(audioClip.name);

                audioSource = newAudio.GetComponent<AudioSource>();
                audioSource.clip = audioClip;

                if (is3DSound)
                {
                    audioSource.maxDistance = maxDistance;    //만약 3d 사운드라면 최대거리 설정
                    if (CheckDistanceY())
                    {
                        audioSource.volume = 0.1f; // 카메라와 Y축 거리 차이가 3 이상이면 소리 안나게
                    }
                    else
                    {
                        audioSource.volume = 1.0f;
                    }
                }

                audioSource.Play();
            }
        }
    }

    private bool CheckDistanceY()
    {
        if (!m_camera)
        {
            m_camera = Camera.main;

            if (!m_camera)
            {
                return false;
            }
        }

        float distance = math.abs(m_camera.transform.position.y - transform.position.y);
        return distance > 3.0f;
    }

    // 클립 정지
    public void audioStop(string audioClipName)
    {
        if (!audioDic.ContainsKey(audioClipName)) return;

        audioSource = audioDic[audioClipName].GetComponent<AudioSource>();
        audioSource.Stop();
    }

    public void loopAudioPlay(AudioClip audioClip, float maxDistance)
    {
        if (loopAudioDic.ContainsKey(audioClip.name))
        {
            // 이미 존재하는 오디오 재생
            audioSource = loopAudioDic[audioClip.name].GetComponent<AudioSource>();
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            if (loopAudioDic.Count >= maxAudioNum)
            {
                // 최대 용량 초과 시, 가장 오래된 소리 제거 및 재사용
                string oldestAudioName = loopAudioQueue.Dequeue();
                GameObject oldestAudioObj = loopAudioDic[oldestAudioName];

                // 딕셔너리에서 제거 후, 재사용 설정
                loopAudioDic.Remove(oldestAudioName);

                // 새 오디오 설정
                audioSource = oldestAudioObj.GetComponent<AudioSource>();
                audioSource.Stop();
                audioSource.clip = audioClip;
                if (is3DSound)
                {
                    audioSource.maxDistance = maxDistance;    //만약 3d 사운드라면 최대거리 설정
                }
                audioSource.loop = true;
                audioSource.Play();

                // 딕셔너리에 다시 추가
                audioDic.Add(audioClip.name, oldestAudioObj);
                loopAudioQueue.Enqueue(audioClip.name);
            }
            else
            {
                // 새로운 오디오 생성 및 추가
                createAudio();
                loopAudioDic.Add(audioClip.name, newAudio);
                loopAudioQueue.Enqueue(audioClip.name);

                audioSource = newAudio.GetComponent<AudioSource>();
                audioSource.clip = audioClip;

                if (is3DSound) audioSource.maxDistance = maxDistance;    //만약 3d 사운드라면 최대거리 설정
                audioSource.loop = true;

                audioSource.Play();
            }
        }
    }

    // 클립 정지
    public void loopAudioStop(string audioClipName)
    {
        if (!loopAudioDic.ContainsKey(audioClipName)) return;

        audioSource = loopAudioDic[audioClipName].GetComponent<AudioSource>();
        audioSource.Stop();
    }

    public void Set_AudioDistance(AudioClip audioClip, float maxDistance)
    {
        if (audioDic.ContainsKey(audioClip.name))
        {
            if (is3DSound)
            {
                // 해당 소리가 있고, 입체음향이면 거리 조절
                audioSource = audioDic[audioClip.name].GetComponent<AudioSource>();
                audioSource.maxDistance = maxDistance;
            }
        }
        else
        {
            //없으면 리턴
            return;
        }
    }

    public void SetLoopAudioVolume(AudioClip audioClip, float volume)
    {
        if (loopAudioDic.ContainsKey(audioClip.name))
        {
            // 해당 소리가 있으면 볼륨 조절
            audioSource = loopAudioDic[audioClip.name].GetComponent<AudioSource>();
            audioSource.volume = volume;
            // Debug.Log("SetAudioVolume: " + audioClip.name + " volume: " + volume);
            // Debug.Log("audioSource.volume: " + audioSource.volume);
        }
    }

    public void ResetAudioObject()
    {
        // 모든 오디오 오브젝트를 정지하고 삭제
        foreach (var audioObj in audioDic.Values)
        {
            audioSource = audioObj.GetComponent<AudioSource>();
            audioSource.Stop();
            Destroy(audioObj);
        }
        audioDic.Clear();
        audioQueue.Clear();

        foreach (var loopAudioObj in loopAudioDic.Values)
        {
            audioSource = loopAudioObj.GetComponent<AudioSource>();
            audioSource.Stop();
            Destroy(loopAudioObj);
        }
        loopAudioDic.Clear();
        loopAudioQueue.Clear();
    }
    
    public void audioPlayDaughterOnly(AudioClip audioClip)
    {
        float maxDistance = Runner.LocalPlayer == GameManager.Instance.DaughterPlayerRef ? 24 : 8;

        if (audioDic.ContainsKey(audioClip.name))
        {
            // 이미 존재하는 오디오 재생
            audioSource = audioDic[audioClip.name].GetComponent<AudioSource>();
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                if (is3DSound)
                {
                    audioSource.maxDistance = maxDistance;
                    if (CheckDistanceY())
                    {
                        audioSource.volume = 0.1f; // 카메라와 Y축 거리 차이가 3 이상이면 소리 안나게
                    }
                    else
                    {
                        audioSource.volume = 1.0f;
                    }
                }
            }
        }
        else
        {
            if (audioDic.Count >= maxAudioNum)
            {
                // 최대 용량 초과 시, 가장 오래된 소리 제거 및 재사용
                string oldestAudioName = audioQueue.Dequeue();
                GameObject oldestAudioObj = audioDic[oldestAudioName];

                // 딕셔너리에서 제거 후, 재사용 설정
                audioDic.Remove(oldestAudioName);

                // 새 오디오 설정
                audioSource = oldestAudioObj.GetComponent<AudioSource>();
                audioSource.Stop();
                audioSource.clip = audioClip;
                if (is3DSound)
                {
                    audioSource.maxDistance = maxDistance;    //만약 3d 사운드라면 최대거리 설정
                    if (CheckDistanceY())
                    {
                        audioSource.volume = 0.1f; // 카메라와 Y축 거리 차이가 3 이상이면 소리 안나게
                    }
                    else
                    {
                        audioSource.volume = 1.0f;
                    }
                }
                audioSource.Play();

                // 딕셔너리에 다시 추가
                audioDic.Add(audioClip.name, oldestAudioObj);
                audioQueue.Enqueue(audioClip.name);
            }
            else
            {
                // 새로운 오디오 생성 및 추가
                createAudio();
                audioDic.Add(audioClip.name, newAudio);
                audioQueue.Enqueue(audioClip.name);

                audioSource = newAudio.GetComponent<AudioSource>();
                audioSource.clip = audioClip;

                if (is3DSound)
                {
                    audioSource.maxDistance = maxDistance;    //만약 3d 사운드라면 최대거리 설정
                    if (CheckDistanceY())
                    {
                        audioSource.volume = 0.1f; // 카메라와 Y축 거리 차이가 3 이상이면 소리 안나게
                    }
                    else
                    {
                        audioSource.volume = 1.0f;
                    }
                }

                audioSource.Play();
            }
        }
    }
}