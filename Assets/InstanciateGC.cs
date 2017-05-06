using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanciateGC : MonoBehaviour {

    [SerializeField]
    GameController gameController;

	// Use this for initialization
	void Awake () {

        if (!FindObjectOfType<GameController>())
            Instantiate(gameController);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
