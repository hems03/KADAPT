using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A : RotationPoll {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!active) return;
        rotate(AXIS.Y);
        perlinNoise(AXIS.X);
	}
}
