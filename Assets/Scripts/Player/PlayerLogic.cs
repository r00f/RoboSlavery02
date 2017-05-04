﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLogic : LivingEntity
{

    #region Serialize Fields

    [SerializeField]
    protected int playerNumber;
    [SerializeField]
    protected float directionDampTime = .25f;
    [SerializeField]
    protected float speedDampTime = .05f;
    [SerializeField]
    protected float directionSpeed = 3f;
    [SerializeField]
    protected float rotationDegreePerSecond = 120f;
    [SerializeField]
    protected float StrafeRotateSpeed = 5;
    [SerializeField]
    protected Vector3 lookAtXForm;
    [SerializeField]
    protected ThirdPersonCamera gameCam;

    #endregion

    #region Private Variables

    protected float speed;
    protected float direction;
    protected float horizontalL;
    protected float verticalL;
    protected float charAngle;

    List<Agent> agentsInRange = new List<Agent>();
    List<Agent> agentsToRemove = new List<Agent>();

    #endregion

    #region Protected Variables

    protected Slider healthBar;

    #endregion

    #region Properties

    public float Speed
    {
        get
        {
            return this.speed;

        }

    }

    public float LocomotionThreshold
    {
        get
        {
            return 0.2f;

        }

    }

    public float HorizontalL
    {
        get
        {
            return horizontalL;

        }

    }

    public float VerticalL
    {
        get
        {
            return verticalL;

        }

    }

    #endregion

    #region Mono Methods

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FixedCamTrigger"))
        {
            gameCam.fixedCamPos = other.transform.GetChild(0).transform.position;
            gameCam.characterInCamTrigger = true;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FixedCamTrigger"))
        {
            gameCam.characterInCamTrigger = false;
        }

    }

    #endregion

    #region Methods

    public override void Initialize()
    {
        base.Initialize();
        gameObject.tag = "Player" + playerNumber;
        gameCam = GameObject.FindGameObjectWithTag("Cam" + playerNumber).GetComponent<ThirdPersonCamera>();
    }

    public virtual void HandleInput()
    {
        if (playerNumber == 1)
        {
            horizontalL = Input.GetAxis("Horizontal");
            verticalL = Input.GetAxis("Vertical");
            animator.SetFloat("Horizontal", horizontalL);
            animator.SetFloat("Vertical", verticalL);

            if (Input.GetButtonDown("L1"))
            {
                animator.SetTrigger("L1");
            }

            if (Input.GetButtonDown("R1"))
            {
                animator.SetTrigger("R1");
            }

            if (Input.GetButtonDown("Fire2"))
            {
                AddSubtractHealth(-25);
                hitSpheres[Random.Range(0, 2)].GetComponent<HandController>().AddSubtractHealth(-25);
            }

            if (Input.GetButton("Fire3"))
            {
                animator.SetBool("SquarePressed", true);
            }

            else
            {
                animator.SetBool("SquarePressed", false);
            }
        }
        else
        {
            horizontalL = Input.GetAxis("HorizontalK");
            verticalL = Input.GetAxis("VerticalK");
            animator.SetFloat("Horizontal", horizontalL);
            animator.SetFloat("Vertical", verticalL);

            if (Input.GetButtonDown("Fire1K"))
            {
                animator.SetTrigger("Punch");
            }

            if (Input.GetButtonDown("Fire2K"))
            {
                AddSubtractHealth(-25);
            }

            if (Input.GetButton("Fire3K"))
            {
                animator.SetBool("SquarePressed", true);
            }

            else
            {
                animator.SetBool("SquarePressed", false);
            }

        }


        animator.SetBool("Strafe", IsTargeting());
        charAngle = 0f;
        direction = 0f;

        StickToWorldSpace(this.transform, gameCam.transform, ref direction, ref speed, ref charAngle, IsInPivot());

        animator.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
        animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);

        if (speed > LocomotionThreshold)
        {
            if (!IsInPivot())
            {
                animator.SetFloat("Angle", charAngle);
            }

        }

        //if we are switching to LocomotionPivot, set direction to 0
        if (charAngle >= 135 || charAngle <= -135)
        {
            animator.SetFloat("Direction", 0f);
            animator.SetFloat("Speed", 1f);
        }

        if (speed < LocomotionThreshold && Mathf.Abs(horizontalL) < 0.05f)
        {
            animator.SetFloat("Direction", 0f);
            animator.SetFloat("Angle", 0f);
        }

    }

    protected void HandleEnemiesInRange()
    {
        //if enemiesInRage is not empty, set lookAtXForm to nearest enemy
        if (agentsInRange.Count != 0)
        {
            if (IsTargeting())
            {
                for (int i = 0; i < agentsInRange.Count; i++)
                {
                    if (i == 0)
                    {
                        agentsInRange[i].isTargeted = true;
                        agentsInRange[i].isTargetLocked = true;
                    }

                    else
                    {
                        agentsInRange[i].isTargeted = false;
                        agentsInRange[i].isTargetLocked = false;
                    }

                }

            }
            else
            {


                for (int i = 0; i < agentsInRange.Count; i++)
                {
                    if (i == 0)
                    {
                        agentsInRange[i].isTargeted = true;
                    }

                    else
                    {
                        agentsInRange[i].isTargeted = false;
                        agentsInRange[i].isTargetLocked = false;
                    }

                }

                //sort enemiesInRange by distance
                agentsInRange.Sort(delegate (Agent c1, Agent c2) {
                    return Vector3.SqrMagnitude(this.transform.position - c1.transform.position).CompareTo
                                (Vector3.SqrMagnitude(this.transform.position - c2.transform.position));
                });

            }

            lookAtXForm = agentsInRange[0].transform.position;

        }
        //else set lookAtXForm to players forward
        else
        {
            lookAtXForm = transform.position + transform.forward;
        }

        //foreach Agent in agentsInRange check if he is dead and if it is, add it to agentsToRemove
        foreach (Agent agent in agentsInRange)
        {

            if (agent.IsDead())
            {
                RemoveAgentFromInRangeList(agent);
            }

        }

        //foreach Agent in agentsToRemove remove it from agentsInRange
        foreach (Agent removeAgent in agentsToRemove)
        {
            removeAgent.isTargeted = false;
            agentsInRange.Remove(removeAgent);
        }

    }

    public void StickToWorldSpace(Transform root, Transform camera, ref float directionOut, ref float speedOut, ref float angleOut, bool isPivoting)
    {
        Vector3 rootDirection = root.forward;
        Vector3 stickDirection = new Vector3(horizontalL, 0, verticalL);
        speedOut = stickDirection.sqrMagnitude;

        // Get camera rotation
        Vector3 cameraDirection = camera.forward;
        cameraDirection.y = 0.0f;
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);

        //Convert joystick input to Worldspace Coordinates
        Vector3 moveDirection = referentialShift * stickDirection;
        Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

        //Debug Rays
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), axisSign, Color.red);

        float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);

        if (!isPivoting)
        {
            angleOut = angleRootToMove;
        }

        angleRootToMove /= 180f;
        directionOut = angleRootToMove * directionSpeed;


    }

    public bool IsTargeting()
    {
        return gameCam.camState == ThirdPersonCamera.CamStates.Target;
    }

    public bool IsInPivot()
    {

        return
            stateInfo.fullPathHash == m_LocomotionPivotRId ||
            stateInfo.fullPathHash == m_LocomotionPivotLId ||
            transInfo.fullPathHash == m_LocomotionPivotRTransId ||
            transInfo.fullPathHash == m_LocomotionPivotLTransId;
    }

    public void RemoveAgentFromInRangeList(Agent agent)
    {
        agentsToRemove.Add(agent);
    }

    public void AddAgentToInRangeList(Agent agent)
    {
        agentsInRange.Add(agent);
        agentsToRemove.Clear();
    }

    public bool IsInLocomotion()
    {
        return stateInfo.fullPathHash == m_LocomotionId;
    }

    public override void Die()
    {
        base.Die();
        gameController.RestartScene(5f);
    }

    #endregion

}
