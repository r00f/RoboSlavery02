using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnKinematicOff : MonoBehaviour {

    Rigidbody rigid;
    Machine machine;

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

    }
}
