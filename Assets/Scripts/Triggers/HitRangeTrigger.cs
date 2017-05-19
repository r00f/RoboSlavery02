using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRangeTrigger : MonoBehaviour {

    Agent agent;

	void Start () {

        agent = GetComponentInParent<Agent>();

    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerLogic>())
        {
            if(!other.GetComponent<PlayerLogic>().IsDead() && !agent.GetHit())
            {
                agent.inHitRange = true;
            }
                
            else
                agent.inHitRange = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerLogic>())
        {
            agent.inHitRange = false;
        }
    }


}
