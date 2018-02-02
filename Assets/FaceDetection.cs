using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Affdex;
using Accord.Statistics;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov.Learning;
using Accord.IO;



public class FaceDetection : MonoBehaviour, FaceSplitter.FaceListener {
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


    
    public int trainTime = 5;
    const int SENSING_RATE = 2;
    const int PROCESSING_RATE = SENSING_RATE * 2;
    int currPred = 2;
    
    LinkedList<int> activeBuffer = new LinkedList<int>();

    

    public FeaturePoint[] featurePointsList;

    public FaceDetection()
    {
        FaceSplitter.registerListener(this);
    }


    int[] toArray(LinkedList<int> ll)
    {
        int bufferSize = trainTime * SENSING_RATE*10; //Need to change
        int[] res = new int[bufferSize];

        for (int i = 0; i < bufferSize && ll.Count != 0; i++)
        {
            res[i] = ll.First.Value;
            ll.RemoveFirst();
        }

        return res;
    }

    public void onFaceFound(float timestamp, int faceId)
    {
        Debug.Log("Found face");
        
        
    }

    public void onFaceLost(float timestamp, int faceId)
    {
        activeBuffer.Clear();
        Debug.Log("Lost the face");
    }

    public void onImageResults(Dictionary<int, Face> faces)
    {
        //Debug.Log(activeBuffer.Count);
        if (faces.Count == 0)
        {
            activeBuffer.Clear();
            return;
        }
        //Debug.Log("Prediction:" + currPred);
        Face face = null;
        foreach (KeyValuePair<int, Face> pair in faces)
        {
            if (face == null)
            {
                face = pair.Value;
                continue;
            }

            Face curr = pair.Value;
            Rect currRec = boundingBox(curr);
            Rect targetRec = boundingBox(face);

            float currArea = currRec.height * currRec.width;
            float targetArea = targetRec.height * targetRec.width;
            if (currArea > targetArea) face = curr;
        }
        int state = findState(face.Measurements.Orientation.eulerAngles);
        activeBuffer.AddLast(state);

        int bufferSize = trainTime * SENSING_RATE;
        if (activeBuffer.Count > bufferSize)
        {
            
            activeBuffer.RemoveFirst();
        }
        //Debug.Log("State: " + state + " Size: " + bufferSize);
        

        featurePointsList = face.FeaturePoints;
    }

    public int[] getBuffer()
    {
        return toArray(activeBuffer);
    }

    Rect boundingBox(Face face)
    {
        FeaturePoint[] featurePoints = {
                face.FeaturePoints[2],  //chinTip
                face.FeaturePoints[0],   //rightTopJaw
                face.FeaturePoints[4],    //leftTopJaw
                face.FeaturePoints[6],   //rightBrowCenter
                face.FeaturePoints[9]    //leftBrowCenter
            };

        float minx = float.MaxValue;
        float xmax = 0;
        float miny = float.MaxValue;
        float ymax = 0;
        foreach (FeaturePoint featurePoint in featurePoints)
        {
            if (featurePoint.x < minx)
            {
                minx = featurePoint.x;
            }
            if (featurePoint.x > xmax)
            {
                xmax = featurePoint.x;
            }
            if (featurePoint.y < miny)
            {
                miny = featurePoint.y;
            }
            if (featurePoint.y > ymax)
            {
                ymax = featurePoint.y;
            }
        }
        return Rect.MinMaxRect(minx, miny, xmax, ymax);

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
