using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonLogic : MonoBehaviour {

    [SerializeField]
    int poisonDot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<LivingEntity>())
        {
            other.GetComponent<LivingEntity>().AddSubtractHealth(poisonDot * Time.deltaTime);

        }
    }
}
