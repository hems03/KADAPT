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

    int bufferSize;


    const int OSCILLATION_ANGLE = 40;
    const int OSCILLATION_SPEED = 4;
    const int PERLIN_NOISE = 30;

    const double PERLIN_A = 1.35;
    double PERLIN_B;


    float currDeltaRotation;
    float currRotation;
    float currPerlinNoise=0f;


    float left;
    float right;
	// Use this for initialization
	void Start () {
        PERLIN_B= Random.Range(0f, 1f);
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
        float input = (float)PERLIN_A * currPerlinNoise + (float)PERLIN_B;
        currPerlinNoise = Mathf.PerlinNoise(input,Time.time);
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
