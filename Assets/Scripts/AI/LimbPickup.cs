using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbPickup : MonoBehaviour {

    [SerializeField]
    int metalAmount = 100;

    GameController gameController;
    MeshRenderer[] meshRenderers;
    List<Material> glowMats = new List<Material>();
    bool destroyLimb;
    float t = 0f;
    Color overheatColor = new Color(255, 69, 0);
    float overheatLerpDuration = 5f;
    float curRepBeamTime;
    float repBeamTime = 5;

    GameObject flameBeamPrefab;

    FlameImpLogic flameImp;
    Vector3 startScale;

    // Use this for initialization
    void Start () {

        startScale = transform.localScale;
        flameBeamPrefab = (GameObject)Resources.Load("RepairBeam");
        flameImp = FindObjectOfType<FlameImpLogic>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach(MeshRenderer r in meshRenderers)
        {
            foreach (Material m in r.materials)
            {
                glowMats.Add(m);
            }
        }

        gameController = FindObjectOfType<GameController>();
		
	}
	
	// Update is called once per frame
	void Update () {

        if(destroyLimb)
        {

            foreach (Material m in glowMats)
            {
                if (m.GetColor("_EmissionColor") != overheatColor)
                {
                    //print("LERP Color to Orange");
                    m.SetColor("_EmissionColor", Color.Lerp(Color.black, overheatColor, t));
                }
            }

            if (t < 1)
            { // while t below the end limit...
              // increment it at the desired rate every update:
                t += Time.deltaTime / overheatLerpDuration;

                if(transform.localScale.sqrMagnitude > 0.001f)
                {
                    transform.localScale -= new Vector3(startScale.x * Time.deltaTime / overheatLerpDuration, startScale.y * Time.deltaTime / overheatLerpDuration, startScale.z * Time.deltaTime / overheatLerpDuration);
                }
                gameController.AddSubstractMetal(metalAmount * Time.deltaTime / overheatLerpDuration);

                curRepBeamTime -= Time.deltaTime * 30;

                if (curRepBeamTime <= 0)
                {
                    GameObject repairBeam = Instantiate(flameBeamPrefab, transform.position, Quaternion.identity);
                    repairBeam.GetComponent<RepairBeamLogic>().target = flameImp.transform;
                    repairBeam.GetComponent<RepairBeamLogic>().targetOffset = Vector3.up;
                    curRepBeamTime = repBeamTime;
                }
            }

            else
            {
                Destroy(gameObject);
            }

        }
    }

    void OnCollisionEnter (Collision other)
    {
        if (other.collider.GetComponent<FlameImpLogic>())
        {
            destroyLimb = true;
        }

    }
}
