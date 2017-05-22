using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMultiscreen : MonoBehaviour {

    Canvas canvas;

	// Use this for initialization
	void Start () {
        canvas = GetComponent<Canvas>();
        canvas.targetDisplay = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
