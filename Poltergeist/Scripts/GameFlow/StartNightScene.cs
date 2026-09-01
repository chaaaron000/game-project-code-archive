using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartNightScene : MonoBehaviour
{
    Animator animator;
    GameObject ladyGhost;
    public GameObject firstWord;
    AudioSource audioSource;
    public GameObject youcant;
    public GameObject howabout;
    public GameObject run;

    public void Start()
    {
        // Canvas 아래의 FirstWord 오브젝트를 찾아 TextMeshProUGUI 컴포넌트 가져오기
        //firstWord = GameObject.Find("Canvas/FirstWord").GetComponent<GameObject>();
        firstWord.gameObject.SetActive(false);
        youcant.gameObject.SetActive(false);
        howabout.gameObject.SetActive(false);
        run.gameObject.SetActive(false);
        ladyGhost = GameObject.Find("LadyGhost");
        ladyGhost.SetActive(true);

        animator = gameObject.GetComponent<Animator>();
        animator.Play("First Scene");
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        /*AnimationEvent animationEvent = new AnimationEvent();
        animationEvent.functionName = "OnAnimationEnd";
        animationEvent.time = animator.GetCurrentAnimatorStateInfo(0).length;
        animator.GetCurrentAnimatorClipInfo[0].clip.AddEvent(animationEvent);*/
    }

    public void OnAnimationEnd()
    {
        animator.enabled = false;
        audioSource.Stop();
        //ladyGhost.SetActive(false);
        firstWord.gameObject.SetActive(false);

    }

    public void FirstLineStart(){
        firstWord.gameObject.SetActive(true);
    }

    public void FirstLineEnd(){
        firstWord.gameObject.SetActive(false);
    }

    public void youcantStart(){
        youcant.gameObject.SetActive(true);
    }

    public void youcantEnd(){
        youcant.gameObject.SetActive(false);
    }

    public void howaboutStart(){
        howabout.gameObject.SetActive(true);
    }

    public void howaboutEnd(){
        howabout.gameObject.SetActive(false);
    }

    public void runStart(){
        run.gameObject.SetActive(true);
    }

    public void runStartEnd(){
        run.gameObject.SetActive(false);
    }

    public void AnimationEnd(){
        animator.enabled=false;
    }

}
