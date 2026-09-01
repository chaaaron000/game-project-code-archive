using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    void Start() // 임시 동영상 추후 변경 예정
    {
        // "비디오가 끝나는 순간(loopPointReached)"에 GoToTitle 함수를 실행하라고 유니티에게 예약
        videoPlayer.loopPointReached += EndReached;
    }

    void Update()
    {
        //  스킵 기능! (마우스 클릭이나 아무 키나 누르면 스킵)
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            GoToTitle();
        }
    }

    void EndReached(VideoPlayer vp)
    {
        GoToTitle();
    }

    void GoToTitle()
    {
        // 동영상이 끝나거나 스킵하면 "Title" 씬으로 넘어갑니다.
        SceneManager.LoadScene("Title");
    }
}