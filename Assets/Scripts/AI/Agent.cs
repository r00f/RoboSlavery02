using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Agent : LivingEntity {

	public GameObject			particle;
	public NavMeshAgent		agent;
    public bool inHitRange;
    public bool inAggroRange;
    public bool knockedUp;
    [SerializeField]
    Transform laserBeamTransform;
    [SerializeField]
    GameObject laserBeamPrefab;

    protected Locomotion locomotion;

    [SerializeField]
    Transform goal;

    [SerializeField]
    bool getHit;

    #region Mono Methods

    void Start () {

        Initialize();
        agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		locomotion = new Locomotion(animator);
	}

    void Update()
    {
        if(!dead)
        {
            HandleVariables();

            if (GetHit())
            {
                grounded = false;
                animator.ResetTrigger("Punch");
                //print("AgentGetHit");
                rigid.isKinematic = false;
            }
            else if(grounded)
            {
                knockedUp = false;
                rigid.isKinematic = true;
                SetDestination();
                SetupAgentLocomotion();
            }

            else if(Time.timeScale == 0)
            {
                //print("stopAgent");
                if (agent.enabled)
                    agent.isStopped = true;
            }

        }
        else
        {
            //print("stopAgent");
            if(agent.enabled && !agent.isStopped)
                agent.isStopped = true;
        }

            agent.enabled = grounded;


    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            print("ParticleEnemyCollision");
            AddSubtractHealth(-2f);
        }
    }


    #endregion

    #region Methods

    public override void Die()
    {
        knockedUp = false;
        base.Die();
    }

    public override void Initialize()
    {
        base.Initialize();
        InstanciateTarget(0);
    }

    public void SetDestination()
	{
        if(agent.enabled)
        {
            if (inAggroRange)
            {
                if (!inHitRange && !dead)
                {
                    agent.destination = goal.position;

                    if (agent.isStopped)
                    {
                        print("resumeAgent");
                        agent.isStopped = false;
                    }


                }

                else
                {
                    if (!dead)
                    {
                        animator.SetTrigger("Punch");
                    }

                }

            }

            else
            {
                //print("stopAgent");
                if (!agent.isStopped)
                    agent.isStopped = true;
            }

        }

    }

    public void InstanciateLaserBeam()
    {
        GameObject laserGO = Instantiate(laserBeamPrefab, laserBeamTransform.position, laserBeamTransform.rotation, laserBeamTransform.parent);
        laserGO.GetComponent<LaserLogic>().target = goal;
        laserGO.GetComponent<LaserLogic>().agent = this;
    }

    protected void SetupAgentLocomotion()
	{
		if (AgentDone())
		{
			locomotion.Do(0, 0);
		}
		else
		{

			float speed = agent.desiredVelocity.magnitude;

			Vector3 velocity = Quaternion.Inverse(transform.rotation) * agent.desiredVelocity;

			float angle = Mathf.Atan2(velocity.x, velocity.z) * 180.0f / 3.14159f;

			locomotion.Do(speed, angle);
		}
	}

    void OnAnimatorMove()
    {
        if (animator.deltaPosition / Time.deltaTime != Vector3.zero && Time.deltaTime != 0)
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
        if (agent.enabled)
            return agent.remainingDistance <= agent.stoppingDistance;
        else
            return false;
	}

    public void SetGoal(Transform inGoal)
    {
        goal = inGoal;
    }

    #endregion
}