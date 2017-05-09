using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (currentRepair < repairCost)
        {
            if(gameController.GetMetalAmount() > 0)
            {
                gameController.AddSubstractMetal(-Time.deltaTime * 10);
                currentRepair += Time.deltaTime * 10;
                currentRepairCost -= Time.deltaTime * 10;
            }

        }

        else
        {
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
