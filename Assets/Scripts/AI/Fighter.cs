using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Fighter : MonoBehaviour, IGoap {

    GameObject brute1;
    public Animator bAni;
    public Animator mAni;
    private float sTime = 0;
    public bool bJump = false;

	// Use this for initialization
	void Start () {
        brute1 = GameObject.Find("brute");
        bAni = brute1.GetComponent<Animator>();
        mAni = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        bJump = bAni.GetCurrentAnimatorStateInfo(0).IsName("Jump");
    }

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("isJump", (bJump != false) ));

        return worldData;
    }

    public abstract HashSet<KeyValuePair<string, object>> createGoalState();

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        // Yay we found a plan for our goal
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
    }

    public void actionsFinished()
    {
        // Everything is done, we completed our actions for this gool. Hooray!
        Debug.Log("<color=blue>Actions completed</color>");
    }

    public void planAborted(GoapAction aborter)
    {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public bool moveAgent(GoapAction nextAction)
    {
        // move towards the NextAction's target
        
        //float step = 0.5f * Time.deltaTime;
        mAni.SetFloat("Speed", 0.6f);
        
        if (Vector3.Distance(nextAction.target.transform.position, gameObject.transform.position) < 2) {
            // we are at the target location, we are done
            mAni.SetFloat("Speed", 0.0f);
            nextAction.setInRange(true);
            return true;
        } else
            return false;
    }
}
