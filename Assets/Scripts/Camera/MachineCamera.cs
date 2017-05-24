using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Camera))]

public class MachineCamera : MonoBehaviour {
    [SerializeField]
    GameObject panel;
    [SerializeField]
    GameObject focalObject;
    Vector3 offset;

    public enum CamStates
    {
        Fixed,
        Following
    }
    public CamStates CameraState;

    // Use this for initialization
    void Start () {

        if (focalObject != null)
        {
            offset = transform.position - focalObject.transform.position;
        }

	}
	
	// Update is called once per frame
	void Update () {
        if ( focalObject != null)
        {
            switch (CameraState){
                case CamStates.Fixed:
                    transform.LookAt(focalObject.transform.position);
                   break;
                case CamStates.Following:
                    transform.position = focalObject.transform.position + offset;
                    break;
                default:
                    break;


            }
        }
		
	}
    public void SwitchCamState()
    {
        panel.SetActive(!panel.activeSelf);
        GetComponent<Camera>().enabled = !GetComponent<Camera>().enabled;
    }
}
