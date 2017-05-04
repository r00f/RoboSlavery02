using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesInRangeTrigger : MonoBehaviour {

    PlayerLogic player;

	// Use this for initialization
	void Start () {

        player = GetComponentInParent<PlayerLogic>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            //print("enemy in range");

            //if an enemy enters this trigger, add it to the enemiesInRange List of the player
            if(other.GetComponent<Agent>())
                player.AddAgentToInRangeList(other.GetComponent<Agent>());
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //print("enemy in range");

            //if an enemy enters this trigger, add it to the enemiesInRange List of the player
            if (other.GetComponent<Agent>())
                player.RemoveAgentFromInRangeList(other.GetComponent<Agent>());
        }
    }
}
