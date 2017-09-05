﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;

public class IncDec : MonoBehaviour {
    private BehaviorAgent behaviorAgent;
    private int num=0;
    // Use this for initialization
    void Start () {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    protected Node BuildTreeRoot()
    {
        return (
            new DecoratorLoop(
                new DecoratorInvert(                 
                    new DecoratorLoop(
                        new Sequence(
                            new LeafInvoke(() => Debug.Log(num)),
                            new LeafWait(1000),
                            new LeafInvoke(() => num++),
                            new DecoratorLoop(
                                new Sequence(
                                    new LeafAssert(() => Input.GetMouseButton(0)),
                                    new LeafInvoke(() => Debug.Log(num)),
                                    new LeafWait(1000),
                                    new LeafInvoke(() => num--)
                                )
                            )
                        )
                    )
              )
            
          )
                    
         );
    }

    // Update is called once per frame
    void Update () {
        
	}
}
