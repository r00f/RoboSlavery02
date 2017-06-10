using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour {

    [SerializeField]
    int handForce = 1000;
    [SerializeField]
    GameObject explosion;

    ParticleSystem[] particleSystems;
    ParticleSystem sparkPs;
    ParticleSystem drillPs;
    ParticleSystem drillPs2;
    ParticleSystem drillBuildUpPs;
    ParticleSystem flameThrowerPs;
    public int drillstage = 1;
    SteamGolemLogic player;
    bool dead;

    [SerializeField]
    protected int maxHealth = 100;
    [SerializeField]
    protected float currentHealth;

    float healthPercentage;

    #region Mono Methods

    void Start () {

        particleSystems = GetComponentsInChildren<ParticleSystem>();
        //if this hand belongs to a Player
        if (GetComponentInParent<PlayerLogic>())
        {
            player = GetComponentInParent<SteamGolemLogic>();
        }
        RestoreHealthToFull();
        sparkPs = transform.GetChild(0).GetComponent<ParticleSystem>();
        drillPs = transform.GetChild(1).GetComponent<ParticleSystem>();
        drillPs2 = transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
        drillBuildUpPs = transform.GetChild(2).GetComponent<ParticleSystem>();
        flameThrowerPs = transform.GetChild(3).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }

        healthPercentage = currentHealth / maxHealth;

    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && transform.GetComponentInParent<PlayerLogic>())
        {

            EmitSparks(0.2f);
            if (other.GetComponent<Rigidbody>() && other.GetComponent<Agent>())
            {
                if(other.GetComponent<Agent>().GetHit() && !other.GetComponent<Agent>().knockedUp)
                {
                    other.GetComponent<Agent>().knockedUp = true;
                    other.GetComponent<Rigidbody>().AddExplosionForce(handForce, transform.position, 1, 0.6f, ForceMode.Impulse);
                    print("PushBAck");
                }

            }

        }
    }

    #endregion

    #region Methods

    void Die()
    {
        if (!dead)
        {
            Instantiate(explosion, transform.parent.position, transform.parent.rotation);
            transform.parent.gameObject.SetActive(false);
            StopAllEmission();
            dead = true;
        }

    }

    public void AddSubtractHealth(float healthAmount)
    {
        //if the LivingEntity is alive, deal damage / heal for lifeAmount
        if (!dead)
        {
            //if we want to heal, check if currentHealth is below maxHealth and then heal
            if (healthAmount > 0)
            {
                if (currentHealth < maxHealth)
                    currentHealth += healthAmount;

                else
                    currentHealth = maxHealth;
            }

            //if we want to deal damage, deal damage
            else
            {
                currentHealth += healthAmount;
            }

        }

    }

    public void EmitSparks(float emissionTime)
    {
        if(sparkPs && !dead)
        {
            ParticleSystem.EmissionModule em = sparkPs.emission;
            em.enabled = true;
            StartCoroutine(StopEmission(emissionTime, em, em));
        }

    }

    public void EmitDrill(float emissionTime, int stage)
    {
        if(drillPs && drillPs2 && !dead)
        {
            ParticleSystem.EmissionModule em2 = drillPs2.emission;
            ParticleSystem.EmissionModule em = drillPs.emission;
            em2.enabled = true;
            em.enabled = true;
            var main = drillPs.main;
            if(stage == 1)
                main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 1f);
            else if(stage == 2)
                main.startLifetime = new ParticleSystem.MinMaxCurve(0.6f , 1.5f);
            StartCoroutine(StopEmission(emissionTime, em, em2));
        }

    }

    public void EmitDrillBuildup(float emissionTime)
    {
        if (drillBuildUpPs && !dead)
        {
            drillstage = 1;
            ParticleSystem.EmissionModule em = drillBuildUpPs.emission;
            em.enabled = true;
            StartCoroutine(EmitDrillBuildupStage1(emissionTime));
            StartCoroutine(StopEmission(emissionTime, em, em));
        }

    }

    public void EmitFlameThrower(bool emission)
    {
        if (flameThrowerPs && !dead)
        {
            ParticleSystem.EmissionModule em = flameThrowerPs.emission;
            em.enabled = emission;
        }

    }



    public float HealthPercentage()
    {
        return healthPercentage;
    }

    public bool IsDead()
    {
        return dead;
    }

    IEnumerator EmitDrillBuildupStage1(float waitTime)
    {
        
        yield return new WaitForSeconds(waitTime);
        if(player.IsInChargeUp())
        {
            StartCoroutine(EmitDrillBuildupStage2(1f));
            EmitDrill(.9f, drillstage);
        }


    }

    IEnumerator EmitDrillBuildupStage2(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (player.IsInChargeUp())
        {
            drillstage = 2;
            EmitDrill(5f, drillstage);
        }
    }

    public void SetDead(bool deadBool)
    {
        dead = deadBool;
    }

    public void RestoreHealthToFull()
    {
        currentHealth = maxHealth;
    }

    IEnumerator StopEmission(float stopTime, ParticleSystem.EmissionModule emissionModule, ParticleSystem.EmissionModule emissionModule2)
    {

        yield return new WaitForSeconds(stopTime);
        emissionModule.enabled = false;
        emissionModule2.enabled = false;

    }

    void StopAllEmission()
    {
        foreach(ParticleSystem p in particleSystems)
        {
            ParticleSystem.EmissionModule em = p.emission;
            em.enabled = false;
        }

    }

    #endregion
}
