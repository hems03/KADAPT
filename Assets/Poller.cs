using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov.Learning;

public class Poller : MonoBehaviour {
    public RotationPoll[] rotaters;
    const float POLL_RATE = .1f;
    RotationPoll active = null;
    Coroutine coroutine;
    HiddenMarkovClassifier classifier;
    int[][] inputSequences =
    {
        //Need to train on real-time data then classify
        new int[] {0,280,288,311,344,21,50,73,79,70,45,10},
        new int[] {0,1,1,1,1,0,359,357,356,355,354,354,354}
    };

    int[] outputLabels =
    {
        0,1
    };

    List<int> activeBufferX = new List<int>();
    List<int> activeBufferY = new List<int>();
    List<int> activeBufferZ = new List<int>();

    // Use this for initialization
    void Start () {
        
        ITopology forward = new Forward(states: 2);
         classifier = new HiddenMarkovClassifier(classes: 2,
            topology: forward, symbols: 360);
        var teacher = new HiddenMarkovClassifierLearning(classifier,
            modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
            {
                Tolerance = 0.05, 
                Iterations = 0     
            });
        double error = teacher.Run(inputSequences, outputLabels);

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
        if (active != null) Debug.DrawLine(active.transform.position,active.transform.forward);
    }

    void setActive(RotationPoll toBeActive)
    {
        active = toBeActive;
        toBeActive.active = true;
  
    }

    IEnumerator PollRotation()
    {
        while (active != null)
        {
            //Debug.Log(active.getName()+" Rotation: " + active.transform.eulerAngles +"Count: "+activeBufferX.Count);
            if (activeBufferX.Count > 20)
            {
                int res=classifier.Decide(activeBufferX.ToArray());
                Debug.Log("Pred X: " + res);
                activeBufferX.Clear();
            }
            if (activeBufferY.Count > 20)
            {
                int res=classifier.Decide(activeBufferY.ToArray());
          
                Debug.Log("Pred Y: " + res);
                activeBufferY.Clear();
            }
            if (activeBufferZ.Count > 20)
            {
                int res=classifier.Decide(activeBufferZ.ToArray());
                Debug.Log("Pred Z: "+res);
                activeBufferZ.Clear();
            }

            int x = (int)Mathf.Floor(active.transform.eulerAngles.x);
            activeBufferX.Add(x);
            int y = (int)Mathf.Floor(active.transform.eulerAngles.y);
            activeBufferY.Add(y);

            int z = (int)Mathf.Floor(active.transform.eulerAngles.z);
            activeBufferZ.Add(z);
            yield return new WaitForSeconds(POLL_RATE);   
        }      
    }
}
