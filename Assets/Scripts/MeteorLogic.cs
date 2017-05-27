using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorLogic : MonoBehaviour {

    [SerializeField]
    GameObject explsionPrefab;
    [SerializeField]
    GameObject lavaBall;
    [SerializeField]
    GameObject dialougeTrigger;
    Rigidbody rigid;
    [SerializeField]
    float maxHealth = 3;
    [SerializeField]
    float currentHealth;
    [SerializeField]
    bool dead;
    ParticleSystem ps;
    FlameImpLogic flameImp;
    Collider col;
    List<Rigidbody> rockRigidBodies = new List<Rigidbody>();
    List<Collider>rockColliders = new List<Collider>();

    // Use this for initialization
    void Start () {

        foreach (Collider c in transform.GetChild(0).GetComponentsInChildren<Collider>())
        {
            rockColliders.Add(c);
        }

        foreach (Rigidbody r in transform.GetChild(0).GetComponentsInChildren<Rigidbody>())
        {
            rockRigidBodies.Add(r);
        }

        foreach (Collider c in rockColliders)
        {
            c.enabled = false;
        }

        col = GetComponent<Collider>();
        flameImp = FindObjectOfType<FlameImpLogic>();
        ps = GetComponentInChildren<ParticleSystem>();
        rigid = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {

        if(currentHealth <= 0 && !dead)
        {
            rigid.isKinematic = true;
            col.enabled = false;
            foreach (Collider c in rockColliders)
            {
                c.enabled = true;
            }
            foreach (Rigidbody r in rockRigidBodies)
            {
                r.isKinematic = false;
            }
            flameImp.inMeteor = false;
            flameImp.LaunchImp();
            Destroy(lavaBall);
            Destroy(dialougeTrigger);
            Instantiate(explsionPrefab, transform.position, Quaternion.identity);
            dead = true;
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

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitBox"))
        {
            SubtractHealth(1);
            Instantiate(explsionPrefab, other.transform.position, Quaternion.identity);

            if(other.GetComponent<HandController>())
            {
                other.GetComponent<HandController>().AddSubtractHealth(-50);
            }
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
