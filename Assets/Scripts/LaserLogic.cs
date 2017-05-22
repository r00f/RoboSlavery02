using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLogic : MonoBehaviour {

    public Transform target;
    public Agent agent;

    // Update is called once per frame
    void Update () {

        if (target && agent.inHitRange)
        {
            transform.LookAt(target.GetChild(0).GetChild(0));
        }
    }
}
