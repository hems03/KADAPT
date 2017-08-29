using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;



public class WanderBehavior : MonoBehaviour {
    public Transform wander1;
    public Transform wander2;
    public Transform wander3;
    public GameObject participant;
    public GameObject victim;
    

    private BehaviorAgent behaviorAgent;
    // Use this for initialization
    void Start () {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    protected Node ST_ApproachAndFace(GameObject target)
    {
        Val<Vector3> targPosition = Val.V(() => target.transform.Find("TargetLocation").transform.position);
        Val<Vector3> actualPosition = Val.V(() => target.transform.position);
        return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_GoTo(targPosition),
                            new LeafWait(250),
                            participant.GetComponent<BehaviorMecanim>().Node_OrientTowards(actualPosition),
                            new LeafWait(1000)
                            );
    }

    protected Node BuildTreeRoot() {
        return (
            new DecoratorLoop(
                 new SequenceParallel(
                    this.ParticipantShootFire(),
                    this.AssertRayShot()
                    )
                )
               
            );
    }

    protected Node ParticipantShootFire()
    {

        return new Sequence(this.ST_ApproachAndFace(victim),
                            new LeafInvoke(()=> { participant.GetComponent<CustomProps>().isShooting = true; }),
                            participant.GetComponent<BehaviorMecanim>().ST_PlayFaceGesture("FIREBREATH", 1000)
                             );
    }

    protected Node AssertRayShot()
    {
        return new DecoratorLoop(
            new Sequence(
                new DecoratorLoop(this.AssertRayShot()),
                this.DuckVictim(victim)
            )
        );

    }
    protected Node WaitForShot()
    {
        Val<bool> isShot = Val.V(() => participant.GetComponent<CustomProps>().isShooting);
        return new LeafAssert(() => {
            return isShot.Value;
        });
    }

    protected Node DuckVictim(GameObject victim)
    {

        return victim.GetComponent<BehaviorMecanim>().ST_PlayBodyGesture("DUCK",1000);
    }
}
