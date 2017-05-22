using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Machine {

    [SerializeField]
    GameObject Exit;
    [SerializeField]
    GameObject MovingPart;
    [SerializeField]
    float progress = 0f;
    Vector3 Startpos;

    

	// Use this for initialization
	void Start () {
        Initialize();        
        Startpos = MovingPart.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            MovingPart.transform.position = Startpos + new Vector3(0, progress, 0);
        }
        if (progress > 0)
        {
            progress -= Time.deltaTime;
        }
		
	}
    protected override void Initialize()
    {
        base.Initialize();
    }
    public override void BottomButton()
    {
        print("Bottom Button Pressed from Machine");
        base.BottomButton();
        progress += 1f;

    }
    public override void TopButton()
    {
        base.TopButton();
        FlameImpLogic go = FindObjectOfType<FlameImpLogic>();
        go.transform.position = Exit.transform.position;
        go.SwitchColliders();
        go.SwitchRenderers();
        go.controllingMachine = false;
        isActive = false;
        Deactivate();

    }

}
