using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using Fusion;
using Nostal.Steam;

public class videoTest : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Unity VideoPlayer 컴포넌트
    public RenderTexture renderTexture; // RenderTexture 참조
    public GameObject KeyInputText;
    public GameObject RawImage;
    public GameObject TextPanel_daughter;
    public GameObject TextPanel_father;

    [SerializeField] private AudioSource m_audioSource;

    private bool isVideoEnded = false; // 비디오 종료 여부

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
        
        ResetRenderTexture();

        videoPlayer.loopPointReached += OnVideoEnd; // 비디오 끝 이벤트 연결
    }

    void Update()
    {
        if (isVideoEnded && Input.anyKeyDown) // 비디오 끝난 후 키 입력 감지
        {
            Debug.Log("키 입력 감지, 로비 씬으로 이동");
            GameSceneManager.Instance.LoadScene(NostalgiaGameLevel.MainMenu);
        }
    }

    private IEnumerator SoundEventCoroutine()
    {
        Debug.Log("엔딩 영상 시간 카운트 시작");
        //yield return new WaitWhile(() => videoPlayer.isPlaying && videoPlayer.time > 28f);
        yield return new WaitForSeconds(28f);
        m_audioSource.Play();
    }

    // RenderTexture 초기화하여 비디오 시작 장면으로 설정
    void ResetRenderTexture()
    {
        if (renderTexture != null)
        {
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;
        }
    }

    // 비디오 재생
    public void PlayVideo()
    {
        if (videoPlayer.isPlaying)
        {
            return;
        }
        
        RawImage.SetActive(true);
        videoPlayer.Play();
        StartCoroutine(SoundEventCoroutine());
    }

    // 비디오 일시 정지
    public void PauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
    }

    // 비디오 정지
    public void StopVideo()
    {
        videoPlayer.Stop();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("비디오 끝, 처리 실행");

        isVideoEnded = true; // 비디오가 끝났음을 표시
        KeyInputText.SetActive(true);
        
        // 스팀 통계 업데이트
        SteamStatsAndAchievements.Instance.IncreaseEndingCreditsWatchedCount();
    }

    public void TriggerEvent()
    {
        Debug.Log("신호 받음! 함수 실행됨!");
        PlayVideo();
        // 원하는 행동 수행
    }

    public void TriggerFadeOutEvent(){
        //화면 페이드 아웃 트리거
        UIManager.Instance.FadeView.FadeOut(1f);
    }

    public void TriggerFadeInEvent(){
        //화면 페이드 인 트리거
        UIManager.Instance.FadeView.FadeIn(1f);
    }

    public void ShowTextPanel(){  
        Debug.Log("LocalPlayer: " + GameManager.Instance.GetLocalPlayer() + " FatherNetworkObject: " + GameManager.Instance.FatherNetworkObject);
        
        Debug.Log("딸 엔딩 텍스트 출력");
        TextPanel_daughter.SetActive(true);
    }
}
