using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TutorialChild : NetworkBehaviour
{
    public Animator animator;
    [System.Serializable]
    public class ChildMovingPosition
    {
        public Transform start;
        public Transform end;
    }
    [SerializeField] private ChildMovingPosition[] childMovingPositions;
    [SerializeField] private AudioSource[] audioSources;

    public override void Spawned() {
        animator = GetComponent<Animator>();
        StartCoroutine(Cry());
    }

    public void SetAnimation(string triggerName) {
        animator.SetTrigger(triggerName);
    }

    public void Triggered(int index) {
        Move(index);
    }

    public void Move(int index) {
        if (index < 0 || index >= childMovingPositions.Length) {
            Debug.LogError("Index out of range for child moving positions.");
            return;
        }

        Transform start = childMovingPositions[index].start;
        Transform end = childMovingPositions[index].end;

        // Move the child to the start position
        transform.position = start.position;
        transform.rotation = start.rotation;

        // Optionally, you can add logic to animate or move towards the end position
        // For example, using a coroutine or tweening library
        StartCoroutine(MoveToEnd(end.position, 2f)); // Move to end position over 1 second
    }

    public IEnumerator MoveToEnd(Vector3 endPosition, float duration) {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(0,0,0); // Ensure final position is set
    }

    private IEnumerator Cry()
    {
        while (true) 
        {
            float waitTime = Random.Range(8, 13);
            yield return new WaitForSeconds(waitTime);
            
            int cryNum = Random.Range(0, 5);
            if (cryNum == 0) {
                audioSources[0].Play();
            } else if(cryNum == 1) {
                audioSources[1].Play();
            }
            else if(cryNum == 2) {
                audioSources[2].Play();
            } else if(cryNum == 3) {
                audioSources[3].Play();
            } else if(cryNum == 4) {
                audioSources[4].Play();
            }
        }
    }
}
