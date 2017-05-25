using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]

public class LivingEntity : MonoBehaviour {

    #region SerializeFields

    [SerializeField]
    public bool grounded;
    [SerializeField]
    protected float groundCheckDistance;
    [SerializeField]
    protected int maxHealth = 100;
    [SerializeField]
    protected float currentHealth;
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    public SphereCollider[] hitSpheres;
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
    protected AudioClip[] attackClips;
    [SerializeField]
    protected AudioClip[] dashClips;

    #endregion

    #region Protected Variables

    protected Vector3 groundNormal;

    protected bool dead;
    protected AnimatorStateInfo baseStateInfo;
    protected AnimatorStateInfo upperBodyStateInfo;
    protected AnimatorTransitionInfo baseTransInfo;
    protected AnimatorTransitionInfo upperBodyTransInfo;
    protected Collider[] ragdollColliders;
    protected ParticleSystem[] particleSystems;
    protected GameController gameController;
    protected Rigidbody rigid;
    protected List<Canvas> canvases = new List<Canvas>();
    protected AudioSource audioSource;
    protected AudioSource footStepAudioSource;

    public bool isTargeted;
    public bool isTargetLocked;

    protected int m_GetHitId;
    protected int m_NothingGetHitTransId;
    protected int m_GetHitNothingTransId;
    protected int m_LocomotionId;
    protected int m_LocomotionPivotRId;
    protected int m_LocomotionPivotLId;
    protected int m_LocomotionPivotRTransId;
    protected int m_LocomotionPivotLTransId;

    #endregion

    #region Methods

    public virtual void Initialize()
    {
        canvases.Add(GameObject.FindGameObjectWithTag("Canvas1").GetComponent<Canvas>());
        canvases.Add(GameObject.FindGameObjectWithTag("Canvas2").GetComponent<Canvas>());
        audioSource = GetComponent<AudioSource>();
        footStepAudioSource = GetComponentInChildren<AudioSource>();
        rigid = GetComponent<Rigidbody>();
        gameController = FindObjectOfType<GameController>();
        animator = GetComponent<Animator>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();

        if (transform.GetChild(0).GetComponentInChildren<Collider>())
        {
            ragdollColliders = transform.GetChild(0).GetComponentsInChildren<Collider>();

            foreach (Collider col in ragdollColliders)
            {
                col.enabled = false;
                col.GetComponent<Rigidbody>().isKinematic = true;
            }

        }

        currentHealth = maxHealth;

        //Hash IDs
        m_GetHitId = Animator.StringToHash("Upper Body.GetHit");
        m_NothingGetHitTransId = Animator.StringToHash("Upper Body.Nothing -> Upper Body.GetHit");
        m_GetHitNothingTransId = Animator.StringToHash("Upper Body.GetHit -> Upper Body.Nothing");
        m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
        m_LocomotionPivotLId = Animator.StringToHash("Base Layer.LocomotionPivotL");
        m_LocomotionPivotRId = Animator.StringToHash("Base Layer.LocomotionPivotR");
        m_LocomotionPivotLTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotL");
        m_LocomotionPivotRTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotR");
    }

    public void HandleVariables()
    {
        CheckGroundStatus();
        baseStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        baseTransInfo = animator.GetAnimatorTransitionInfo(0);

        if (animator.layerCount > 1)
        {
            upperBodyStateInfo = animator.GetCurrentAnimatorStateInfo(1);
            upperBodyTransInfo = animator.GetAnimatorTransitionInfo(1);
        }


        animator.SetFloat("MovementSpeed", movementSpeed);
        animator.SetFloat("AttackSpeed", attackSpeed);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

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
                PlaySFX("GetHit", 0.5f, 0.6f);
                animator.SetTrigger("GetHit");
                animator.SetFloat("GetHitRandom", Random.Range(0, 2));
            }

        }

    }

    public virtual void InstanciateTarget(int playerNumber)
    {

        switch (playerNumber)
        {
            case 0:
                //spawn enemy Targets in both Canvases
                int i = 0;
                foreach (Canvas canvas in canvases)
                {
                    i++;

                    GameObject enemytTargetGO = Instantiate<GameObject>(targetGO, transform.position, Quaternion.identity, canvas.transform);
                    enemytTargetGO.GetComponent<TargetLogic>().SetTarget(transform);
                    enemytTargetGO.GetComponent<TargetLogic>().canvasNumber = i;
                }
                break;

            case 1:
                //spawn player 1 Target in player 2 Canvas
                GameObject player1TargetGO = Instantiate<GameObject>(targetGO, transform.position, Quaternion.identity, canvases[1].transform);
                player1TargetGO.GetComponent<TargetLogic>().SetTarget(transform);
                player1TargetGO.GetComponent<TargetLogic>().canvasNumber = 2;
                break;

            case 2:
                //spawn player 2 Target in player 1 Canvas
                GameObject player2TargetGO = Instantiate<GameObject>(targetGO, transform.position, Quaternion.identity, canvases[0].transform);
                player2TargetGO.GetComponent<TargetLogic>().SetTarget(transform);
                player2TargetGO.GetComponent<TargetLogic>().canvasNumber = 1;
                break;

        }

    }

    public bool GetHit()
    {
        return upperBodyStateInfo.fullPathHash == m_GetHitId || upperBodyTransInfo.fullPathHash == m_NothingGetHitTransId;
    }

    public bool GetHitNothingTrans()
    {
        return upperBodyTransInfo.fullPathHash == m_GetHitNothingTransId;
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
        {
            if (hitInfo.transform.tag == "Floor")
            {
                groundNormal = hitInfo.normal;
                animator.applyRootMotion = true;
                grounded = true;

            }
        }
        else
        {
            groundNormal = Vector3.up;
            animator.applyRootMotion = false;
            grounded = false;
        }

    }

    public bool IsInLocomotion()
    {
        return baseStateInfo.fullPathHash == m_LocomotionId;
    }

    public virtual void Die()
    {
        if (!dead)
        {
            //print("die");
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            DisableAllHitSpheres();
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
        if(hitSpheres.Length > 0)
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
        PlaySFX("FootStep", 0.8f, 0.81f);
    }

    public void PlaySFX(string sfxName, float pitchMin, float pitchMax)
    {

        switch (sfxName)
        {
            case "FootStep":
                if(footStepClips.Length > 0)
                {
                    footStepAudioSource.pitch = Random.Range(pitchMin, pitchMax);
                    footStepAudioSource.PlayOneShot(footStepClips[Random.Range(0, footStepClips.Length)], .05f);
                }
                break;
            case "GetHit":
                if (getHitClips.Length > 0)
                {
                    audioSource.Stop();
                    audioSource.pitch = Random.Range(pitchMin, pitchMax);
                    audioSource.PlayOneShot(getHitClips[Random.Range(0, getHitClips.Length)], .3f);
                }
                break;
            case "Drill":
                if (attackClips.Length > 0)
                {
                    audioSource.Stop();
                    audioSource.pitch = Random.Range(pitchMin, pitchMax);
                    audioSource.PlayOneShot(attackClips[Random.Range(0, attackClips.Length)], .3f);
                }
                break;
            case "Dash":
                if (dashClips.Length > 0)
                {
                    audioSource.Stop();
                    audioSource.pitch = Random.Range(pitchMin, pitchMax);
                    audioSource.PlayOneShot(dashClips[Random.Range(0, dashClips.Length)], .5f);
                }
                break;
        }
    }

    #endregion
}
