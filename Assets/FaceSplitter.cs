using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Affdex;
using System;

public class FaceSplitter : ImageResultsListener
{

    public interface FaceListener
    {
        void onImageResults(Dictionary<int, Face> faces);


        void onFaceFound(float timestamp, int faceId);

        void onFaceLost(float timestamp, int faceId);
    }

    static LinkedList<FaceListener> listeners=new LinkedList<FaceListener>();

    public static void registerListener(FaceListener listener)
    {
        listeners.AddLast(listener);
    }

    public override void onImageResults(Dictionary<int, Face> faces)
    {
        LinkedListNode<FaceListener> ptr = listeners.First;
        while (ptr != null)
        {
            ptr.Value.onImageResults(faces);
            ptr = ptr.Next;
        }
    }

    public override void onFaceFound(float timestamp, int faceId)
    {
        LinkedListNode<FaceListener> ptr = listeners.First;
        while (ptr != null)
        {
            ptr.Value.onFaceFound(timestamp,faceId);
            ptr = ptr.Next;
        }
    }

    public override void onFaceLost(float timestamp, int faceId)
    {
        LinkedListNode<FaceListener> ptr = listeners.First;
        while (ptr != null)
        {
            ptr.Value.onFaceLost(timestamp,faceId);
            ptr = ptr.Next;
        }
    }
}
