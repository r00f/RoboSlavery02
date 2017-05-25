using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnFloorHit : MonoBehaviour {

    [SerializeField]
    GameObject explsionPrefab;
    Rigidbody rigid;

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody>();
		
	}
	
	// Update is called once per frame
	void Update () {
		

	}

    void OnCollisionEnter(Collision other)
    {
        if(other.collider.CompareTag("Floor"))
        {
            Instantiate(explsionPrefab, transform.position, Quaternion.identity);
            rigid.isKinematic = true;
        }

    }
}
