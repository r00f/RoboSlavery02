using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour {



    #region Variables
    [SerializeField]
    protected List<Rigidbody> movingParts = new List<Rigidbody>();
    [SerializeField]
    protected GameObject PositionMainCam;
    [SerializeField]
    protected List<MachineCamera> cameras = new List<MachineCamera>();
    [SerializeField]
    protected MachineTrigger Trigger;
    public bool isActive = false;
    public float Axis_HL { get; set; }
    public float Axis_HR { get; set; }
    public float Axis_VL { get; set; }
    public float Axis_VR { get; set; }


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

        Trigger.ReferenceMachine = this.GetComponent<Machine>();
    }
	
	// Update is called once per frame
	void Update () {
    }
    public virtual void Activate()
    {
        foreach(Rigidbody r in movingParts)
        {
            r.isKinematic = false;
        }

        isActive = true;
        FindObjectOfType<FlameImpLogic>().SwitchCamera(PositionMainCam.transform.position);

        for (int i = cameras.Count -1; i>=0; i--)
        {
            cameras[i].GetComponent<MachineCamera>().SwitchCamState();
        }
        //make things in a list Glow
    }
    public virtual void Deactivate()
    {
        isActive = false;
        FindObjectOfType<FlameImpLogic>().SwitchCamera();

        for (int i = cameras.Count - 1; i >= 0; i--)
        {
            cameras[i].GetComponent<MachineCamera>().SwitchCamState();
        }
        //make things in a list notglow
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