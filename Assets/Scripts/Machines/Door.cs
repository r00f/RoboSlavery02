using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Machine {

    [SerializeField]
    GameObject Exit;
    [SerializeField]
    float UpForce;

    

	// Use this for initialization
	void Start () {
        Initialize();
	}
	
	// Update is called once per frame
	void Update () {
      
		
	}
    protected override void Initialize()
    {
        base.Initialize();
    }
    public override void BottomButton()
    {
        print("Bottom Button Pressed from Machine");
        base.BottomButton();
        movingParts[0].AddForce(0, UpForce, 0);
    }

    public override void TopButton()
    {
        base.TopButton();
        FlameImpLogic go = FindObjectOfType<FlameImpLogic>();
        go.transform.position = Exit.transform.position;
        go.transform.rotation = Exit.transform.rotation;
        go.controllingMachine = false;
        go.FireImp();
        Deactivate();
    }

}
