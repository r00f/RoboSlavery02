using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairBeamLogic : MonoBehaviour {

    public Vector3 targetOffset;
    public Transform target;
    [SerializeField]
    float speed;

    void Start()
    {
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position + targetOffset, step);
    }
}
