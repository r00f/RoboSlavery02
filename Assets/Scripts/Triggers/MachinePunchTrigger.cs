using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinePunchTrigger : MonoBehaviour {

    Machine referenceMachine;
    [SerializeField]
    GameObject explosion;
    [SerializeField]
    ParticleSystem particle;
    [SerializeField]
    int timesToHit;
    [SerializeField]
    int howManyThingsToRemove;
    bool oneshot = true;
      
	// Use this for initialization
	void Start () {
        referenceMachine = GetComponentInParent<Machine>();
        ParticleSystem.EmissionModule em = particle.emission;
        em.enabled = true;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider Other)
    {
       if(Other.tag == "PlayerHitBox")
        {
            if (timesToHit > 0)
            {
                timesToHit--;
            }
            else
            {
                if (referenceMachine != null && oneshot)
                {
                    referenceMachine.RemoveFromExitchain(howManyThingsToRemove);
                    Instantiate(explosion, transform.position, transform.rotation);
                    ParticleSystem.EmissionModule em = particle.emission;
                    em.enabled = false;
                    oneshot = false;
                }
            }
        }
    }
}
