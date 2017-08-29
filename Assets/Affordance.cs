using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
public class RayAffordance:MonoBehaviour
{
    public GameObject participant;
    public GameObject victim;
    

    protected Node ParticipantShootRay()
    {
        Val<Vector3> pos = victim.transform.position;
        return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_GoTo(pos),
                             new LeafWait(1000),
                             participant.GetComponent<BehaviorMecanim>().Node_HeadLook(pos)
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
