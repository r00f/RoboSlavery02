using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLogic : MonoBehaviour {

    Rigidbody rigid;

	// Use this for initialization
	void Start () {

        rigid = GetComponent<Rigidbody>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerHitBox"))
        {
            rigid.isKinematic = false;
            rigid.AddExplosionForce(1000, other.transform.position, 10);
        }

    }
}
