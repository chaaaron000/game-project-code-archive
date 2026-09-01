using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EndingSceneController : MonoBehaviour
{
    public PlayableDirector playableDirector;
    // Start is called before the first frame update
    void Start()
    {
        GameSceneManager.OnSceneLoadComplete += EndingSceneLoadingComplete;
    }

    private void EndingSceneLoadingComplete(int sceneIndex){
        Debug.Log("엔딩 씬 로딩 끝");
        StartCoroutine(EndingTimelinePlay());
    }

    IEnumerator EndingTimelinePlay(){
        yield return new WaitForSeconds(4f);

        Debug.Log("코루틴 타임라인 실행");
        playableDirector.Play();
    }

}
