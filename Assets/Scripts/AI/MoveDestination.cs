using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveDestination : MonoBehaviour {

    public Transform goal;
    NavMeshAgent agent;
    [SerializeField]
    bool inHitRange;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    void Update() {
        
        if(!inHitRange)
        {
            agent.speed = 3;
            agent.angularSpeed = 120;
            agent.acceleration = 8;
            agent.destination = goal.position;
        }


        else
        {
            agent.acceleration = 0;
            agent.speed = 0;
            agent.angularSpeed = 0;
            agent.destination = transform.position;

        }
            

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerHitRangeTrigger"))
        {
            inHitRange = true;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHitRangeTrigger"))
        {
            inHitRange = false;
        }

    }


}
