﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SteamGolemLogic : PlayerLogic
{

    #region SerializeFields

    [SerializeField]
    bool overheatMode;
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

    #endregion

    #region Private Variables

    Slider handLHealthBar;
    Slider handRHealthBar;

    bool onetime;
    float t;

    //HashTags
    int m_ChargeId;
    int m_ChargeLoopId;

    #endregion

    #region Mono Methods

    void Start()
    {
        Initialize();
        healthBar = canvases[0].transform.GetChild(2).GetChild(0).GetComponent<Slider>();
        handLHealthBar = canvases[0].transform.GetChild(2).GetChild(1).GetComponent<Slider>();
        handRHealthBar = canvases[0].transform.GetChild(2).GetChild(2).GetComponent<Slider>();
        //Hash IDs
        m_ChargeId = Animator.StringToHash("Base Layer.WhirlWind.WhirlWindCharge");
        m_ChargeLoopId = Animator.StringToHash("Base Layer.WhirlWind.ChargeLoop");
    }

    void Update()
    {
        //Update Animator / Call Die() if currentHealth is <= 0
        HandleVariables();
        //handle ColorLerping / Speedincrease if in overheat-Mode
        HandleOverheating();
        //update healthBar
        healthBar.value = currentHealth / maxHealth;
        handLHealthBar.value = hitSpheres[0].GetComponent<HandController>().HealthPercentage();
        handRHealthBar.value = hitSpheres[1].GetComponent<HandController>().HealthPercentage();

        HandleEntititesInRange();

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

    public override void HandleInput()
    {
        base.HandleInput();

        if (rePlayer.GetButton("Right Button"))
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
                        ParticleSystem.EmissionModule em = p.emission;
                        em.enabled = false;
                    }

                    else if (p.CompareTag("AfterBurner"))
                    {
                        ParticleSystem.EmissionModule em = p.emission;
                        em.enabled = true;
                    }

                }

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
                foreach (ParticleSystem p in particleSystems)
                {
                    if (p.CompareTag("Steam"))
                    {
                        ParticleSystem.EmissionModule em = p.emission;
                        em.enabled = true;
                    }
                    else if (p.CompareTag("AfterBurner"))
                    {
                        ParticleSystem.EmissionModule em = p.emission;
                        em.enabled = false;
                    }

                }

                movementSpeed /= overheatSpeedMultiplier;
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

    public bool IsInChargeUp()
    {
        return stateInfo.fullPathHash == m_ChargeId || stateInfo.fullPathHash == m_ChargeLoopId;
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

    public void SwitchOverheat()
    {
        overheatMode = !overheatMode;
    }

    #endregion

}
