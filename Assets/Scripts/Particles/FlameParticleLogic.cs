using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameParticleLogic : MonoBehaviour {

    Rigidbody rigid;
    [SerializeField]
    Vector3 rigidVelocity;
    ParticleSystem ps;
    PlayerLogic player;

	// Use this for initialization
	void Start () {

        ps = GetComponent<ParticleSystem>();
        rigid = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<PlayerLogic>();



    }

    // Update is called once per frame
    void Update()
    {
        ParticleSystem.ForceOverLifetimeModule folt = ps.forceOverLifetime;
        ParticleSystem.MainModule main = ps.main;
        folt.xMultiplier = -rigidVelocity.x;
        folt.zMultiplier = -rigidVelocity.z;


        if (player)
        {
            if (player.IsInLocomotion())
            {
                folt.yMultiplier = 0;
                main.startLifetimeMultiplier = 0.7f;
            }
            else
            {
                main.startLifetimeMultiplier = 1f;
                folt.yMultiplier = 2;
            }

        }

    }


    void FixedUpdate () {

        rigidVelocity = rigid.velocity;

    }
}
