using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour {

    Rigidbody rigid;

    [SerializeField]
    float explosionForce;
    [SerializeField]
    float explosionRadius;
    [SerializeField]
    float explosionUp;
    [SerializeField]
    GameObject explosion;
    [SerializeField]
    GameObject explosionBig;
    public bool isLaunchedImp;
    FlameImpLogic flameImp;
    AudioSource audioSource;


    // Use this for initialization
    void Start () {

        flameImp = FindObjectOfType<FlameImpLogic>();
        rigid = GetComponent<Rigidbody>();
        rigid.AddExplosionForce(explosionForce, transform.position - transform.forward * 2, explosionRadius, explosionUp);
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(1f, 1.2f);
		
	}
	
    void OnCollisionEnter(Collision other)
    {
        if(isLaunchedImp)
        {
            if (other.collider.CompareTag("Floor") || other.collider.CompareTag("Enemy"))
            {
                Instantiate(explosionBig, transform.position, transform.rotation);
                flameImp.launched = false;
                flameImp.SwitchColliders();
                flameImp.SwitchRenderers();
                Destroy(gameObject);

            }
        }
        else
        {
            if (other.collider.CompareTag("Floor") || other.collider.CompareTag("Enemy"))
            {
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(gameObject);

            }

        }


    }


}
