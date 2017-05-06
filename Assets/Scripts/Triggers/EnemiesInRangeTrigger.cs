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

            if(other.GetComponent<LivingEntity>())
                player.AddEntityToList(other.GetComponent<LivingEntity>());

    }

    void OnTriggerExit(Collider other)
    {
            if (other.GetComponent<LivingEntity>())
                player.RemoveEntityFromList(other.GetComponent<LivingEntity>());

    }
}
