using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Statistics;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov.Learning;
using Accord.IO;


public class FacePoller : MonoBehaviour {
    public FaceDetection fd;
    const int SENSING_RATE = 2;
    const int PROCESSING_RATE = SENSING_RATE * 2;

    HiddenMarkovClassifier classifier;
    // Use this for initialization
    void Start () {
        classifier = Serializer.Load<HiddenMarkovClassifier>("INPUT");
        StartCoroutine("Polling");
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    IEnumerator Polling()
    {
        /*int bufferSize = trainTime * SENSING_RATE;
        if (activeBuffer.Count >= bufferSize)
        {
            int pred = classifier.Decide(toArray(activeBuffer));
            activeBuffer.Clear();
            currPred = pred;
        }*/
        while (true)
        {
            int pred = classifier.Decide(fd.getBuffer());
            Debug.Log("Pred: " + pred);
            yield return new WaitForSeconds(PROCESSING_RATE);
        }
        
    }
}
