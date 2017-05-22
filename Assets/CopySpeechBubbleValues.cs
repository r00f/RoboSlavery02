using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopySpeechBubbleValues : MonoBehaviour {

    [SerializeField]
    GameObject originalBubblePanel;

    [SerializeField]
    GameObject bubblePanel;
    [SerializeField]
    Text originalBubbleText;
    [SerializeField]
    Text bubbleText;
    [SerializeField]
    Text originalPortraitName;
    [SerializeField]
    Text portraitName;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {

        bubbleText.text = originalBubbleText.text;
        portraitName.text = originalPortraitName.text;
        bubblePanel.SetActive(originalBubblePanel.activeSelf);
		
	}
}
