using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poll : MonoBehaviour {
    public RotationPoll[] rotaters;

    const float POLL_RATE = 1f;
    RotationPoll active = null;
    Coroutine coroutine;
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        for(int i = 0; i < rotaters.Length; i++)
        {

            if (Input.GetKeyDown("" + i))
            {
                Debug.Log("New Object Chosen");
                setActive(rotaters[i]);
                if (coroutine != null) StopCoroutine(coroutine);

                coroutine=StartCoroutine("PollRotation");

                continue;          
            }

            if (rotaters[i] != active)
            {
                rotaters[i].active = false;
            }
      
        }
	}

    void setActive(RotationPoll toBeActive)
    {
        active = toBeActive;
        toBeActive.active = true;
        Debug.Log(active.active);
    }

    IEnumerator PollRotation()
    {
        while (active != null)
        {
            Debug.Log("Rotation: " + active.transform.eulerAngles);
            yield return new WaitForSeconds(POLL_RATE);   
        }      
    }
}
