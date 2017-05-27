using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDoorLogic : MonoBehaviour {

    Rigidbody rigid;
    Machine machine;
    public float yVelocity;

    void Start()
    {
        machine = GetComponentInParent<Machine>();
        rigid = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Floor") && machine.isActive == false)
        {
            rigid.isKinematic = true;
        }

        if (other.collider.GetComponent<LivingEntity>())
        {
            if (other.contacts[0].normal.y > 0.8f)
            {
                print("Door hits LivingEntity from above with " + yVelocity + " Y Velocity.");
                other.collider.GetComponent<LivingEntity>().AddSubtractHealth(20 * yVelocity);
            }

        }
    }


    void OnCollisionExit(Collision other)
    {
        if (other.collider.GetComponent<LivingEntity>())
        {
            other.collider.GetComponent<LivingEntity>().inDoorCollider = false;
            //other.collider.GetComponent<Rigidbody>().AddForce();
        }
    }

    void OnCollisionStay(Collision other)
    {

        if (other.collider.GetComponent<LivingEntity>())
        {
            if (other.contacts[0].normal.y <= 0.8f)
            {
                print(other.contacts[0].normal);
                other.collider.GetComponent<LivingEntity>().inDoorCollider = true;
                other.collider.GetComponent<Rigidbody>().AddForce(-50 * other.contacts[0].normal, ForceMode.Acceleration);

            }
            else
            {


            }

        }

    }

    public void SetYVelocity()
    {
        yVelocity = rigid.velocity.y;
    }

    }
