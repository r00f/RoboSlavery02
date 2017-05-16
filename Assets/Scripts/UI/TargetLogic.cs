using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetLogic : MonoBehaviour {

    [SerializeField]
    float targetYOffset;

    [SerializeField]
    Transform follow;

    [SerializeField]
    float scaleMultiplier;

    Camera cam;

    public int canvasNumber;

    void Start()
    {

        if (cam == null)
            cam = GameObject.FindGameObjectWithTag("Cam" + canvasNumber).GetComponent<Camera>();

    }


    void LateUpdate()
    {

        //scale Target with distance to camera

        if (follow.GetComponent<LivingEntity>().isTargeted)
        {
            transform.localScale = new Vector3(scaleMultiplier / Vector3.SqrMagnitude(follow.position - cam.transform.position), scaleMultiplier / Vector3.SqrMagnitude(follow.position - cam.transform.position), 1);

            if(follow.GetComponent<LivingEntity>().isTargetLocked)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if(follow.GetComponent<PlayerLogic>())
                    {
                        transform.GetChild(i).GetComponent<Image>().color = Color.green;
                    }
                    else
                    {
                        transform.GetChild(i).GetComponent<Image>().color = Color.red;
                    }
                    
                }

            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<Image>().color = Color.yellow;
                }

            }


            if (!transform.GetChild(0).gameObject.activeSelf)
            {
                //print("enableTarget");

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }

        }
        else 
        {
            if (transform.GetChild(0).gameObject.activeSelf)
            {

                //print("disableTarget");
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<Image>().color = Color.yellow;
                    transform.GetChild(i).gameObject.SetActive(false);
                }

            }

        }

        Vector3 wantedPos = cam.WorldToScreenPoint(follow.position + new Vector3(0, targetYOffset, 0));
        transform.position = wantedPos;
    }

    public void SetTarget(Transform target)
    {
        follow = target;
    }

}
