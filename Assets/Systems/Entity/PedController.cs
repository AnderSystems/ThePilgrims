using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PedController : MonoBehaviour
{
    public static PedController main;

    public OrbitalCam cam;

    public Ped ped { get; set; }
    public Transform orbit;

    public Vector3 moveInputs { get; set; }
    public Vector3 moveInputsRaw { get; set; }
    public float currentWalkState { get; set; } = 1;
    public bool isWalking { get; set; }
    public bool HidePedWhenInside;

    public float timeToLookDirectionDamp = 3f;


    [System.Serializable] public class interaction
    {
        public RaycastHit hit;
        public LayerMask rayLayers;
        public float maxDistance = 1;
    }
    [SerializeField] public interaction Interaction;


    public void LateUpdate()
    {
        if (!HidePedWhenInside)
            return;
        if (Vector3.Distance(renderCam.main.transform.position, cam.Center.transform.position) <= 0.3f)
        {
            ped.HidePedOnThisFrame(true);
        } else
        {
            ped.ShowPed();
        }
    }

    private void Update()
    {
        //Manage walk states
        if (Input.GetButton("Run"))
        {
            currentWalkState = 1.5f;
        } else
        {
            if (Input.GetButtonDown("Walk")) {isWalking = !isWalking;}

            if (isWalking)
            {
                currentWalkState = .5f;
            } else
            {
                currentWalkState = 1;
            }
        }

        //Perform playerPed
        moveInputs = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveInputsRaw = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //Check if ped is freezed
        if (ped.isFreezed)
            return;

        //Apply the movements
        ped.Move(orbit.TransformDirection(moveInputs * 2) + (Vector3.up * 2), currentWalkState);

        if (moveInputsRaw.magnitude > 0)
        {
            if ((int)timeToLookDirection <= 1)
            {
                ped.LookTo(orbit.transform.forward + orbit.transform.position, ped.ThurnSpeed * 2);
                timeToLookDirection += timeToLookDirectionDamp * 0.001f;
            } else
            {
                ped.LookTo(orbit.TransformDirection(moveInputsRaw) + orbit.transform.position, ped.ThurnSpeed / 2);
            }
        } else
        {
            timeToLookDirection = 0;
        }
    }
    float timeToLookDirection;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + (orbit.transform.forward * Interaction.maxDistance));
    }

    private void Start()
    {
        main = this;
        ped = GetComponent<Ped>();
    }

    private void OnEnable()
    {
        ped = GetComponent<Ped>();
        ped.controller = this;
    }
}
