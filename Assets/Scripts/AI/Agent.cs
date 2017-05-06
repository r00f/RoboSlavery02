using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Agent : LivingEntity {

	public GameObject			particle;
	public NavMeshAgent		agent;
    public bool inHitRange;
    public bool inAggroRange;

    protected Locomotion locomotion;
	protected Object particleClone;

    [SerializeField]
    Transform goal;

    [SerializeField]
    bool getHit;

    bool stopped;

    #region Mono Methods

    void Start () {

        Initialize();
        agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		locomotion = new Locomotion(animator);
		particleClone = null;
	}

    void Update()
    {
        if(!dead)
        {
            SetDestination();
            SetupAgentLocomotion();
            HandleVariables();
        }
        else if(!stopped)
        {
            //print("stopAgent");
            agent.Stop();
            stopped = true;
        }

    }

    #endregion

    #region Methods

    public override void Initialize()
    {
        base.Initialize();
        InstanciateTarget(0);
    }

    public void SetDestination()
	{
        

        if(inAggroRange)
        {
            if (!inHitRange && !dead)
            {
                agent.destination = goal.position;

                if (stopped)
                {
                    //print("resumeAgent");
                    agent.Resume();
                    stopped = false;
                }


            }

            else
            {
                if (!dead)
                    animator.SetTrigger("Punch");
            }

        }

        else if (!stopped)
        {
           //print("stopAgent");
            agent.Stop();
            stopped = true;
        }



    }

    protected void SetupAgentLocomotion()
	{
		if (AgentDone())
		{
			locomotion.Do(0, 0);
			if (particleClone != null)
			{
				GameObject.Destroy(particleClone);
				particleClone = null;
			}
		}
		else
		{
			float speed = agent.desiredVelocity.magnitude;

			Vector3 velocity = Quaternion.Inverse(transform.rotation) * agent.desiredVelocity;

			float angle = Mathf.Atan2(velocity.x, velocity.z) * 180.0f / 3.14159f;

            /*
            print("angle: " + angle);
            print("velocity: " + velocity);
            print("desiredVelocity: " + agent.desiredVelocity);
            */

			locomotion.Do(speed, angle);
		}
	}

    void OnAnimatorMove()
    {
        if (animator.deltaPosition / Time.deltaTime != Vector3.zero)
        {
            agent.velocity = animator.deltaPosition / Time.deltaTime;
            transform.rotation = animator.rootRotation;
        }
    }

	protected bool AgentDone()
	{
        return !agent.pathPending && AgentStopping();
	}

	protected bool AgentStopping()
	{
        return agent.remainingDistance <= agent.stoppingDistance;
	}

    public void SetGoal(Transform inGoal)
    {
        goal = inGoal;

    }

    #endregion


}