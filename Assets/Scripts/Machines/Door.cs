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

        if(!exitedPipe)
            MoveImpThroughPipe();

        HandlePossessedGlow();

        foreach (MachineHelper h in auxiliaryMovingParts)
        {
            h.direction = movingParts[0].velocity.y;
        }
		
	}
    public override void Activate()
    {
        base.Activate();
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
        print("Bottom Button Pressed from Machine");
        base.BottomButton();
        movingParts[0].AddForce(0, UpForce, 0);
    }

    public override void TopButton()
    {
        base.TopButton();
        FlameImpLogic go = FindObjectOfType<FlameImpLogic>();
        exitedPipe = false;
        go.controllingMachine = false;
        Deactivate();
    }

}
