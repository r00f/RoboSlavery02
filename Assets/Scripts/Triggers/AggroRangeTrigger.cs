using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroRangeTrigger : MonoBehaviour {

    Agent agent;

    void Start()
    {

        agent = GetComponentInParent<Agent>();

    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerLogic>())
        {
            agent.inAggroRange = true;
            agent.SetGoal(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerLogic>())
        {
            agent.inAggroRange = false;
        }
    }

}
