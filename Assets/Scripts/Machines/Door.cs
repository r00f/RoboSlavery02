using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Machine {

    [SerializeField]
    GameObject Exit;
    [SerializeField]
    GameObject MovingPart;
    [SerializeField]
    float UpForce;
   // float progress = 0f;
    Vector3 Startpos;

    

	// Use this for initialization
	void Start () {
        Initialize();        
        Startpos = MovingPart.transform.position;
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
        MovingPart.GetComponent<Rigidbody>().AddForce(0, UpForce, 0);

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
