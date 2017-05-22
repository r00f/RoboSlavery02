using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionForce : MonoBehaviour
{
    [SerializeField]
    float explosionForce = 4;
    [SerializeField]
    float explosionUp = 0.5f;
    [SerializeField]
    float radius = 1;

    private IEnumerator Start()
    {
        // wait one frame because some explosions instantiate debris which should then
        // be pushed by physics force
        yield return null;

        float r = radius;
        var cols = Physics.OverlapSphere(transform.position, r);
        var enemyRigidbodies = new List<Rigidbody>();
        var playerRigidbodies = new List<Rigidbody>();

        foreach (var col in cols)
        {
            if (col.attachedRigidbody != null && !enemyRigidbodies.Contains(col.attachedRigidbody))
            {
                if (!col.transform.GetComponentInParent<PlayerLogic>())
                    enemyRigidbodies.Add(col.attachedRigidbody);
                else
                    playerRigidbodies.Add(col.attachedRigidbody);
            }
        }

        foreach (var rb in enemyRigidbodies)
        {
            rb.AddExplosionForce(explosionForce / enemyRigidbodies.Count, transform.position, r, explosionUp, ForceMode.Impulse);
        }

        foreach (var rb in playerRigidbodies)
        {
            if(rb.transform.GetComponentInParent<PlayerLogic>().IsDead())
                rb.AddExplosionForce(explosionForce / playerRigidbodies.Count, transform.position, r, explosionUp, ForceMode.Impulse);
        }
    }
}
