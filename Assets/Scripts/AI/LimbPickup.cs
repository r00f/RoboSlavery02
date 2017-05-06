using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbPickup : MonoBehaviour {

    GameController gameController;

	// Use this for initialization
	void Start () {

        gameController = FindObjectOfType<GameController>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter (Collision other)
    {
        if (other.collider.GetComponent<FlameImpLogic>())
        {
            //other.collider.GetComponent<LivingEntity>().AddSubtractHealth(20f);
            gameController.AddMetal(100);
            Destroy(gameObject);
        }

    }
}
