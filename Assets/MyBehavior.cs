using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;


public class MyBehavior : MonoBehaviour {
    public GameObject participant;
    public GameObject victim;
    private BehaviorAgent behaviorAgent;
    // Use this for initialization
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected Node ST_ApproachAndWait(Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);
        return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
    }

    protected Node BuildTreeRoot()
    {
        return new DecoratorLoop(
                    this.ParticipantShootRay()   
                );
    }

  
    protected Node ParticipantShootRay()
    {
        Val<Vector3> pos = Val.V(() => victim.transform.position);
  
        return new Sequence(this.ST_ApproachAndWait(victim.transform),
                    this.ST_ApproachAndWait(victim.transform),
                    this.ST_ApproachAndWait(victim.transform)
                             );
    }

    protected Node AssertRayShot()
    {
        return new DecoratorLoop(
            new Sequence(
                    new DecoratorInvert(
                        new DecoratorLoop(
                            new Sequence(this.WaitForShot())
                        )
                    ),
                   new LeafWait(500), KillVictim())
        );

    }

    protected Node WaitForShot()
    {
        return new LeafAssert(() => participant.GetComponent<Animator>().GetBool("H_Clap"));
    }

    protected Node KillVictim()
    {
        Vector3 victimPosition = victim.transform.position;

        victimPosition.y = victimPosition.y - 10;
        return new Sequence(new LeafInvoke(() => { }));
    }
}
