﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOffsetLogic : MonoBehaviour {

    Renderer meshRenderer;
    [SerializeField]
    float scrollSpeed = 10;

	// Use this for initialization
	void Start () {

        meshRenderer = GetComponent<Renderer>();
		
	}
	
	// Update is called once per frame
	void Update () {

        if(meshRenderer.materials.Length > 1)
        {
            meshRenderer.materials[1].SetVector("_Offset", meshRenderer.materials[1].GetVector("_Offset") + new Vector4(scrollSpeed * Time.deltaTime, 0, 0, 0));
        }
        else
        {
            meshRenderer.material.SetVector("_Offset", meshRenderer.material.GetVector("_Offset") + new Vector4(scrollSpeed * Time.deltaTime, 0, 0, 0));
            meshRenderer.material.color = new Color(1, 1, 1, 0);
        }

    }
}
