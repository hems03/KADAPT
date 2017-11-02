using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MarkovModel : MonoBehaviour {
    //Accord.Statistics.Models.Markov.HiddenMarkovModel hmm = new Accord.Statistics.Models.Markov.HiddenMarkovModel(3, 2);
    double[][] inputSequences =
    {
        //Need to train on real-time data then classify
        new double[] {0.0,280.0,288.0,311.2,344.7,21.4,50.7,73.0,79.9,70.0,45.2,10.9},
        new double[] {0.0,1.1,1.8,1.9,1.4,0.4,359.3,357.3,356.5,355.6,354.9,354.6,354.8}  
    };
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
