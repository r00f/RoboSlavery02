using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField]
    float playerNumber;
    [SerializeField]
    float distanceAway;
    [SerializeField]
    float distanceUp;
    [SerializeField]
    float camXRotation;
    [SerializeField]
    float camPosSmooth = 0.1f;
    [SerializeField]
    float camRotSmooth = 0.1f;

    //Wall Variables
    [SerializeField]
    float distanceFromWall = 1f;
    [SerializeField]
    float sphereCastRadius = 0.8f;

    [SerializeField]
    float sphereCastMaxDistance;

    [SerializeField]
    bool cameraHittingWall;

    [SerializeField]
    bool wideScreen;

    [SerializeField]
    Animator letterBoxTopAnim;

    [SerializeField]
    Animator letterBoxBottomAnim;

    public Vector3 fixedCamPos;

    Vector3 velocityCamSmooth = Vector3.zero;
    Vector3 velocityLookDir = Vector3.zero;

    [SerializeField]
    float lookDirDampTime;

    [SerializeField]
    float camSizeLerpTime;

    Vector3 targetPosition;
    Vector3 lookDir;
    Vector3 curLookDir;
    Transform followXForm;

    float currentCamX;
    float wantedCamX;


    ContactPoint[] contactPoints;

    public CamStates camState = CamStates.Behind;
    public bool characterInCamTrigger;
    PlayerLogic player;
    SteamGolemLogic steamGolem;


    public enum CamStates
    {
        Behind,
        Target,
        Fixed
    }

    // Use this for initialization
    void Start()
    {
        steamGolem = FindObjectOfType<SteamGolemLogic>();
        player = GameObject.FindGameObjectWithTag("Player" + playerNumber).GetComponent<PlayerLogic>();
        followXForm = player.transform.GetChild(1);

        Vector3 characterOffset = followXForm.position + new Vector3(0, distanceUp, 0);
        lookDir = Vector3.Lerp(followXForm.right * (player.HorizontalL < 0 ? 1f : 0f), followXForm.forward * (player.VerticalL < 0 ? -1f : 0f), Mathf.Abs(Vector3.Dot(this.transform.forward, followXForm.forward)));
        curLookDir = Vector3.Normalize(characterOffset - this.transform.position);
        curLookDir.y = 0;
        curLookDir = Vector3.SmoothDamp(curLookDir, lookDir, ref velocityLookDir, lookDirDampTime);

    }

    // Update is called once per frame
    void Update()
    {
        /*
        currentCamX = GetComponent<Camera>().rect.x;

        if (playerNumber == 1)
        {

            if (steamGolem.IsOverheated())
            {
                wantedCamX = 0f;
            }
            else
            {
                wantedCamX = .5f;
            }

            if(currentCamX != wantedCamX)
            {
                GetComponent<Camera>().rect = new Rect(Mathf.Lerp(currentCamX, wantedCamX, Time.deltaTime * camSizeLerpTime), 0f, 1f, 1.91f);
            }

        }

        else
        {
            if (steamGolem.IsOverheated())
            {
                wantedCamX = -.99f;
            }
            else
            {
                wantedCamX = -0.5f;
            }

            if (currentCamX != wantedCamX)
            {
                GetComponent<Camera>().rect = new Rect(Mathf.Lerp(currentCamX, wantedCamX, Time.deltaTime * camSizeLerpTime), 0f, 1f, 1.91f);
            }

            if (currentCamX <= -.98f)
            {
                GetComponent<Camera>().enabled = false;
            }
            else
            {
                GetComponent<Camera>().enabled = true;
            }

        }
        */
        letterBoxTopAnim.SetBool("WideScreen", wideScreen);
        letterBoxBottomAnim.SetBool("WideScreen", wideScreen);

    }

    void LateUpdate()
    {
        Vector3 characterOffset = followXForm.position + new Vector3(0, distanceUp, 0);

        if(characterInCamTrigger)
        {
            camState = CamStates.Fixed;
        }

        else if(Input.GetAxis("Target") > 0.1f)
        {
            camState = CamStates.Target;
        }
        else
        {
            camState = CamStates.Behind;
        }

        switch (camState)
        {
            case CamStates.Behind:

                wideScreen = false;

                if (player.Speed > player.LocomotionThreshold && player.IsInLocomotion() && !player.IsInPivot())
                {
                    lookDir = Vector3.Lerp(followXForm.right * (player.HorizontalL < 0 ? 1f : 0f), followXForm.forward * (player.VerticalL < 0 ? -1f : 0f), Mathf.Abs(Vector3.Dot(this.transform.forward, followXForm.forward)));
                    curLookDir = Vector3.Normalize(characterOffset - this.transform.position);
                    curLookDir.y = 0;
                    curLookDir = Vector3.SmoothDamp(curLookDir, lookDir, ref velocityLookDir, lookDirDampTime);

                }

                //calculate targetPosition

                targetPosition = characterOffset + followXForm.up * distanceUp - Vector3.Normalize(curLookDir) * distanceAway;

                //Debug.DrawRay(this.transform.position, lookDir, Color.green);
                //Debug.DrawRay(follow.position, follow.up * distanceUp, Color.red);
                //Debug.DrawRay(follow.position, -1 * follow.forward * distanceAway, Color.blue);
                //Debug.DrawLine(followXForm.position, targetPosition, Color.magenta);

                break;

            case CamStates.Target:

                wideScreen = true;
                targetPosition = characterOffset + followXForm.up * distanceUp - followXForm.forward * distanceAway;
                break;

            case CamStates.Fixed:

                wideScreen = false;
                targetPosition = fixedCamPos;
                break;
        }



        sphereCastMaxDistance = Vector3.Distance(characterOffset, this.transform.position);

        //SmoothDamp cameraPosition towards targetPosition
        CompensateForWalls(characterOffset, ref targetPosition);
        smoothPosition(this.transform.position, targetPosition);


        //New code that has a fixed camXRotation and only rotates around the y axis

        if (cameraHittingWall)
        {
            Quaternion newRotation = Quaternion.LookRotation(followXForm.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime / camRotSmooth);
        }

        else
        {
            Quaternion newRotation = Quaternion.LookRotation(followXForm.position - transform.position, Vector3.up);
            newRotation.x = 0f;
            newRotation.z = 0f;
            Quaternion finalRotation = newRotation * Quaternion.Euler(camXRotation, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime / camRotSmooth);
        }

    }

    private void smoothPosition (Vector3 fromPos, Vector3 toPos)
    {
        this.transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCamSmooth, camPosSmooth);
    }

    private void CompensateForWalls(Vector3 fromObject, ref Vector3 toTarget)
    {
        Debug.DrawLine(fromObject, toTarget, Color.cyan);

        //compensate for walls between cam and character
        /*
        RaycastHit wallHit = new RaycastHit();
        if(Physics.Linecast(fromObject, toTarget, out wallHit))
        {
            Debug.DrawRay(wallHit.point, Vector3.left, Color.red);
            toTarget = new Vector3(wallHit.point.x, toTarget.y, wallHit.point.z) + new Vector3(distanceFromWall * wallHit.normal.x, 0, distanceFromWall * wallHit.normal.z);
            cameraHittingWall = true;
        }

        else
        {
            cameraHittingWall = false;

        }
        */

        // Cast a sphere wrapping around camera
        // to see if it is about to hit anything.

        int layerMask = 1 << 8;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereCastRadius, layerMask);

        if (hitColliders.Length > 0 && sphereCastMaxDistance <= 4.5f)
        {
            //print(hitColliders[0].ClosestPointOnBounds(transform.position));
            toTarget = new Vector3(hitColliders[0].ClosestPointOnBounds(transform.position).x, toTarget.y, hitColliders[0].ClosestPointOnBounds(transform.position).z) + new Vector3(transform.forward.x * distanceFromWall, 0, transform.forward.z * distanceFromWall);
            cameraHittingWall = true;
        }

        else
        {
            cameraHittingWall = false;

        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
    }
}