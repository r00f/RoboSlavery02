using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HurtCapsuleTrigger : MonoBehaviour {


    LivingEntity livingEntity;
    [SerializeField]
    bool isPlayerHurtCapsule;
    [SerializeField]
    bool isGolemHurtCapsule;
    [SerializeField]
    GameObject explosion;

    void Start()
    {

        livingEntity = GetComponentInParent<LivingEntity>();

        if(GetComponentInParent<PlayerLogic>())
        {
            isPlayerHurtCapsule = true;

            if (GetComponentInParent<SteamGolemLogic>())
            {
                isGolemHurtCapsule = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitBox") && !isPlayerHurtCapsule)
        {
            livingEntity.AddSubtractHealth(-10);
        }
        if (other.CompareTag("EnemyHitBox") && isPlayerHurtCapsule)
        {
            livingEntity.AddSubtractHealth(-10);
        }

        if (other.GetComponent<FlameImpLogic>() && isGolemHurtCapsule)
        {
            if (other.GetComponent<FlameImpLogic>().IsDashing())
            {
                Instantiate(explosion, transform.position + new Vector3(0, .9f, 0), Quaternion.identity);
            }
        }

    }

    void OnTriggerStay(Collider other)
    {

        if (other.GetComponent<FlameImpLogic>() && isGolemHurtCapsule)
        {
            if (other.GetComponent<FlameImpLogic>().IsDashing() && !other.GetComponent<FlameImpLogic>().fused)
            {
                other.GetComponent<FlameImpLogic>().SwitchColliders();
                other.GetComponent<FlameImpLogic>().SwitchRenderers();
                GetComponentInParent<SteamGolemLogic>().SwitchOverheat();
                other.GetComponent<FlameImpLogic>().fused = true;
            }
        }

    }

    void OnCollisionEnter(Collision other)
    {

    }

}
