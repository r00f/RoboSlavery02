using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairPanelLogic : MonoBehaviour {

    [SerializeField]
    float targetYOffset;

    [SerializeField]
    Transform follow;

    [SerializeField]
    float scaleMultiplier;

    Slider slider;
    Text repairCostText;

    HoloArmLogic holoArm;

    Camera cam;

    public int canvasNumber;

    void Start()
    {
        repairCostText = GetComponentInChildren<Text>();
        slider = GetComponentInChildren<Slider>();
        
        holoArm = follow.GetComponent<HoloArmLogic>();
        slider.maxValue = holoArm.repairCost;
        if (cam == null)
            cam = GameObject.FindGameObjectWithTag("Cam" + canvasNumber).GetComponent<Camera>();

    }
    void Update()
    {
        repairCostText.text = "" + (int)holoArm.CurrentRepairCost();
        slider.value = holoArm.CurrentRepair();
    }

    void LateUpdate()
    {
        transform.GetChild(0).gameObject.SetActive(follow.gameObject.activeSelf);
        transform.GetChild(1).gameObject.SetActive(follow.gameObject.activeSelf);

        //scale Target with distance to camera
        transform.localScale = new Vector3(scaleMultiplier / Vector3.SqrMagnitude(follow.position - cam.transform.position), scaleMultiplier / Vector3.SqrMagnitude(follow.position - cam.transform.position), 1);

        Vector3 wantedPos = cam.WorldToScreenPoint(follow.position + new Vector3(0, targetYOffset, 0));
        transform.position = wantedPos;
    }
}
