using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHelper : MachineHelper {
    [SerializeField]
    float xrot;
    [SerializeField]
    float yrot;
    [SerializeField]
    float zrot;
    // Use this for initialization
    void Start () {
        Initialize();
    }
	
	// Update is called once per frame
	void Update () {
        if (direction != 0)
        {
            HandleMovement();
        }
    }
    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void HandleMovement()
    {
        base.HandleMovement();
        transform.Rotate(xrot * direction *Time.deltaTime, yrot * direction * Time.deltaTime, zrot * direction * Time.deltaTime);
    }
}
