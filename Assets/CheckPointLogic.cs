using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointLogic : MonoBehaviour {

    GameController gameController;

	// Use this for initialization
	void Start () {

        gameController = FindObjectOfType<GameController>();
		
	}
	
    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerLogic>())
        {
            //OnTriggerEnter set Players spawnPosistion to transform.position
            gameController.SetSpawnPosition(transform.position);

        }

    }
}
