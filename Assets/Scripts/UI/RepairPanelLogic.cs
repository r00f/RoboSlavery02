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

    [SerializeField]
    Image circleSlider;

    Slider slider;
    Text repairCostText;

    HoloArmLogic holoArm;
    SteamGolemLogic steamGolem;

    Camera cam;

    public int canvasNumber;

    void Start()
    {
        repairCostText = GetComponentInChildren<Text>();
        //slider = GetComponentInChildren<Slider>();

        if (follow.GetComponent<HoloArmLogic>())
            holoArm = follow.GetComponent<HoloArmLogic>();
        else
            steamGolem = follow.GetComponent<SteamGolemLogic>();

        //slider.maxValue = holoArm.repairCost;
        if (cam == null)
            cam = GameObject.FindGameObjectWithTag("Cam" + canvasNumber).GetComponent<Camera>();

    }

    void Update()
    {

        if(holoArm)
        {
            repairCostText.text = "" + (int)holoArm.CurrentRepairCost();
            circleSlider.fillAmount = holoArm.CurrentRepair() / holoArm.repairCost;

        }
        else
        {
            repairCostText.text = "" + (int)steamGolem.CurrentRepairCost();
            circleSlider.fillAmount = steamGolem.CurrentRepair() / steamGolem.MaxHealth();
        }



        //slider.value = holoArm.CurrentRepair();
    }

    void LateUpdate()
    {
        if(holoArm)
        {
            transform.GetChild(0).gameObject.SetActive(follow.gameObject.activeSelf);
            transform.GetChild(1).gameObject.SetActive(follow.gameObject.activeSelf);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(follow.GetComponent<SteamGolemLogic>().holoTorso.activeSelf);
            transform.GetChild(1).gameObject.SetActive(follow.GetComponent<SteamGolemLogic>().holoTorso.activeSelf);
        }


        //scale Target with distance to camera
        transform.localScale = new Vector3(scaleMultiplier / Vector3.SqrMagnitude(follow.position - cam.transform.position), scaleMultiplier / Vector3.SqrMagnitude(follow.position - cam.transform.position), 1);

        Vector3 wantedPos = cam.WorldToScreenPoint(follow.position + new Vector3(0, targetYOffset, 0));
        transform.position = wantedPos;
    }
}
