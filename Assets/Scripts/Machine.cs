using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour {



    #region Variables
    [SerializeField]
    int NumberOfCams;
    [SerializeField]
    GameObject[] Cameras = new GameObject[4];
    [SerializeField]
    MachineTrigger Trigger;
    public bool isActive = false;
    public float Axis_HL { get; set; }
    public float Axis_HR { get; set; }
    public float Axis_VL { get; set; }
    public float Axis_VR { get; set; }


    #endregion


    // Use this for initialization
    void Start () {
		
	}
    protected virtual void Initialize()
    {
        Trigger.ReferenceMachine = this.GetComponent<Machine>();
    }
	
	// Update is called once per frame
	void Update () {
    }
    public void Activate()
    {
        isActive = true;
        for (int i = NumberOfCams-1; i>=0; i--)
        {
            Cameras[i].SetActive(true);
        }
        //make things in a list Glow
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
}

#endregion