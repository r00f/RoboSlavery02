﻿using System.Collections;
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
    protected FlameImpLogic flameImp;
    protected bool exitedPipe = true;

    public bool isActive = false;
    public float Axis_HL { get; set; }
    public float Axis_HR { get; set; }
    public float Axis_VL { get; set; }
    public float Axis_VR { get; set; }

    protected float t = 1;

    [SerializeField]
    Transform pipeTransformParent;

    public List<Transform> pipeTransforms = new List<Transform>();
    [SerializeField]
    Transform nextPipeTransform;
    [SerializeField]
    float pipeSpeed;

    #endregion

    protected virtual void Initialize()
    {
        flameImp = FindObjectOfType<FlameImpLogic>();

        foreach (Transform t in pipeTransformParent.GetComponentsInChildren<Transform>())
        {
            pipeTransforms.Add(t);
        }
        pipeTransforms.Remove(pipeTransformParent);


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
        flameImp.SwitchCamera(positionMainCam.transform.position);

        for (int i = cameras.Count -1; i>=0; i--)
        {
            cameras[i].GetComponent<MachineCamera>().SwitchCamState();
        }

    }
    public virtual void Deactivate()
    {
        t = 0;
        isActive = false;
        flameImp.SwitchCamera();

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

    protected void HandleInPipeBool()
    {


    }

    protected void MoveImpThroughPipe()
    {

        for (int i = 0; i < pipeTransforms.Count; i++)
        {
            if (flameImp.transform.position == pipeTransforms[i].position)
            {
                print("Reached " + pipeTransforms[i].position);

                if (i < pipeTransforms.Count - 1)
                    nextPipeTransform = pipeTransforms[i + 1];
                else
                    nextPipeTransform = null;
            }
        }

        if(nextPipeTransform)
        {
            exitedPipe = false;
            flameImp.transform.position = Vector3.MoveTowards(flameImp.transform.position, nextPipeTransform.position, pipeSpeed * Time.deltaTime);
        }

        else
        {
            flameImp.transform.rotation = pipeTransforms[pipeTransforms.Count-1].transform.rotation;
            flameImp.LaunchImp();
            exitedPipe = true;
        }


    }

    #region Virtualss
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
    public void R2()
    {
        exitedPipe = false;
        flameImp.controllingMachine = false;
        Deactivate();
    }
    #endregion
}

