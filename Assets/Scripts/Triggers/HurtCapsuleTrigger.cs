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
    PlayerLogic player;

    void Start()
    {
        
        livingEntity = GetComponentInParent<LivingEntity>();

        if(GetComponentInParent<PlayerLogic>())
        {
            player = GetComponentInParent<PlayerLogic>();
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
        if (other.CompareTag("FixedCamTrigger") && player)
        {
            player.gameCam.fixedCamPos = other.transform.GetChild(0).transform.position;
            player.gameCam.characterInCamTrigger = true;
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

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FixedCamTrigger") && player)
        {
            player.gameCam.characterInCamTrigger = false;
            if (isGolemHurtCapsule)
            {
                if (GetComponent<SteamGolemLogic>().fused)
                {
                    FindObjectOfType<FlameImpLogic>().gameCam.characterInCamTrigger = false;
                }
            }
        }

    }
}
