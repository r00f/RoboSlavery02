using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class HoloArmLogic : MonoBehaviour {

    public float repairCost = 100;
    float currentRepairCost;
    float currentRepair;
    GameController gameController;
    [SerializeField]
    GameObject realArm;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        currentRepairCost = repairCost;
    }

    public void RepairArm()
    {
        if (currentRepair < repairCost && gameController)
        {
            if(gameController.GetMetalAmount() > 0)
            {
                gameController.AddSubstractMetal(-Time.deltaTime * 20);
                currentRepair += Time.deltaTime * 20;
                currentRepairCost -= Time.deltaTime * 20;
            }

        }

        else
        {
            DialogueLua.SetVariable("ArmsRepaired", DialogueLua.GetVariable("ArmsRepaired").AsInt + 1);
            print("Number of Arms Repaired: " + DialogueLua.GetVariable("ArmsRepaired").AsInt);
            realArm.SetActive(true);
            realArm.GetComponentInChildren<HandController>().SetDead(false);
            realArm.GetComponentInChildren<HandController>().RestoreHealthToFull();
            currentRepairCost = repairCost;
            currentRepair = 0;
            gameObject.SetActive(false);
            print("ArmRepaired");
        }
    }

    public float CurrentRepair()
    {
        return currentRepair;
    }

    public float CurrentRepairCost()
    {
        return currentRepairCost;
    }

}
