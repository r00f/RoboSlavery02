using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HurtCapsuleTrigger : MonoBehaviour {


    LivingEntity livingEntity;
    [SerializeField]
    bool isPlayerHurtCapsule;
    [SerializeField]
    bool isGolemHurtCapsule;

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
