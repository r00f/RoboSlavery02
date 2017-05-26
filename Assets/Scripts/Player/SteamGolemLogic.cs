using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SteamGolemLogic : PlayerLogic
{

    #region SerializeFields

    [SerializeField]
    public GameObject holoTorso;
    [SerializeField]
    float hoverForce;
    [SerializeField]
    GameObject explosion;
    [SerializeField]
    GameObject explosionBig;
    [SerializeField]
    Material body02Mat;
    [SerializeField]
    float overheatDOT = 2;
    [SerializeField]
    float overheatSpeedMultiplier = 2;
    [SerializeField]
    float overheatLerpDuration = 5;
    [SerializeField]
    Color overheatColor;
    [SerializeField]
    List<GameObject> leftArms = new List<GameObject>();
    [SerializeField]
    List<GameObject> rightArms = new List<GameObject>();
    [SerializeField]
    GameObject flameBeamPrefab;
    [SerializeField]
    float repBeamTime;
    [SerializeField]
    float hoverTopSpeed = 10;
    [SerializeField]
    AudioSource fireLoopAudioSource;
    #endregion

    #region Private Variables
    public string ChainedAction;
    float curRepBeamTime;
    FlameImpLogic flameImp;
    bool overheatMode;
    Slider handLHealthBar;
    Slider handRHealthBar;
    List<ParticleSystem> footFlameParticleSystems = new List<ParticleSystem>();
    bool onetime;
    float t;

    //HashTags
    int m_ChargeId;
    int m_ChargeLoopId;
    int m_PunchL;
    int m_PunchR;
    int m_Spin;
    int m_HeavyPunchL;
    int m_HeavyPunchR;


    #endregion

    #region Mono Methods

    void Start()
    {
        Initialize();
        foreach(ParticleSystem ps in particleSystems)
        {
            if (ps.CompareTag("FootFlame"))
                footFlameParticleSystems.Add(ps);
        }
        flameImp = FindObjectOfType<FlameImpLogic>();
        healthBar = canvases[0].transform.GetChild(2).GetChild(0).GetComponent<Slider>();
        handLHealthBar = canvases[0].transform.GetChild(2).GetChild(1).GetComponent<Slider>();
        handRHealthBar = canvases[0].transform.GetChild(2).GetChild(2).GetComponent<Slider>();
        //Hash IDs
        m_PunchL = Animator.StringToHash("Base Layer.PunchSequence.PunchL");
        m_PunchR = Animator.StringToHash("Base Layer.PunchSequence.PunchR");
        m_HeavyPunchL = Animator.StringToHash("Base Layer.PunchSequence.Punch2L");
        m_HeavyPunchR = Animator.StringToHash("Base Layer.PunchSequence.Punch2R");
        m_ChargeId = Animator.StringToHash("Base Layer.WhirlWind.WhirlWindCharge");
        m_Spin = Animator.StringToHash("Base Layer.WhirlWind.WhirlWindRelease");
        m_ChargeLoopId = Animator.StringToHash("Base Layer.WhirlWind.ChargeLoop");
        ChainedAction = "";
    }

    void Update()
    {
        //Update Animator / Call Die() if currentHealth is <= 0
        HandleVariables();
        //handle ColorLerping / Speedincrease if in overheat-Mode
        HandleOverheating();
        //update healthBar
        healthBar.value = 1 - currentHealth / maxHealth;
        //handle which arms are being displayed
        HandleArmDisplay();
        handLHealthBar.value = 1 - hitSpheres[0].GetComponent<HandController>().HealthPercentage();
        handRHealthBar.value = 1 - hitSpheres[1].GetComponent<HandController>().HealthPercentage();

        HandleEntititesInRange();

        if(IsDashOutTransition())
        {
            SwitchSteamParticles("Steam");
        }

        //make sure all Hit Spheres are disabled if we are not in one of the attack substates
        if (!animator.GetBool("Attacking") && IsHitSphereEnabled())
        {
            DisableAllHitSpheres();
        }

        //if the character is alive, handle Input
        if (!dead)
        {
            HandleInput();
        }
        //else set overHeatMode to false, start RestartScene coroutine and make ragdoll Spaz around if user presses Fire2
        else
        {
            overheatMode = false;
            if (Input.GetButtonDown("Fire2"))
            {
                int randInt = Random.Range(0, ragdollColliders.Length);
                ragdollColliders[randInt].GetComponent<Rigidbody>().AddExplosionForce(3000, ragdollColliders[randInt].transform.position - Vector3.up, 10);
            }

        }

    }

    void FixedUpdate()
    {
        if (!dead)
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

    #endregion

    #region Methods

    #region Statechecks

    public bool IsInChargeUp()
    {

        return baseStateInfo.fullPathHash == m_ChargeId || baseStateInfo.fullPathHash == m_ChargeLoopId;

    }

    public bool IsHitSphereEnabled()
    {
        if (hitSpheres.Length != 0)
            return hitSpheres[0].enabled || hitSpheres[1].enabled;
        else
            return false;
    }

    public bool IsOverheated()
    {
        return overheatMode;
    }

    public bool IsPunching()
    {
        return baseStateInfo.fullPathHash == m_PunchL || baseStateInfo.fullPathHash == m_PunchR;
    }

    public bool LeftPunch()
    {
        return baseStateInfo.fullPathHash == m_PunchL;
    }

    public bool RightPunch()
    {
        return baseStateInfo.fullPathHash == m_PunchR;
    }

    public bool IsHeavyPunch()
    {
        return baseStateInfo.fullPathHash == m_HeavyPunchL || baseStateInfo.fullPathHash == m_HeavyPunchR;
    }

    public bool IsSpinning()
    {
        return baseStateInfo.fullPathHash == m_Spin;
    }

    #endregion

    public override void HandleInput()
    {
        base.HandleInput();

        animator.SetBool("Grounded", grounded);
        animator.SetBool("OverheatMode", overheatMode);

        if (!grounded)
        {
            animator.SetFloat("YVelocity", rigid.velocity.y);
        }

        if (rePlayer.GetButton("R2"))
        {
            if(!animator.GetBool("Attacking") && (!hitSpheres[0].GetComponent<HandController>().IsDead() || !hitSpheres[1].GetComponent<HandController>().IsDead()))
            {
                animator.SetBool("Blocking", true);

                if (!overheatMode)
                    movementSpeed = 0.5f;
                else
                    movementSpeed = 0.5f * overheatSpeedMultiplier;
            }
            else
            {
                animator.SetBool("Blocking", false);
            }

            //animator.SetTrigger("Punch");
        }
        else
        {
            animator.SetBool("Blocking", false);
            if (!overheatMode)
                movementSpeed = 0.8f;
            else
                movementSpeed = 0.8f * overheatSpeedMultiplier;
        }

        if (LeftPunch())
        {
            if (IsHitSphereEnabled() && ChainedAction == "Explosion")
            {
                print("Instantiate Explosion at hitSpheres[0]");
                Instantiate(explosion, hitSpheres[0].transform.position, Quaternion.identity);
            }
            ChainedAction = "";
        }
        else if (RightPunch())
        {
            if (IsHitSphereEnabled() && ChainedAction == "Explosion")
            {
                print("Instantiate Explosion at hitSpheres[1]");
                Instantiate(explosion, hitSpheres[1].transform.position, Quaternion.identity);
            }
            ChainedAction = "";

        }
        if (IsHitSphereEnabled() && ChainedAction == "Big Bang")
        {
            print("Kaboom");
            Instantiate(explosionBig, hitSpheres[0].transform.position, Quaternion.identity);
            ChainedAction = "";
        }

    }

    public override void AddSubtractHealth(float healthAmount)
    {
        if(!animator.GetBool("Blocking"))
        { 
            base.AddSubtractHealth(healthAmount);
        }

        else
        {
            hitSpheres[0].GetComponent<HandController>().AddSubtractHealth(healthAmount/4);
            hitSpheres[1].GetComponent<HandController>().AddSubtractHealth(healthAmount/4);
        }
        //print("AddSubstract " + healthAmount + " health from golem");
    }

    public override void Die()
    {
        base.Die();
        if(overheatMode)
        {
            Instantiate(explosionBig, transform.position + new Vector3(0, .9f, 0), Quaternion.identity);
        }
    }

    void HandleOverheating()
    {
        //Change _EmissionColor to orange if in OverheatMode and back to black if not
        if (overheatMode)
        {
            AddSubtractHealth(Time.deltaTime * overheatDOT);

            if (!onetime)
            {
                t = 0;
                attackSpeed *= overheatSpeedMultiplier;
                movementSpeed *= overheatSpeedMultiplier;
                foreach (ParticleSystem p in particleSystems)
                {
                    if (p.CompareTag("Steam"))
                    {
                        ParticleSystem.MainModule main = p.main;
                        ParticleSystem.EmissionModule em = p.emission;
                        main.simulationSpeed = 6;
                        main.startSpeed = 0.5f;
                        main.startSizeMultiplier = 0.6f;
                        main.startLifetime = 2f;
                        em.rateOverTime = 10;
                    }

                }
                Instantiate(explosionBig, transform.position + new Vector3(0, .9f, 0), Quaternion.identity);
                onetime = true;

            }

            if (body02Mat.GetColor("_EmissionColor") != overheatColor)
            {
                //print("LERP Color to Orange");
                body02Mat.SetColor("_EmissionColor", Color.Lerp(body02Mat.GetColor("_EmissionColor"), overheatColor, t));

            }


            if (t < 1)
            { // while t below the end limit...
              // increment it at the desired rate every update:
                t += Time.deltaTime / overheatLerpDuration;
            }




        }

        else
        {
            if (onetime)
            {
                t = 0;
                attackSpeed /= overheatSpeedMultiplier;
                movementSpeed /= overheatSpeedMultiplier;
                SwitchSteamParticles("Steam");
                foreach (ParticleSystem p in particleSystems)
                {
                    if (p.CompareTag("Steam"))
                    {
                        ParticleSystem.MainModule main = p.main;
                        ParticleSystem.EmissionModule em = p.emission;
                        main.simulationSpeed = 4;
                        main.startSizeMultiplier = 0.4f;
                        main.startSpeed = .1f;
                        main.startLifetime = 8f;
                        em.rateOverTime = 2;
                    }

                }
                onetime = false;
            }

            if (body02Mat.GetColor("_EmissionColor") != Color.black)
            {
                //print("LERP Color to Black");
                body02Mat.SetColor("_EmissionColor", Color.Lerp(body02Mat.GetColor("_EmissionColor"), Color.black, t));
            }


            if (t < 1)
            { // while t below the end limit...
              // increment it at the desired rate every update:
                t += Time.deltaTime / overheatLerpDuration;
            }

        }

    }

    public void SwitchOverheat()
    {
        overheatMode = !overheatMode;
        RemoveEntityFromList(FindObjectOfType<FlameImpLogic>());
        FindObjectOfType<FlameImpLogic>().RemoveEntityFromList(FindObjectOfType<SteamGolemLogic>());
    }

    public void SwitchSteamParticles(string patricleTag)
    {
        foreach (ParticleSystem p in particleSystems)
        {

            if (p.CompareTag("AfterBurner") || p.CompareTag("Steam"))
            {

                if (p.CompareTag(patricleTag))
                {
                    ParticleSystem.EmissionModule em = p.emission;
                    em.enabled = true;
                }

                else
                {
                    ParticleSystem.EmissionModule em = p.emission;
                    em.enabled = false;
                }
            }
        }

    }

    public void HandleArmDisplay()
    {

        if(!IsOverheated() && !dead)
        {
            //if both arms are destoryed 
            if (!leftArms[0].gameObject.activeSelf && !rightArms[0].gameObject.activeSelf)
            {
                holoTorso.gameObject.SetActive(false);
                //if imp targets golem
                if (isTargetLocked)
                {
                    //if imp does not use Right stick HorizontalAxis display left HoloArm
                    if (FindObjectOfType<FlameImpLogic>().rePlayer.GetAxis("Right Horizontal") == 0f)
                    {
                        if (!leftArms[1].gameObject.activeSelf && !rightArms[1].gameObject.activeSelf)
                        {
                            leftArms[1].gameObject.SetActive(true);
                        }
                    }
                    //else Switch between HoloArms
                    else
                    {
                        if (FindObjectOfType<FlameImpLogic>().rePlayer.GetAxis("Right Horizontal") > 0.8f)
                        {
                            leftArms[1].gameObject.SetActive(false);
                            rightArms[1].gameObject.SetActive(true);
                        }
                        else if (FindObjectOfType<FlameImpLogic>().rePlayer.GetAxis("Right Horizontal") < -0.8f)
                        {
                            leftArms[1].gameObject.SetActive(true);
                            rightArms[1].gameObject.SetActive(false);
                        }

                    }

                }
                //else disable both holoarms
                else
                {
                    leftArms[1].gameObject.SetActive(false);
                    rightArms[1].gameObject.SetActive(false);
                }
            }

            //if only left arm is destroyed and Imp targets Golem, display HoloArm
            else if (!leftArms[0].gameObject.activeSelf)
            {
                if (isTargetLocked)
                    leftArms[1].gameObject.SetActive(true);
                else
                    leftArms[1].gameObject.SetActive(false);
            }

            //if only right arm is destroyed and Imp targets Golem, display HoloArm

            else if (!rightArms[0].gameObject.activeSelf)
            {
                holoTorso.gameObject.SetActive(false);
                if (isTargetLocked)
                    rightArms[1].gameObject.SetActive(true);
                else
                    rightArms[1].gameObject.SetActive(false);
            }
            //if no arm is destroyed and the torso is damaged, display HoloTorso
            else
            {
                holoTorso.gameObject.SetActive(false);
                if (currentHealth < maxHealth && isTargetLocked)
                    holoTorso.gameObject.SetActive(true);
                else
                    holoTorso.gameObject.SetActive(false);
            }
        }
        else
        {
            //disable both holoarms and torso
            leftArms[1].gameObject.SetActive(false);
            rightArms[1].gameObject.SetActive(false);
            holoTorso.gameObject.SetActive(false);
        }
    }


    public void RepairArm()
    {
        //if left HoloArm is active
       if(leftArms[1].gameObject.activeSelf)
        {
            curRepBeamTime -= Time.deltaTime * 30;
            if (curRepBeamTime <= 0)
            {
                GameObject repairBeam = Instantiate(flameBeamPrefab, flameImp.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                repairBeam.GetComponent<RepairBeamLogic>().target = leftArms[1].transform;
                curRepBeamTime = repBeamTime;
            }

            leftArms[1].GetComponent<HoloArmLogic>().RepairArm();
        }
       else if (rightArms[1].gameObject.activeSelf)
        {
            curRepBeamTime -= Time.deltaTime * 30;
            if (curRepBeamTime <= 0)
            {
                GameObject repairBeam = Instantiate(flameBeamPrefab, flameImp.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                repairBeam.GetComponent<RepairBeamLogic>().target = rightArms[1].transform;
                curRepBeamTime = repBeamTime;
            }

            rightArms[1].GetComponent<HoloArmLogic>().RepairArm();
        }
       else if(holoTorso.activeSelf)
        {
            curRepBeamTime -= Time.deltaTime * 30;
            if (curRepBeamTime <= 0)
            {
                GameObject repairBeam = Instantiate(flameBeamPrefab, flameImp.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                repairBeam.GetComponent<RepairBeamLogic>().target = holoTorso.transform.GetChild(0);
                curRepBeamTime = repBeamTime;
            }
            RepairTorso();
        }
    }

    public void RepairTorso()
    {
        if (currentHealth < maxHealth && gameController)
        {
            if (gameController.GetMetalAmount() > 0)
            {
                gameController.AddSubstractMetal(-Time.deltaTime * 20);
                AddSubtractHealth(Time.deltaTime * 20);
            }
        }

    }

    public float MaxHealth()
    {
        return maxHealth;
    }

    public float CurrentRepair()
    {
        return currentHealth;
    }

    public float CurrentRepairCost()
    {
        return maxHealth - currentHealth;
    }

    public void Dash()
    {
        SwitchSteamParticles("AfterBurner");
        animator.SetTrigger("Dash");
        PlaySFX("Dash", 1.2f, 1.22f);
    }

    public void Hover()
    {
        if(!grounded)
        {
            rigid.AddForce(new Vector3(transform.forward.x * hoverForce / 3, hoverForce, transform.forward.z * hoverForce / 3) * Time.deltaTime);

            if (rigid.velocity.magnitude > hoverTopSpeed)
                rigid.velocity = rigid.velocity.normalized * hoverTopSpeed;

        }

    }

    public void Jump()
    {
        if (grounded)
        {
            // jump!
            animator.applyRootMotion = false;
            rigid.velocity = new Vector3(0, jumpPower, 0);
            StartCoroutine(PropellForward());
        }

    }

    public void EmitFootFlameParticles(bool emit)
    {
        foreach (ParticleSystem ps in footFlameParticleSystems)
        {
            ParticleSystem.EmissionModule em = ps.emission;
            em.enabled = emit;
        }
    }

    IEnumerator PropellForward()
    {
        yield return new WaitForSeconds(0.05f);
        rigid.AddForce(transform.forward * 10000);
    }

    public void PlayFireLoop(bool play)
    {
        if(play)
        {
            if (!fireLoopAudioSource.isPlaying)
            {
                fireLoopAudioSource.time = Random.Range(0f, 1f);
                fireLoopAudioSource.Play();
            }
        }
        else
            fireLoopAudioSource.Stop();
    }

    #endregion
}
