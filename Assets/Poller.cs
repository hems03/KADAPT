﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov.Learning;
using Accord.IO;
using Affdex;

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

    Transform mainCamera;
    CameraInput cameraInput;
    string cameraName;
    string currentCameraName = "";

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
        load();
        active = rotaters[0];
        activeBuffer = activeBufferA;
        coroutine = StartCoroutine("PollRotation");
        StartCoroutine("Training");

    }

    void load()
    {
        classifier = Serializer.Load<HiddenMarkovClassifier>("INPUT");
        teacher = new HiddenMarkovClassifierLearning(classifier, modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
        {
            Tolerance = 0.1,
            Iterations = 0
        });

    }

    void train()
    {
        ITopology forward = new Forward(states: 3);
        classifier = new HiddenMarkovClassifier(classes: 3,
           topology: forward, symbols: 9);
         teacher = new HiddenMarkovClassifierLearning(classifier,
            modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
            {
                Tolerance = 0.1,
                Iterations = 0
            });
        int[] A = activeBufferA.ToArray();
        int[] B = activeBufferB.ToArray();
        int[] C = activeBufferC.ToArray();
        inputSequences = new int[3][] { A, B, C };
        outputLabels = new int[3] { 0, 1, 2 };
        double error = teacher.Run(inputSequences, outputLabels);
        Serializer.Save<HiddenMarkovClassifier>(teacher.Classifier,"INPUT");
        activeBufferA.Clear();
        activeBufferB.Clear();
        activeBufferC.Clear();
        Debug.Log("Trained Model");
    }

    void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        cameraInput = mainCamera.GetComponent<CameraInput>();
    }

    // Update is called once per frame
    void Update () {

        

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Training");
            active.active = false;
            train();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            load();
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
        if (toBeActive != null)
        {
            toBeActive.active = true;
        }
       
  
    }

    IEnumerator PollRotation()
    {
        //Debug.Log("Coroutine");
        while (active != null)
        {
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


            if (activeBuffer.Count > 20&&active.active==false)
            {
                int pred = classifier.Decide(activeBuffer.ToArray());
                activeBuffer.Clear();
                Debug.Log("Prediction: " + pred);
            }else
            {
                if (active.active)
                {
                    Debug.Log("State " + bufferIndex + ":" + state + " " + rotation);
                }
               
            }
            yield return new WaitForSeconds(POLL_RATE);   
        }      
    }

    IEnumerator Training()
    {
        setActive(rotaters[0]);
       
        activeBuffer = activeBufferA;
        yield return new WaitForSeconds(5);
        setActive(rotaters[1]);
        activeBuffer = activeBufferB;
        yield return new WaitForSeconds(5);
        setActive(rotaters[2]);
        activeBuffer = activeBufferC;
        yield return new WaitForSeconds(5);
        setActive(null);
        train();
       
    }
}
