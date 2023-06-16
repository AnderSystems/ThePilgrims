using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalCam : Cam
{
    public static Vector3 eulers;
    public static Vector2 mouseInputs { get { return new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); } }

    public Cam Alternative_Cam;

    public static Cam CreateNewOrbital(Vector3 pos, Quaternion rot, float FOV)
    {
        OrbitalCam r = new GameObject("Cam").AddComponent<OrbitalCam>();
        Transform parent = null;
#if UNITY_EDITOR
        parent = UnityEditor.Selection.activeTransform;
#endif

        r.transform.parent = parent;
        r.transform.position = pos;
        r.transform.rotation = rot;
        r.FieldOfView = FOV;

        r.transform.position += r.transform.forward * -3;
        r.Follow = new GameObject("OrbitalCamera").transform;
        r.Center = new GameObject("Orbit").transform;

        r.Follow.transform.position = pos;
        r.Follow.transform.parent = parent;

        r.Center.transform.position = pos;
        r.Center.transform.parent = r.Follow;

        r.transform.parent = r.Center;

        return r;
    }


    public Transform Follow;
    public Transform Center;

    [Space]

    [Min(0)] public float PositionDamping = 5;
    [Min(0)] public float RotationDamping = 5;
    [Min(0)] public float ResetTimeOut = 3;

    [Space]

    public float Sensitivity = 1;
    public float LimitsY = 80;

    public Vector3 resetEuler;
    public bool isReseted { get; set; }

    //Reset
    public void Reset(Vector3 euler)
    {
        resetEuler = euler;
        isReseted = true;
    } public void Reset()
    {
        Reset(Quaternion.LookRotation(Follow.forward).eulerAngles);
    }

    //Behaviour
    public void CalculeEuler()
    {
        Vector3 e = eulers;
        e.y += mouseInputs.x * Sensitivity;
        e.x -= mouseInputs.y * Sensitivity;
        eulers = e;
    }
    public void OnCameraChanged(Cam newCam)
    {
        if (newCam != this)
        {
            Center.transform.parent = Follow.transform.parent;
        }
    }

    public override void Set(float Lerp)
    {
        if (CurrentCam as OrbitalCam)
        {
            Center.transform.rotation = ((OrbitalCam)CurrentCam).Center.rotation;
        }
        base.Set(Lerp);
    }

    int Cam;

    //Mono
    private void Update()
    {
        if (CurrentCam != this)
            return;
        CalculeEuler();

        if (Input.mouseScrollDelta.magnitude != 0)
        //if(Input.GetKeyDown(KeyCode.Tab))
        {
            Invoke("SwapCam", .1f);
            Debug.Log($"CurrentCam: {CurrentCam.gameObject.name}");
        }
    }

    public void SwapCam()
    {
        if (CurrentCam != this)
            return;
        Alternative_Cam.Set();
    }

    public void FixedUpdate()
    {
        if (CurrentCam != this)
            return;


        if (mouseInputs.magnitude > 0.2f)
        {
            isReseted = false;
            CancelInvoke("Reset");
        }
        else
        {
            Invoke("Reset", ResetTimeOut);
        }

        if (PositionDamping > 0)
        {
            Center.transform.parent = null;
            Center.transform.position = Vector3.Lerp(Center.position, Follow.position, PositionDamping * Time.smoothDeltaTime);
        } else
        {
            Center.transform.parent = null;
            Center.transform.position = Follow.position;
        }

        if (RotationDamping > 0 && isReseted)
        {
            eulers = new Vector3(Mathf.LerpAngle(eulers.x, resetEuler.x, RotationDamping * Time.smoothDeltaTime), Mathf.LerpAngle(eulers.y, resetEuler.y, RotationDamping * Time.smoothDeltaTime));
        }

        eulers = new Vector3(Mathf.Clamp(eulers.x, -LimitsY, LimitsY), eulers.y,0);
        Center.eulerAngles = new Vector3(eulers.x, eulers.y, 0);
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,1,0,0.3f);
        Gizmos.DrawSphere(Center.position, .3f);
        Gizmos.DrawLine(transform.position, Center.position);

        Gizmos.color = new Color(1, 1, 0, 0.5f);
        Gizmos.DrawWireSphere(Follow.position, .3f);
        Gizmos.DrawLine(Center.position, Follow.position);
    }
}
