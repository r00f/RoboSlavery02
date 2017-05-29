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
    public ThirdPersonCamera gameCam;
    public Player rePlayer;
    public bool fused;

    protected float speed;
    protected float direction;
    protected float horizontalL;
    protected float verticalL;
    protected float charAngle;

    [SerializeField]
    List<LivingEntity> entitiesInRange = new List<LivingEntity>();
    [SerializeField]
    List<LivingEntity> entitiesToRemove = new List<LivingEntity>();

    //Hash IDs
    int m_DashId;
    int m_IdleDashTransId;
    int m_LocoDashTransId;
    int m_DashIdleTransId;
    int m_DashLocoTransId;

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

    void OnParticleCollision(GameObject other)
    {
        if(other.CompareTag("Enemy"))
        {
            print("ParticlePlayerCollision");
            AddSubtractHealth(-2f);
        }
    }

    #endregion

    #region Methods

    public override void Initialize()
    {
        base.Initialize();
        m_IdleDashTransId = Animator.StringToHash("Base Layer.Idle -> Base Layer.Dash");
        m_LocoDashTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.Dash");
        m_DashIdleTransId = Animator.StringToHash("Base Layer.Dash -> Base Layer.Idle");
        m_DashLocoTransId = Animator.StringToHash("Base Layer.Dash -> Base Layer.Locomotion");
        m_DashId = Animator.StringToHash("Base Layer.Dash");
        InstanciateTarget(playerNumber);
        rePlayer = ReInput.players.GetPlayer(playerNumber-1);
        gameObject.tag = "Player" + playerNumber;
        gameCam = GameObject.FindGameObjectWithTag("Cam" + playerNumber).GetComponent<ThirdPersonCamera>();
    }

    public virtual void HandleInput()
    {
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
            animator.SetBool("L1", rePlayer.GetButton("L1"));
            animator.SetBool("R1", rePlayer.GetButton("R1"));
        }
        else
        {
            animator.SetBool("Right Button", false);
            animator.SetBool("Left Button", false);
            animator.SetBool("Up Button", false);
            animator.SetBool("Bottom Button", false);
            animator.SetBool("L1", false);
            animator.SetBool("R1", false);
        }

        //buttons
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
            baseStateInfo.fullPathHash == m_LocomotionPivotRId ||
            baseStateInfo.fullPathHash == m_LocomotionPivotLId ||
            baseTransInfo.fullPathHash == m_LocomotionPivotRTransId ||
            baseTransInfo.fullPathHash == m_LocomotionPivotLTransId;
    }

    public bool IsDashing()
    {
        return baseStateInfo.fullPathHash == m_DashId || baseTransInfo.fullPathHash == m_IdleDashTransId || baseTransInfo.fullPathHash == m_LocoDashTransId;
    }

    public bool IsDashOutTransition()
    {
        return baseTransInfo.fullPathHash == m_DashIdleTransId || baseTransInfo.fullPathHash == m_DashLocoTransId;
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

    public override void Die()
    {
        base.Die();
        gameController.RestartScene(5f);
    }

    #endregion

}
