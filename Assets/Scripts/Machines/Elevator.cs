using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : Machine {
    [SerializeField]
    GameObject Exit;
    float progresss = 0f;
    [SerializeField]
    float cap;
    bool movingup = false;
    bool movingdown = false;


    // Use this for initialization
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePossessedGlow();
        if (movingdown && !movingup)
        {
            if (progresss > 0f)
            {
                foreach (MachineHelper h in auxiliaryMovingParts)
                {
                    h.direction = 1f;
                }
                movingParts[0].transform.position -= new Vector3(0, Time.deltaTime, 0);
                progresss -= Time.deltaTime;
            }
            else {
                foreach (MachineHelper h in auxiliaryMovingParts)
                {
                    h.direction = 0f;
                }
            }
        }
        if (!movingdown && movingup)
        {
            if (progresss < cap)
            {
                foreach (MachineHelper h in auxiliaryMovingParts)
                {
                    h.direction = -1f;
                }
                movingParts[0].transform.position += new Vector3(0, Time.deltaTime, 0);
                progresss += Time.deltaTime;
            }
            else {
                foreach (MachineHelper h in auxiliaryMovingParts)
                {
                    h.direction = 0f;
                }
            }
        }

    }
    public override void Activate()
    {
        //foreach (Rigidbody c in movingParts)
        //{
        //    c.isKinematic = false;
        //}
        t = 0;
        isActive = true;
        FindObjectOfType<FlameImpLogic>().SwitchCamera(positionMainCam.transform.position);

        for (int i = cameras.Count - 1; i >= 0; i--)
        {
            cameras[i].GetComponent<MachineCamera>().SwitchCamState();
        }
    }
    public override void Deactivate()
    {
        base.Deactivate();
    }
    protected override void Initialize()
    {
        base.Initialize();
    }
    public override void BottomButton()
    {
        //print("Bottom Button Pressed from Machine");
        base.BottomButton();
        movingdown = true;
    }

    public override void TopButton()
    {
        base.TopButton();
        movingup = true;
        //move lift up
    }
    public override void BottomButtonRelease()
    {
        foreach (MachineHelper h in auxiliaryMovingParts)
        {
            h.direction = 0f;
        }
        base.BottomButtonRelease(); 
        movingdown = false;
    }
    public override void TopButtonRelease()
    {
        foreach (MachineHelper h in auxiliaryMovingParts)
        {
            h.direction = 0f;
        }
        base.BottomButtonRelease();
        movingup = false;
    }
    public override void LeftButton()
    {
        base.LeftButton();
        FlameImpLogic go = FindObjectOfType<FlameImpLogic>();
        go.transform.position = Exit.transform.position;
        go.transform.rotation = Exit.transform.rotation;
        go.controllingMachine = false;
        go.LaunchImp();
        Deactivate();
    }
    // Update is called once per frame
}
