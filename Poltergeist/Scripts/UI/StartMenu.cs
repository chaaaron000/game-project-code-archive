using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    private string nextSceneName = "House_clean_day 1";
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            animator.enabled = true;
        }
    }

    public void OnAnimationEnd()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
