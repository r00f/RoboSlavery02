using UnityEngine;
using System.Collections;

public class Locomotion
{
    private Animator m_Animator = null;
    
    int m_SpeedId = 0;
    int m_AgularSpeedId = 0;
    int m_DirectionId = 0;
    int m_IdleId = 0;
    int m_LocomotionId;
    int m_LocomotionPivotRId;
    int m_LocomotionPivotLId;


    public float m_SpeedDampTime = 0.1f;
    public float m_AnguarSpeedDampTime = 0.25f;
    public float m_DirectionResponseTime = 0.2f;


    
    public Locomotion(Animator animator)
    {
        m_Animator = animator;
        m_SpeedId = Animator.StringToHash("Speed");
        m_AgularSpeedId = Animator.StringToHash("Angle");
        m_DirectionId = Animator.StringToHash("Direction");
        m_IdleId = Animator.StringToHash("Base Layer.Idle");
        m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
        m_LocomotionPivotLId = Animator.StringToHash("Base Layer.LocomotionPivotL");
        m_LocomotionPivotRId = Animator.StringToHash("Base Layer.LocomotionPivotR");
    }

    public void Do(float speed, float direction)
    {
        AnimatorStateInfo state = m_Animator.GetCurrentAnimatorStateInfo(0);

        bool inTransition = m_Animator.IsInTransition(0);
        bool inIdle = state.fullPathHash == m_IdleId;
        bool inTurn = state.fullPathHash == m_LocomotionPivotLId || state.fullPathHash == m_LocomotionPivotRId;
        bool inWalkRun = state.fullPathHash == m_LocomotionId;

        float speedDampTime = inIdle ? 0 : m_SpeedDampTime;
        float angularSpeedDampTime = inWalkRun || inTransition ? m_AnguarSpeedDampTime : 0;
        float directionDampTime = inTurn || inTransition ? 1000000 : 0;

        float angularSpeed = direction / m_DirectionResponseTime;
        
        m_Animator.SetFloat(m_SpeedId, speed, speedDampTime, Time.deltaTime);
        m_Animator.SetFloat(m_AgularSpeedId, angularSpeed, angularSpeedDampTime, Time.deltaTime);
        m_Animator.SetFloat(m_DirectionId, direction, directionDampTime, Time.deltaTime);
    }	
}
