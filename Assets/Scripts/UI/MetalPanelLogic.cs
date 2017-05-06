using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalPanelLogic : MonoBehaviour {

    GameController gameController;

	// Use this for initialization
	void Start () {

        gameController = FindObjectOfType<GameController>();
		
	}
	
	// Update is called once per frame
	void Update () {

        GetComponentInChildren<Text>().text = "Metal: " + gameController.GetMetalAmount();
    }
}
