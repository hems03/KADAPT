using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class RotationPoll : MonoBehaviour {
    

   


    public bool active = false;
    public string TAG = "TEMP";
    public enum AXIS {
        X=0,
        Y=1,
        Z=2
    }


    const int OSCILLATION_ANGLE = 80;
    const int OSCILLATION_SPEED = 4;
    const int PERLIN_NOISE = 20;


    float currDeltaRotation;
    float currRotation;
    float currPerlinNoise=0f;


    float left;
    float right;
	// Use this for initialization
	void Start () {
        left = 360 - OSCILLATION_ANGLE;
        right = OSCILLATION_ANGLE;
	}
	// Update is called once per frame
	void Update () {
        if (!active) return;

    }

   

    public void rotate(AXIS axis)
    {
        currDeltaRotation = Time.time * OSCILLATION_SPEED;
        float t = Mathf.Sin(currDeltaRotation) * OSCILLATION_ANGLE;
        if (t < 0) t = 360 + t;    
        Vector3 currRotation = transform.eulerAngles;
        switch (axis)
        {
            case AXIS.X:
                currRotation.x = t;
                break;
            case AXIS.Y:
                currRotation.y = t;
                break;
            case AXIS.Z:
                currRotation.z = t;
                break;
        }
        transform.eulerAngles = currRotation;
    }

    public void perlinNoise(AXIS axis)
    {
        currPerlinNoise = Mathf.PerlinNoise(currPerlinNoise, Time.time);
        float s = currPerlinNoise-.5f;
        s *= PERLIN_NOISE;
        Vector3 currRotation = transform.eulerAngles;
        switch (axis)
        {
            case AXIS.X:
                currRotation.x = s;
                break;
            case AXIS.Y:
                currRotation.y = s;
                break;
            case AXIS.Z:
                currRotation.z = s;
                break;
        }
        transform.eulerAngles = currRotation;

    }

    public abstract string getName();
    
}
