using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B : RotationPoll {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!active) return;
        rotate(AXIS.X);
        perlinNoise(AXIS.Y);
	}
    override public string getName()
    {
        return "B";
    }
}
