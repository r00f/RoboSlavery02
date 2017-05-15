using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlameImpLogic : PlayerLogic {

    [SerializeField]
    GameObject projectile;
    [SerializeField]
    GameObject impProjectile;
    [SerializeField]
    Renderer[] renderers;
    [SerializeField]
    MeshRenderer[] eyes;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Light pointLight;
    Collider[] colliders;
    GameObject carryProjectile;
    SteamGolemLogic steamGolem;
    float pointLightIntensity;
    float pointLightFlickerTime;

    public bool fused;
    public bool launched;

    int m_DashId;
    int m_IdleDashTransId;
    int m_LocoDashTransId;

    void Start () {

        Initialize();
        healthBar = canvases[1].transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        colliders = GetComponentsInChildren<Collider>();
        pointLight = GetComponentInChildren<Light>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        renderers = GetComponentsInChildren<Renderer>();
        pointLightIntensity = pointLight.intensity;
        steamGolem = FindObjectOfType<SteamGolemLogic>();

    }
	
	void Update () {

        //Update Animator / Call Die() if currentHealth is <= 0
        HandleVariables();

        HandleEntititesInRange();

        if(!dead)
        {
            HandleInput();
        }

        healthBar.value = currentHealth / maxHealth;
            
        if(pointLightFlickerTime == 0)
         pointLight.intensity = currentHealth / maxHealth * pointLightIntensity + Random.Range(-0.2f, 0.2f);

    }

    void FixedUpdate()
    {

        if (fused)
        {
            if (rePlayer.GetButtonDown("Bottom Button") && steamGolem.IsInChargeUp())
            {
                steamGolem.Jump();
            }
        }

            if (!dead)
        {
            if (pointLightFlickerTime >= Random.Range(3,8))
            {
                pointLightFlickerTime++;

            }
            else
            {
                pointLightFlickerTime = 0;
            }
            if(fused)
            {
                rigid.position = steamGolem.GetComponent<Rigidbody>().position;
                rigid.MoveRotation(steamGolem.transform.rotation);

                if(pointLight.shadowStrength == 1)
                {
                    pointLight.shadowStrength = 0;

                }
            }
            else if (launched)
            {
                rigid.position = carryProjectile.transform.position;
            }
            else
            {
                if (!IsTargeting() && IsInLocomotion() && ((direction >= 0 && horizontalL >= 0) || (direction < 0 && horizontalL < 0)))
                {
                    //print("usualRotationLogic");
                    Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (horizontalL < 0f ? -1f : 1f)), Mathf.Abs(horizontalL));
                    Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
                    rigid.MoveRotation(rigid.rotation * deltaRotation);
                }

                if (IsTargeting())
                {
                    //print("targetingRotationLogic");
                    var localTarget = transform.InverseTransformPoint(lookAtXForm);

                    float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

                    Vector3 eulerAngleVelocity = new Vector3(0, angle, 0);
                    Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime * StrafeRotateSpeed);
                    rigid.MoveRotation(rigid.rotation * deltaRotation);
                }

            }

        }

    }

    public override void Initialize()
    {
        base.Initialize();
        m_IdleDashTransId = Animator.StringToHash("Base Layer.Idle -> Base Layer.Dash");
        m_LocoDashTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.Dash");
        m_DashId = Animator.StringToHash("Base Layer.Dash");

    }

    public override void Die()
    {
        base.Die();

        skinnedMeshRenderer.enabled = false;
        foreach(MeshRenderer rend in eyes)
        {
            rend.enabled = false;

        }
    }

    public override void HandleInput()
    {

        if(rePlayer.GetButton("Right Button"))
        {
            steamGolem.RepairArm();
        }



        if (fused)
        {
            animator.SetFloat("Angle", 0f); animator.SetFloat("Direction", 0f);
            //Handle Fused Input
            if(rePlayer.GetButtonDown("Bottom Button") && steamGolem.IsInChargeUp())
            {
                steamGolem.Jump();
            }

            if(steamGolem.IsHitSphereEnabled())
            {
                if (rePlayer.GetButtonDown("Bottom Button"))
                {
                    FireImp();
                }
            }

            if (rePlayer.GetButtonDown("Left Button"))
            {
                steamGolem.Dash();
            }
        }
        else
        {
            base.HandleInput();
        }
    }

    public void FireProjectile()
    {

        Instantiate(projectile, transform.position + new Vector3(transform.forward.x, 1, transform.forward.z), transform.rotation);

    }

    public void FireImp()
    {
        steamGolem.SwitchOverheat();
        GameObject go = Instantiate(impProjectile, transform.position + new Vector3(transform.forward.x, 1, transform.forward.z), transform.rotation);
        go.GetComponent<ProjectileLogic>().isLaunchedImp = true;
        carryProjectile = go;
        fused = false;
        launched = true;
    }

    public bool IsDashing()
    {
        return stateInfo.fullPathHash == m_DashId || transInfo.fullPathHash == m_IdleDashTransId || transInfo.fullPathHash == m_LocoDashTransId;
    }

    public bool IsFused()
    {
        return fused;
    }

    public void SwitchColliders()
    {

        foreach(Collider col in colliders)
        {
            if(col.name != "TorsoMesh")
                col.enabled = !col.enabled;
        }

        rigid.isKinematic = !rigid.isKinematic;

    }

    public void SwitchRenderers()
    {
        foreach(Renderer renderer in renderers)
        {
            renderer.enabled = !renderer.enabled;
        }
    }

}

