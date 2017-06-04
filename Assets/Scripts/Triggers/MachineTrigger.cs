using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class MachineTrigger : MonoBehaviour {

    public  Machine ReferenceMachine { get; set; }

    ConversationTrigger conversationTrigger;

	// Use this for initialization
	void Start () {

        conversationTrigger = GetComponent<ConversationTrigger>();

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
                if(conversationTrigger)
                {
                    if (DialogueLua.GetVariable("ArmsRepaired").AsInt >= 1)
                    {
                        DialogueLua.SetVariable("InFirstMachine", true);
                        conversationTrigger.enabled = true;
                    }

                }

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
