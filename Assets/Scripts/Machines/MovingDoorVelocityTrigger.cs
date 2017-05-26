using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDoorVelocityTrigger : MonoBehaviour {

    MovingDoorLogic movingDoor;

	// Use this for initialization
	void Start () {

        movingDoor = GetComponentInParent<MovingDoorLogic>();

	}

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<LivingEntity>())
        {
            movingDoor.SetYVelocity();
        }

    }
	
}
