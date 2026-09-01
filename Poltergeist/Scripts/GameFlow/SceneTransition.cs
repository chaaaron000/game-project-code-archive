using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public GameObject Player;

    public string sceneToLoad = "House_worn_night 1";
    public float timeToStay;
    private float timer = 0.0f;

    private float leftEnd = 10.0f;
    private float rightEnd = -13.0f;
    private float topEnd = 5.0f;
    private float bottomEnd = -7.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Debug.Log(Player.transform.position);
        //Debug.Log(timer);

        if (hasEntered())
        {
            // Debug.Log("Player Entered");
            
            timer += Time.deltaTime;
            if (timer >= timeToStay)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            // Debug.Log("Player Exited");
            timer = 0.0f;
        }
    }

    bool hasEntered()
    {
        return (Player.transform.position.x < leftEnd && Player.transform.position.x > rightEnd && Player.transform.position.z < topEnd && Player.transform.position.z > bottomEnd) ? true : false;
    }
}
