using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampController : MonoBehaviour {

    Rigidbody rb;
    float yVelocity;
    [SerializeField]
    AudioSource impactAudioSource;
    AudioSource audioSource;
    [SerializeField]
    AudioClip hydraulicsUpClip;
    [SerializeField]
    AudioClip hydraulicsDownClip;



    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        yVelocity = 0;
        StartCoroutine(StartMovingUp());
        rb = GetComponent<Rigidbody>();
	}
	
    void FixedUpdate()
    {
        rb.velocity = Vector3.up * yVelocity;
    }

    public void MoveUp()
    {
        audioSource.PlayOneShot(hydraulicsUpClip);
        yVelocity = 1;
    }

    public void MoveDown()
    {
        audioSource.PlayOneShot(hydraulicsDownClip);
        yVelocity = -15;
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.contacts[0].normal.y > 0.8f)
        {
            impactAudioSource.PlayOneShot(impactAudioSource.clip);
            MoveUp();

            if(other.collider.GetComponent<LivingEntity>())
            {
                other.collider.GetComponent<LivingEntity>().AddSubtractHealth(-1000);
            }
        }
        else
        {
            MoveDown();
        }
    }

    IEnumerator StartMovingUp()
    {
        yield return new WaitForSeconds(Random.Range(0, 3));
        MoveUp();
    }
}
