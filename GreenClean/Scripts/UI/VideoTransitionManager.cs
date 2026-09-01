using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoTransitionManager : MonoBehaviour
{
    [Header("설정")]
    public VideoPlayer videoPlayer;
    public string targetSceneName = "Game"; // 로딩할 게임 씬 이름

    private AsyncOperation _asyncOp;

    private void Awake()
    {
        // 씬이 "Title"에서 "Game"으로 넘어가도 이 영상 프리팹이 파괴되지 않게 막아줍니다.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 렌더 텍스처에 남아있는 이전 영상의 찌꺼기(마지막 프레임)를 강제로 날려버립니다.
        if (videoPlayer.targetTexture != null)
        {
            videoPlayer.targetTexture.Release();
        }

        // 영상의 재생 위치를 무조건 0프레임(가장 처음)으로 강제 고정합니다.
        videoPlayer.frame = 0;

        // 1. 영상 재생 시작
        videoPlayer.Play();

        // 2. 타이틀 BGM 정지 
        // (SoundManager에 StopBGM 함수가 없다면 추가해 주시거나, 상황에 맞게 수정하세요)
        if (SoundManager.Instance != null)
        {
            // 예시: SoundManager.Instance.StopBGM(); 
            // 만약 Stop 함수를 안 만드셨다면 아래처럼 꼼수로 오디오 소스를 직접 끌 수도 있습니다.
            AudioSource bgmSource = SoundManager.Instance.GetComponent<AudioSource>();
            if (bgmSource != null) bgmSource.Stop();
        }

        // 3. 영상이 끝났을 때를 감지하는 이벤트 연결
        videoPlayer.loopPointReached += OnVideoFinished;

        // 4. 뒤에서는 아무도 모르게 Game 씬을 미리 로딩하기 시작합니다.
        StartCoroutine(PreloadScene());
    }

    private IEnumerator PreloadScene()
    {
        // 백그라운드에서 씬 로딩 시작
        _asyncOp = SceneManager.LoadSceneAsync(targetSceneName);

        // 로딩이 100% 다 되어도, 영상이 안 끝났으면 화면을 넘기지 말라고 꽉 잡아둡니다.
        _asyncOp.allowSceneActivation = false;

        yield return null;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // 영상 끝! 이벤트 해제
        videoPlayer.loopPointReached -= OnVideoFinished;

        // 5. 잡아두었던 씬 전환을 허락합니다.
        if (_asyncOp != null)
        {
            _asyncOp.allowSceneActivation = true;
        }

        // [참고] 이 순간 씬이 Game으로 바뀌면서 
        // GameManager의 Start()가 실행되고 -> GameBGM이 자동으로 예쁘게 흘러나옵니다!

        // 6. 씬이 넘어가면 이 영상 프리팹은 역할이 끝났으니 스스로 파괴합니다.
        Destroy(gameObject, 0.5f);
    }
}