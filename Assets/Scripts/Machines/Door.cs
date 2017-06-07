using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Machine {

    [SerializeField]
    GameObject Exit;
    [SerializeField]
    float UpForce;
    [SerializeField]
    List<AudioClip> pushClips = new List<AudioClip>();
    AudioSource audioSource;
    [SerializeField]
    AudioSource loopAudioSource;


    // Use this for initialization
    void Start () {
        Initialize();
        audioSource = GetComponent<AudioSource>();
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
        loopAudioSource.Play();
    }
    public override void Deactivate()
    {
        base.Deactivate();
        loopAudioSource.Stop();
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
        audioSource.PlayOneShot(pushClips[Random.Range(0, pushClips.Count)], .2f);
    }

    public override void TopButton()
    {
        base.TopButton();

    }

}
