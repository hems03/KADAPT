using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov.Learning;

public class Poller : MonoBehaviour {

    
    int TOP_LEFT = 0;
    int TOP_MID = 1;
    int TOP_RIGHT = 2;
    int MID_LEFT = 3;
    int MID_MID = 4;
    int MID_RIGHT = 5;
    int BOT_LEFT = 6;
    int BOT_MID = 7;
    int BOT_RIGHT = 8;
    int NONE = 9;

    public RotationPoll[] rotaters;
    const float POLL_RATE = .1f;
    RotationPoll active = null;
    Coroutine coroutine;
    HiddenMarkovClassifier classifier;
    HiddenMarkovClassifierLearning teacher;

    int[][] inputSequences;

    int[] outputLabels =
    {
        0,1,2
    };

    List<int> activeBufferA = new List<int>();
    List<int> activeBufferB = new List<int>();
    List<int> activeBufferC = new List<int>();
    int bufferIndex = -1;
    List<int> activeBuffer = null;
    

    // Use this for initialization
    void Start () {

    }

    void train()
    {
        ITopology forward = new Forward(states: 3);
        classifier = new HiddenMarkovClassifier(classes: 3,
           topology: forward, symbols: 9);
         teacher = new HiddenMarkovClassifierLearning(classifier,
            modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
            {
                Tolerance = 0.05,
                Iterations = 0
            });
        int[] A = activeBufferA.ToArray();
        int[] B = activeBufferB.ToArray();
        int[] C = activeBufferC.ToArray();
        inputSequences = new int[3][] { A, B, C };
        outputLabels = new int[3] { 0, 1, 2 };
        double error = teacher.Run(inputSequences, outputLabels);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("Space"))
        {
            train();
        }

        for(int i = 0; i < rotaters.Length; i++)
        {
            if (Input.GetKeyDown("" + i))
            {
                Debug.Log("New Object Chosen");
                setActive(rotaters[i]);

                bufferIndex = i;
                if (bufferIndex == 0)
                {
                    activeBuffer = activeBufferA;
                }else if (bufferIndex == 1)
                {
                    activeBuffer = activeBufferB;
                }else if (bufferIndex == 2)
                {
                    activeBuffer = activeBufferC;
                }


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
            /*if (activeBufferX.Count > 20)
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
            activeBufferZ.Add(z);*/
            Vector3 rotation = active.transform.eulerAngles;
            if (rotation.x > 180)
            {
                rotation.x -= 360;
            }

            if (rotation.y > 180)
            {
                rotation.y -= 360;
            }
            int state = NONE;

            if (rotation.x > 20.0 && rotation.x < 60)
            {
                if (rotation.y > 20.0&&rotation.y<60.0)
                {
                    state = BOT_RIGHT;
                }
                else if (rotation.y < -20&&rotation.y>-60)
                {
                    state = BOT_LEFT;
                    
                }
                else
                {
                    state = BOT_MID;
                }
            }
            else if (rotation.x < -20 && rotation.x > -60)
            {
                if (rotation.y > 20.0)
                {
                    state =TOP_RIGHT;
                }
                else if (rotation.y < -20)
                {
                    state = TOP_LEFT;
                }
                else
                {
                    state = TOP_MID;
                }
            }
            else
            {
                if (rotation.y > 20.0&&rotation.y<90)
                {
                    state = MID_RIGHT;
                }
                else if (rotation.y < -20&&rotation.y>-90)
                {
                    state = MID_LEFT;
                }
                else
                {
                    state = MID_MID;
                }
            }
            activeBuffer.Add(state);
            Debug.Log("State "+bufferIndex+":" + state+" "+rotation );
            yield return new WaitForSeconds(POLL_RATE);   
        }      
    }
}
