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

    public Machine ReferenceMachine
    {
        get; set;
    }
    public bool inMeteor;
    public bool fused;
    public bool launched;
    public bool controllingMachine;

    void Start () {

        Initialize();
        healthBar = canvases[1].transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        colliders = GetComponentsInChildren<Collider>();
        pointLight = GetComponentInChildren<Light>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        renderers = GetComponentsInChildren<Renderer>();
        pointLightIntensity = pointLight.intensity;
        steamGolem = FindObjectOfType<SteamGolemLogic>();

        if(inMeteor)
        {
            SwitchColliders();
            SwitchRenderers();
        }

    }
	
	void Update () {

        //print(IsDashing());
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
            gameCam.camState = ThirdPersonCamera.CamStates.Behind;
            if (rePlayer.GetButtonDown("Bottom Button") && steamGolem.IsInChargeUp())
            {
                steamGolem.Jump();
            }

            if (!steamGolem.grounded)
            {
                if(rePlayer.GetButton("Bottom Button"))
                {
                    steamGolem.PlayFireLoop(true);
                    steamGolem.Hover();
                    steamGolem.EmitFootFlameParticles(true);
                }
                else
                {
                    steamGolem.PlayFireLoop(false);
                    steamGolem.EmitFootFlameParticles(false);
                }
            }
            else
            {
                steamGolem.EmitFootFlameParticles(false);
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
            else if (controllingMachine)
            {
                rigid.position = ReferenceMachine.transform.GetChild(0).transform.position;

            }
            else if (launched)
            {
                rigid.position = carryProjectile.transform.position;
            }
            else if(inMeteor)
            {   
                rigid.position = FindObjectOfType<MeteorLogic>().gameObject.transform.position - new Vector3(0, 1, 0);
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

        if (fused)
        {

            if (rePlayer.GetButtonDown("R2"))
            {
                steamGolem.Dash();
            }

            animator.SetFloat("Angle", 0f); animator.SetFloat("Direction", 0f);

            //Handle Fused Input

            if (steamGolem.IsPunching())
            {
                foreach (SphereCollider sphereCol in steamGolem.hitSpheres)
                {
                    sphereCol.GetComponent<HandController>().EmitFlameThrower(false);
                }

                if (rePlayer.GetButtonDown("Bottom Button"))
                {
                    steamGolem.ChainedAction = "Explosion";
                    //explosive punch - alternately release imp
                }

                if (rePlayer.GetButtonDown("R2"))
                {
                    //eject Imp
                    LaunchImp();
                    /*
                    transform.position = steamGolem.transform.position + steamGolem.transform.forward * 2;
                    transform.rotation = steamGolem.transform.rotation;
                    SwitchColliders();
                    SwitchRenderers();
                    */
                }

            }
            else if (steamGolem.IsHeavyPunch())
            {
                foreach (SphereCollider sphereCol in steamGolem.hitSpheres)
                {
                    sphereCol.GetComponent<HandController>().EmitFlameThrower(rePlayer.GetButton("Bottom Button"));
                }

                if (rePlayer.GetButton("Bottom Button"))
                {
                    steamGolem.PlayFireLoop(true);
                    steamGolem.SwitchSteamParticles("AfterBurner");
                }
                else
                {
                    steamGolem.PlayFireLoop(false);
                    steamGolem.SwitchSteamParticles("Steam");
                }

                /*
                if (rePlayer.GetButtonDown("Bottom Button"))
                {
                    steamGolem.ChainedAction = "Big Bang";
                }
                */
                if (rePlayer.GetButtonDown("R2"))
                {
                    LaunchImp();
                    /*
                    transform.position = steamGolem.transform.position + steamGolem.transform.forward * 2;
                    transform.rotation = steamGolem.transform.rotation;
                    SwitchColliders();
                    SwitchRenderers();
                    */
                }
            }
            else if (steamGolem.IsSpinning())
            {
                foreach (SphereCollider sphereCol in steamGolem.hitSpheres)
                {
                    sphereCol.GetComponent<HandController>().EmitFlameThrower(rePlayer.GetButton("Bottom Button"));
                }

                if (rePlayer.GetButton("Bottom Button"))
                {
                    steamGolem.PlayFireLoop(true);
                    steamGolem.SwitchSteamParticles("AfterBurner");
                }
                else
                {
                    steamGolem.PlayFireLoop(false);
                    steamGolem.SwitchSteamParticles("Steam");
                }
            }
            else
            {
                foreach (SphereCollider sphereCol in steamGolem.hitSpheres)
                {
                    sphereCol.GetComponent<HandController>().EmitFlameThrower(false);
                }
            }
        }
        else if (controllingMachine && ReferenceMachine != null)
        {
            //print("Im here");
            if (rePlayer.GetButtonDown("Left Button"))
            {

                ReferenceMachine.LeftButton();
            }
            if (rePlayer.GetButtonDown("Bottom Button"))
            {
                //print("Bottom pressed");
                ReferenceMachine.BottomButton();
            }
            if (rePlayer.GetButtonDown("Up Button"))
            {
                ReferenceMachine.TopButton();
            }
            if (rePlayer.GetButtonDown("Right Button"))
            {

                ReferenceMachine.RightButton();
            }
            if(rePlayer.GetButtonUp("Left Button")) {
                ReferenceMachine.LeftButtonRelease();
            }
            if (rePlayer.GetButtonUp("Right Button"))
            {
                ReferenceMachine.RightButtonRelease();
            }
            if (rePlayer.GetButtonUp("Bottom Button"))
            {
                ReferenceMachine.BottomButtonRelease();
            }
            if (rePlayer.GetButtonUp("Up Button"))
            {
                ReferenceMachine.TopButtonRelease();
            }
            ReferenceMachine.Axis_HR = rePlayer.GetAxis("Right Horizontal");
            ReferenceMachine.Axis_HL = rePlayer.GetAxis("Left Horizontal");
            ReferenceMachine.Axis_VR = rePlayer.GetAxis("Right Vertical");
            ReferenceMachine.Axis_VL = rePlayer.GetAxis("Left Vertical");
        }
        else
        {
            base.HandleInput();

            if (steamGolem.isTargetLocked)
            {
                if (rePlayer.GetButton("Right Button"))
                {
                    steamGolem.RepairArm();
                    steamGolem.PlayFireLoop(true);
                }
                else
                {
                    steamGolem.PlayFireLoop(false);
                }
            }
            else
            {
                steamGolem.PlayFireLoop(false);
            }

            if (rePlayer.GetButtonDown("R2"))
            {
                PlaySFX("Dash", 1.4f, 1.5f);
            }

        }
}

    public void FireProjectile()
    {

        Instantiate(projectile, transform.position + new Vector3(transform.forward.x, 1, transform.forward.z), transform.rotation);

    }

    public void LaunchImp()
    {
        if (fused) {
            steamGolem.SwitchOverheat();
        }
        GameObject go = Instantiate(impProjectile, transform.position + new Vector3(transform.forward.x, 1, transform.forward.z), transform.rotation);
        go.GetComponent<ProjectileLogic>().isLaunchedImp = true;
        carryProjectile = go;
        fused = false;
        launched = true;
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
    public void SwitchCamera()
    {
        gameCam.camState = ThirdPersonCamera.CamStates.Behind;
        gameCam.GetComponent<ThirdPersonCamera>().fixedCamPos = new Vector3 (0,0,0);
    }
    public void SwitchCamera(Vector3 inPos)
    {
        gameCam.GetComponent<ThirdPersonCamera>().fixedCamPos = inPos;
        gameCam.camState = ThirdPersonCamera.CamStates.Fixed;
    }
}

