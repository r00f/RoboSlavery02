using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]

public class LivingEntity : MonoBehaviour {

    [SerializeField]
    protected int maxHealth = 100;
    [SerializeField]
    protected float currentHealth;
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected SphereCollider[] hitSpheres;
    [SerializeField]
    protected float movementSpeed = 1;
    [SerializeField]
    protected float attackSpeed = 1;
    [SerializeField]
    protected GameObject targetGO;
    //AudioClips
    [SerializeField]
    protected AudioClip[] footStepClips;
    [SerializeField]
    protected AudioClip[] getHitClips;
    [SerializeField]
    protected AudioClip[] drillClips;

    protected bool dead;
    protected AnimatorStateInfo stateInfo;
    protected AnimatorTransitionInfo transInfo;
    protected Collider[] ragdollColliders;
    protected ParticleSystem[] particleSystems;
    protected GameController gameController;
    protected Rigidbody rigid;
    protected Canvas canvas;
    protected AudioSource audioSource;

    public bool isTargeted;
    public bool isTargetLocked;

    protected int m_LocomotionId;
    protected int m_LocomotionPivotRId;
    protected int m_LocomotionPivotLId;
    protected int m_LocomotionPivotRTransId;
    protected int m_LocomotionPivotLTransId;

    #region Methods

    public virtual void AddSubtractHealth(float healthAmount)
    {

        //if the LivingEntity is alive, deal damage / heal for lifeAmount
        if (!dead)
        {
            //if we want to heal, check if currentHealth is below maxHealth and then heal
            if (healthAmount > 0)
            {
                
                if(currentHealth < maxHealth)
                    currentHealth += healthAmount;

                else
                    currentHealth = maxHealth;
            }

            //if we want to deal damage, deal damage
            else
            {
                
                currentHealth += healthAmount;
            }
                
            //if damage is higher than 10, set GetHit Trigger (hitStun)
            if (healthAmount <= -10)
            {
                PlaySFX("GetHit", 0.77f, 0.83f);
                animator.SetTrigger("GetHit");
                animator.SetFloat("GetHitRandom", Random.Range(0, 2));
            }

        }

    }

    public virtual void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas1").GetComponent<Canvas>();
        audioSource = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody>();
        gameController = FindObjectOfType<GameController>();
        animator = GetComponent<Animator>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();

        if(transform.GetChild(0).GetComponentInChildren<Collider>())
        {
            ragdollColliders = transform.GetChild(0).GetComponentsInChildren<Collider>();

            foreach (Collider col in ragdollColliders)
            {
                col.enabled = false;
                col.GetComponent<Rigidbody>().isKinematic = true;
            }

        }
            
        currentHealth = maxHealth;

        //Instanciate targetGOinstance in Canvas

        GameObject targetGOinstance = Instantiate<GameObject>(targetGO, transform.position, Quaternion.identity, canvas.transform);
        targetGOinstance.GetComponent<TargetLogic>().SetTarget(transform);

        //Hash IDs
        m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
        m_LocomotionPivotLId = Animator.StringToHash("Base Layer.LocomotionPivotL");
        m_LocomotionPivotRId = Animator.StringToHash("Base Layer.LocomotionPivotR");
        m_LocomotionPivotLTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotL");
        m_LocomotionPivotRTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotR");
    }

    public void HandleVariables()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        transInfo = animator.GetAnimatorTransitionInfo(0);
        animator.SetFloat("MovementSpeed", movementSpeed);
        animator.SetFloat("AttackSpeed", attackSpeed);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if (!dead)
        {
            //print("die");
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
       
            animator.enabled = false;

            foreach (Collider col in ragdollColliders)
            { 
                if(!col.CompareTag("PlayerHitBox") && !col.CompareTag("EnemyHitBox"))
                    col.enabled = true;

                col.GetComponent<Rigidbody>().isKinematic = false;
            }

            foreach (ParticleSystem p in particleSystems)
            {
                ParticleSystem.EmissionModule em = p.emission;
                em.enabled = false;
            }

            dead = true;
        }

    }

    public virtual void EnableHitSphere(string side)
    {
        if (side == "L")
        {
            hitSpheres[0].enabled = true;
            hitSpheres[0].GetComponent<HandController>().EmitDrill(.15f, 1);
            hitSpheres[0].GetComponent<HandController>().drillstage = 1;
            //flameimp timing 

        }

        else if (side == "R")
        {
            hitSpheres[1].enabled = true;
            hitSpheres[1].GetComponent<HandController>().EmitDrill(.15f, 1);
            hitSpheres[1].GetComponent<HandController>().drillstage = 1;
        }

        else if (side == "Both")
        {
            hitSpheres[1].enabled = true;
            hitSpheres[1].GetComponent<HandController>().EmitSparks(0.3f);
            hitSpheres[1].GetComponent<HandController>().EmitDrill(0.3f, hitSpheres[1].GetComponent<HandController>().drillstage);
            hitSpheres[0].enabled = true;
            hitSpheres[0].GetComponent<HandController>().EmitSparks(0.3f);
            hitSpheres[0].GetComponent<HandController>().EmitDrill(0.3f, hitSpheres[0].GetComponent<HandController>().drillstage);
        }

        PlaySFX("Drill", 0.95f, 1.05f);   
    }

    public void EmitDrillBuildup()
    {
        hitSpheres[0].GetComponent<HandController>().EmitDrillBuildup(1f);
        hitSpheres[1].GetComponent<HandController>().EmitDrillBuildup(1f);
    }

    public void DisableAllHitSpheres()
    {
        //print("DISABLEHITSPHERES");
        foreach(SphereCollider sphereCol in hitSpheres)
        {
           //print("Disable " + sphereCol);
            if(sphereCol.enabled)
                sphereCol.enabled = false;
        }
    }

    public bool IsDead()
    {
        return dead;
    }

    public void PlayFootstepSFX()
    {
        PlaySFX("FootStep", 0.5f, 0.51f);
    }

    public void PlaySFX(string sfxName, float pitchMin, float pitchMax)
    {
        audioSource.pitch = Random.Range(pitchMin, pitchMax);

        switch (sfxName)
        {
            case "FootStep":
                if(footStepClips.Length > 0)
                    audioSource.PlayOneShot(footStepClips[Random.Range(0, footStepClips.Length)], .05f);
                break;
            case "GetHit":
                if (getHitClips.Length > 0)
                    audioSource.PlayOneShot(getHitClips[Random.Range(0, getHitClips.Length)], .3f);
                break;
            case "Drill":
                if (drillClips.Length > 0)
                    audioSource.PlayOneShot(drillClips[Random.Range(0, drillClips.Length)], .3f);
                break;
        }
    }

    #endregion
}
