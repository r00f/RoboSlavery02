using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour {



    #region Variables
    [SerializeField]
    protected List<Rigidbody> movingParts = new List<Rigidbody>();
    [SerializeField]
    protected GameObject positionMainCam;
    [SerializeField]
    protected List<MachineCamera> cameras = new List<MachineCamera>();
    [SerializeField]
    protected MachineTrigger Trigger;
    [SerializeField]
    protected Material glowHandlerMat;
    [SerializeField]
    protected Color overheatColor;
    [SerializeField]
    float overheatLerpDuration;
    [SerializeField]
    protected List<MachineHelper> auxiliaryMovingParts = new List<MachineHelper>();

    public bool isActive = false;
    public float Axis_HL { get; set; }
    public float Axis_HR { get; set; }
    public float Axis_VL { get; set; }
    public float Axis_VR { get; set; }

    protected float t = 1;
    
    

    #endregion

    protected virtual void Initialize()
    {
        foreach (Rigidbody r in GetComponentsInChildren<Rigidbody>())
        {
            movingParts.Add(r);
        }
        foreach (MachineCamera c in GetComponentsInChildren<MachineCamera>())
        {
            cameras.Add(c);
        }
        foreach(MachineHelper h in GetComponentsInChildren<MachineHelper>())
        {
            auxiliaryMovingParts.Add(h);
        }
        Trigger.ReferenceMachine = this.GetComponent<Machine>();
    }

    // Update is called once per frame

    public virtual void Activate()
    {
        t = 0;
        foreach(Rigidbody r in movingParts)
        {
            r.isKinematic = false;
        }

        isActive = true;
        FindObjectOfType<FlameImpLogic>().SwitchCamera(positionMainCam.transform.position);

        for (int i = cameras.Count -1; i>=0; i--)
        {
            cameras[i].GetComponent<MachineCamera>().SwitchCamState();
        }
    }
    public virtual void Deactivate()
    {
        t = 0;
        isActive = false;
        FindObjectOfType<FlameImpLogic>().SwitchCamera();

        for (int i = cameras.Count - 1; i >= 0; i--)
        {
            cameras[i].GetComponent<MachineCamera>().SwitchCamState();
        }
        //make things in a list notglow
    }
    protected void HandlePossessedGlow()
    {
        if (isActive)
        {
            if (glowHandlerMat.GetColor("_EmissionColor") != overheatColor)
            {
                //print("LERP Color to Orange");
                glowHandlerMat.SetColor("_EmissionColor", Color.Lerp(glowHandlerMat.GetColor("_EmissionColor"), overheatColor, t));

            }


            if (t < 1)
            { // while t below the end limit...
              // increment it at the desired rate every update:
                t += Time.deltaTime / overheatLerpDuration;
            }
        }
        else
        {
            if (glowHandlerMat.GetColor("_EmissionColor") != Color.black)
            {
                //print("LERP Color to Orange");
                glowHandlerMat.SetColor("_EmissionColor", Color.Lerp(glowHandlerMat.GetColor("_EmissionColor"), Color.black, t));

            }


            if (t < 1)
            { // while t below the end limit...
              // increment it at the desired rate every update:
                t += Time.deltaTime / overheatLerpDuration;
            }
        }
    }
    #region Virtuals
    public virtual void LeftStick()
    {
    }
    public virtual void RightStick()
    {
    }
    public virtual void LeftButton()
    {
    }
    public virtual void BottomButton()
    {
    }
    public virtual void TopButton()
    {
    }
    public virtual void RightButton()
    {
    }
    public virtual void LeftButtonRelease()
    {
    }
    public virtual void BottomButtonRelease()
    {
    }
    public virtual void TopButtonRelease()
    {
    }
    public virtual void RightButtonRelease()
    {
    }
}

#endregion