using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineTrigger : MonoBehaviour {

    public  Machine ReferenceMachine { get; set; }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerStay(Collider other)
    {

        if (other.GetComponent<FlameImpLogic>())
        {
            if (other.GetComponent<FlameImpLogic>().IsDashing() && (!other.GetComponent<FlameImpLogic>().fused || !other.GetComponent<FlameImpLogic>().controllingMachine) && !ReferenceMachine.isActive)
            {
                other.GetComponent<FlameImpLogic>().SwitchColliders();
                other.GetComponent<FlameImpLogic>().SwitchRenderers();
                //setactive call when camera logic is done
                other.GetComponent<FlameImpLogic>().ReferenceMachine = ReferenceMachine;
                other.GetComponent<FlameImpLogic>().controllingMachine = true;
                ReferenceMachine.Activate();

            }
        }

    }
}
