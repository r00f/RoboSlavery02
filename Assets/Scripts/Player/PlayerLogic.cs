using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PlayerLogic : LivingEntity
{

    #region Serialize Fields

    [SerializeField]
    protected float jumpPower;
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

    #endregion

    #region Protected Variables

    protected Vector3 lookAtXForm;
    protected ThirdPersonCamera gameCam;
    public Player rePlayer;

    protected float speed;
    protected float direction;
    protected float horizontalL;
    protected float verticalL;
    protected float charAngle;

    [SerializeField]
    List<LivingEntity> entitiesInRange = new List<LivingEntity>();
    [SerializeField]
    List<LivingEntity> entitiesToRemove = new List<LivingEntity>();

    protected float groundCheckDistance = 0.2f;
    [SerializeField]
    protected bool grounded;
    protected Vector3 groundNormal;

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
        InstanciateTarget(playerNumber);
        rePlayer = ReInput.players.GetPlayer(playerNumber-1);
        gameObject.tag = "Player" + playerNumber;
        gameCam = GameObject.FindGameObjectWithTag("Cam" + playerNumber).GetComponent<ThirdPersonCamera>();
    }

    public virtual void HandleInput()
    {

        //check if golem is grounded
        CheckGroundStatus();

        //movement
        horizontalL = rePlayer.GetAxis("Left Horizontal");
        verticalL = rePlayer.GetAxis("Left Vertical");

        animator.SetFloat("Horizontal", horizontalL);
        animator.SetFloat("Vertical", verticalL);
        animator.SetFloat("L2", rePlayer.GetAxis("L2"));
        animator.SetFloat("R2", rePlayer.GetAxis("R2"));

        if(grounded)
        {
            animator.SetBool("Right Button", rePlayer.GetButton("Right Button"));
            animator.SetBool("Left Button", rePlayer.GetButton("Left Button"));
            animator.SetBool("Up Button", rePlayer.GetButton("Up Button"));
            animator.SetBool("Bottom Button", rePlayer.GetButton("Bottom Button"));
        }
        else
        {
            animator.SetBool("Right Button", false);
            animator.SetBool("Left Button", false);
            animator.SetBool("Up Button", false);
            animator.SetBool("Bottom Button", false);

        }




        //buttons
        if (rePlayer.GetButtonDown("Start"))
        {
            gameController.PauseGame();
        }
        if (rePlayer.GetAxis("L2") > 0.1f)
        {
            animator.SetBool("Strafe", true);
            gameCam.camState = ThirdPersonCamera.CamStates.Target;
        }
        else
        {
            animator.SetBool("Strafe", false);
            gameCam.camState = ThirdPersonCamera.CamStates.Behind;

        }

        if (rePlayer.GetButtonDown("L1"))
        {
            animator.SetTrigger("L1");
        }

        if (rePlayer.GetButtonDown("R1"))
        {
            animator.SetTrigger("R1");
        }

        if (rePlayer.GetButtonDown("Up Button"))
        {
            AddSubtractHealth(-25);

            if(hitSpheres.Length > 0)
                hitSpheres[Random.Range(0, 2)].GetComponent<HandController>().AddSubtractHealth(-25);
        }

        if(playerNumber == 2)

        {
            if (rePlayer.GetButtonDown("Bottom Button"))
            {
                animator.SetTrigger("Punch");
            }
        }


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

    protected void HandleEntititesInRange()
    {
        //if entitiesInRange is not empty, set lookAtXForm to nearest enemy
        if (entitiesInRange.Count != 0)
        {
            if (IsTargeting())
            {
                for (int i = 0; i < entitiesInRange.Count; i++)
                {
                    if (i == 0)
                    {
                        entitiesInRange[i].isTargeted = true;
                        entitiesInRange[i].isTargetLocked = true;
                    }

                    else
                    {
                        entitiesInRange[i].isTargeted = false;
                        entitiesInRange[i].isTargetLocked = false;
                    }

                }

            }
            else
            {


                for (int i = 0; i < entitiesInRange.Count; i++)
                {
                    if (i == 0)
                    {
                        entitiesInRange[i].isTargeted = true;
                        entitiesInRange[i].isTargetLocked = false;
                    }

                    else
                    {
                        entitiesInRange[i].isTargeted = false;
                        entitiesInRange[i].isTargetLocked = false;
                    }

                }

                //sort enemiesInRange by distance
                entitiesInRange.Sort(delegate (LivingEntity c1, LivingEntity c2) {
                    return Vector3.SqrMagnitude(this.transform.position - c1.transform.position).CompareTo
                                (Vector3.SqrMagnitude(this.transform.position - c2.transform.position));
                });

            }

            lookAtXForm = entitiesInRange[0].transform.position;

        }
        //else set lookAtXForm to players forward
        else
        {
            lookAtXForm = transform.position + transform.forward;
        }

        //foreach Agent in agentsInRange check if he is dead and if it is, add it to agentsToRemove
        foreach (LivingEntity entity in entitiesInRange)
        {

            if (entity.IsDead())
            {
                RemoveEntityFromList(entity);
            }

        }

        //foreach Agent in agentsToRemove remove it from agentsInRange
        foreach (LivingEntity entity in entitiesToRemove)
        {
            entity.isTargeted = false;
            entity.isTargeted = false;
            entitiesInRange.Remove(entity);
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

    public void RemoveEntityFromList(LivingEntity enitity)
    {
        entitiesToRemove.Add(enitity);
    }

    public void AddEntityToList(LivingEntity enitity)
    {
        entitiesInRange.Add(enitity);
        entitiesToRemove.Clear();
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

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
        {
            groundNormal = hitInfo.normal;
            grounded = true;
            animator.applyRootMotion = true;
        }
        else
        {
            grounded = false;
            groundNormal = Vector3.up;
            animator.applyRootMotion = false;
        }
    }

    #endregion

}
