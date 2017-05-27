using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorLogic : MonoBehaviour {

    [SerializeField]
    GameObject explsionPrefab;
    Rigidbody rigid;
    [SerializeField]
    float maxHealth = 30;
    [SerializeField]
    float currentHealth;
    [SerializeField]
    bool dead;
    ParticleSystem ps;
    FlameImpLogic flameImp;


	// Use this for initialization
	void Start () {

        flameImp = FindObjectOfType<FlameImpLogic>();
        ps = GetComponentInChildren<ParticleSystem>();
        rigid = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
		
	}
	
	// Update is called once per frame
	void Update () {

        if(currentHealth <= 0 && !dead)
        {
            flameImp.inMeteor = false;
            flameImp.LaunchImp();
            dead = true;
            Destroy(gameObject);
        }
		

	}

    void OnCollisionEnter(Collision other)
    {
        if(other.collider.CompareTag("Floor"))
        {
            ParticleSystem.EmissionModule em = ps.emission;
            em.enabled = false;
            Instantiate(explsionPrefab, transform.position, Quaternion.identity);
        }

        else if (other.collider.CompareTag("PlayerHitBox"))
        {
            SubtractHealth(50);
            Instantiate(explsionPrefab, other.transform.position, Quaternion.identity);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitBox"))
        {
            SubtractHealth(50);
            Instantiate(explsionPrefab, other.transform.position, Quaternion.identity);
        }

    }


    public void SubtractHealth(float healthAmount)
    {

        //if the Meteor is alive, deal damage / heal for lifeAmount
        if (!dead)
        {

         currentHealth -= healthAmount;

        }

    }
}
