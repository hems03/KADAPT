using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Affdex;
using Accord.Statistics;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov.Learning;
using Accord.IO;



public class FaceDetection : ImageResultsListener {
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

    int currPred = 2;

    List<int> activeBuffer = new List<int>();

    HiddenMarkovClassifier classifier;

    public FeaturePoint[] featurePointsList;

    public override void onFaceFound(float timestamp, int faceId)
    {
        classifier = Serializer.Load<HiddenMarkovClassifier>("INPUT");
        Debug.Log("Found face");
    }

    public override void onFaceLost(float timestamp, int faceId)
    {
        Debug.Log("Lost the face");
    }

    public override void onImageResults(Dictionary<int, Face> faces)
    {
        Debug.Log("Prediction:"+currPred);

        foreach (KeyValuePair<int, Face> pair in faces)
        {
            int FaceId = pair.Key;  
            Face face = pair.Value;    
            //Debug.Log(face.Measurements.Orientation.eulerAngles);
            int state = findState(face.Measurements.Orientation.eulerAngles);
            activeBuffer.Add(state);
            if (activeBuffer.Count > 20)
            {
                int pred = classifier.Decide(activeBuffer.ToArray());
                activeBuffer.Clear();
                currPred = pred;
                //Debug.Log("Prediction: " + pred);
            }
            
            featurePointsList = face.FeaturePoints;
        }       
    }
    private int findState(Vector3 rotation)
    {
        
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
            if (rotation.y > 20.0 && rotation.y < 60.0)
            {
                state = BOT_RIGHT;
            }
            else if (rotation.y < -20 && rotation.y > -60)
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
                state = TOP_RIGHT;
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
            if (rotation.y > 20.0 && rotation.y < 90)
            {
                state = MID_RIGHT;
            }
            else if (rotation.y < -20 && rotation.y > -90)
            {
                state = MID_LEFT;
            }
            else
            {
                state = MID_MID;
            }
        }

        return state;
    }
}
