using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;



public class BehaviorTree : MonoBehaviour
{

    public GameObject participant;
    public GameObject victim;
    public GameObject victim1;
    public GameObject victim2;

    static GameObject currentVictim = null;
    Val<GameObject> currVictim = Val.V(() => currentVictim);

    private BehaviorAgent behaviorAgent;

    private bool isLockedOn = false;
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

    protected Node ST_ApproachAndFace(Val<GameObject> target)
    {
        Val<Vector3> targPosition = Val.V(() => target.Value.transform.Find("TargetPosition").transform.position);
        Val<Vector3> actualPosition = Val.V(() => target.Value.transform.position);
        return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_GoTo(targPosition),

                            participant.GetComponent<BehaviorMecanim>().Node_OrientTowards(actualPosition),
                            new LeafWait(1000)
                            );
    }

    protected Node BuildTreeRoot()
    {
        return (
            new DecoratorLoop(
                 new SequenceParallel(
                    new DecoratorInvert(new DecoratorLoop(new DecoratorInvert(new SequenceParallel(CowerVictims(new List<GameObject> { victim, victim1, victim2 }))))),
                    new DecoratorInvert(new DecoratorLoop(this.AssertVictimChosen())),
                    new DecoratorInvert(new DecoratorLoop(this.AssertRayShot()))
                    )
                )

            );
    }

    protected Node ParticipantShootFire()
    {

        return new Sequence(
                            new LeafInvoke(() => Debug.Log(currVictim)),
                            this.ST_ApproachAndFace(currVictim),
                            new LeafInvoke(() => { participant.GetComponent<CustomProps>().isShooting = true; }),
                            participant.GetComponent<BehaviorMecanim>().ST_PlayFaceGesture("FIREBREATH", 1000),
                            new LeafInvoke(() => { participant.GetComponent<CustomProps>().isShooting = false; }
                            )
                            );
    }

    protected Node AssertRayShot()
    {

        return new DecoratorLoop(new Sequence(new DecoratorInvert(new DecoratorLoop(new DecoratorInvert(new Sequence(this.WaitForShot())))),
                                 new LeafWait(250),
                                 new LeafInvoke(() => Debug.Log(currVictim.Value)),
                                 new LeafInvoke(() =>
                                 {
                                     GameObject victim = currVictim.Value;
                                     victim.GetComponent<Animator>().SetTrigger("B_Dying");
                                     isLockedOn = false;

                                 })

                                )
                                );
    }

    protected Node AssertVictimChosen()
    {

        return new Sequence(new DecoratorInvert(new DecoratorLoop(new DecoratorInvert(new Sequence(this.WaitUntilVictimChosen())))),
                                 new LeafInvoke(() =>
                                 {
                                     Debug.Log("Victim chosen");
                                     Debug.Log(currentVictim);
                                 }),
                                 this.ParticipantShootFire()
                                )
                                ;
    }

    protected Node WaitUntilVictimChosen()
    {
        Val<bool> isFirst = Val.V(() => Input.GetKeyDown("1"));
        Val<bool> isSecond = Val.V(() => Input.GetKeyDown("2"));
        Val<bool> isThird = Val.V(() => Input.GetKeyDown("3"));
        Val<bool> isLockedOnVal = Val.V(() => isLockedOn);
        return new LeafAssert(() =>
        {
            isLockedOn = true;
            if (isFirst.Value)
            {
                currentVictim = victim;
            }
            else if (isSecond.Value)
            {
                currentVictim = victim1;

            }
            else if (isThird.Value)
            {
                currentVictim = victim2;
            }
            else
            {
                isLockedOn = false;
            }

            return isLockedOnVal.Value;
        });
    }

    protected Node AssertAssailantNear(GameObject targVictim)
    {
        return new DecoratorLoop(new Sequence(new DecoratorInvert(new DecoratorLoop(new DecoratorInvert(new Sequence(this.WaitUntilNear(targVictim))))),
                                 new LeafWait(250),
                                 targVictim.GetComponent<BehaviorMecanim>().ST_PlayBodyGesture("DUCK", 8000)
                                )
                                );
    }
    protected Node WaitForShot()
    {
        Val<bool> isShot = Val.V(() => participant.GetComponent<CustomProps>().isShooting);
        return new LeafAssert(() =>
        {
            Debug.Log(isShot.Value);
            return isShot.Value;
        });
    }

    protected Node WaitUntilNear(GameObject victim)
    {
        Val<bool> isNear = Val.V(() => { return (victim.transform.position - participant.transform.position).magnitude < 4; });
        return new LeafAssert(() =>
        {
            return isNear.Value;
        });
    }



    protected Node KillVictim(GameObject victim)
    {
        return victim.GetComponent<BehaviorMecanim>().ST_PlayBodyGesture("DUCK", 10000);
    }


    protected Node CowerVictims(List<GameObject> victims)
    {
        SequenceParallel seq = new SequenceParallel();
        List<Node> cowers = new List<Node>();

        for (int i = 0; i < victims.Count; i++)
        {
            cowers.Add(this.AssertAssailantNear(victims[i]));
        }
        seq.Children = cowers;
        return seq;
    }
}
