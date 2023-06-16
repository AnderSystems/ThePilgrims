using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class renderCam_Editor : Editor
{

}

#endif


public class renderCam : MonoBehaviour
{
    //Static fields
    public static void SendMessageToAll(string Message, object Contex)
    {
        foreach (var item in FindObjectsOfType<MonoBehaviour>())
        {
            item.SendMessage(Message, Contex, SendMessageOptions.DontRequireReceiver);
        }
    }
    /// <summary>
    /// Get the current cam
    /// </summary>
    public static Cam CurrentCam { get { return Cam.CurrentCam; } }
    public static Camera UnityCam { get { return GetMain().GetComponentInChildren<Camera>(); } }
    public static Animator anim { get { return UnityCam.GetComponent<Animator>(); } }
    public static float CurrentLerp;
    public static float CurrentFov;

    public static void PlayTrigger(string TriggerName)
    {
        anim.SetTrigger(TriggerName);
    }
    public static void PlayAnimation(string animation)
    {
        anim.Play(animation);
    }

    //Main camera
    //public static renderCam main { get { return FindObjectOfType<renderCam>(); } }
    public static renderCam main;
    /// <summary>
    /// Search for main camera
    /// </summary>
    /// <returns></returns>
    public static renderCam GetMain()
    {

        
        if (!main)
        {
            main = FindObjectOfType<renderCam>();
        }

        if(!main)
        {
            renderCam _cam = new GameObject("renderCam").AddComponent<renderCam>();
            _cam.gameObject.AddComponent<Camera>();
            _cam.gameObject.AddComponent<AudioListener>();
            main = _cam;
            return _cam;
        } else
        {
            return main;
        }
    }
    /// <summary>
    /// Preview some cam
    /// </summary>
    /// <param name="target"></param>
    public static void PreviewCam(Cam target)
    {
        main.transform.position = target.transform.position;
        main.transform.rotation = target.transform.rotation;
        UnityCam.fieldOfView = target.FieldOfView;
    }

    /// <summary>
    /// Set the position and rotation of this renderCam
    /// </summary>
    /// <param name="_lerp"></param>
    /// <param name="target"></param>
    public void SetPositionAndRotation(float _lerp, Transform target)
    {
        if (_lerp > 0)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, _lerp * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, _lerp * Time.deltaTime);
        }
        else
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }


    //Params
    public Cam StartCamera;
    [Space]
    public float InteractionRange = 10;
    public LayerMask InteractionLayers;
    public string InteractionButtonName = "interaction";
    public string InteractionButtonNameVariant = "interaction2";
    public RaycastHit lookAtHit;

    //Mono Behaviour
    private void Awake()
    {
        if (!main)
        {
            main = FindObjectOfType<renderCam>();
        }
    }
    private void Start()
    {
        if (!StartCamera)
            return;
        StartCamera.Set();
    }
    private void OnDrawGizmos()
    {
        GetMain();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0,1,1,0.2f);
        Gizmos.DrawSphere(transform.position, InteractionRange);
    }
    private void FixedUpdate()
    {
        if (!CurrentCam)
            return;
        if (Application.isPlaying || !Application.isEditor)
        {
            transform.parent = CurrentCam.transform;
        }
        CurrentFov = CurrentCam.FieldOfView;

        UnityCam.fieldOfView = Mathf.Lerp(UnityCam.fieldOfView, CurrentFov, CurrentLerp / 100);

        if (CurrentCam.CameraCollisionEndPoint)
        {
            RaycastHit camCollisionHit;
            if (Physics.Linecast(CurrentCam.CameraCollisionEndPoint.position, CurrentCam.transform.position + (Vector3.down * UnityCam.nearClipPlane), out camCollisionHit, CurrentCam.CollisionLayers))
            {
                transform.position = camCollisionHit.point + (camCollisionHit.normal * UnityCam.nearClipPlane);
            } else
            {
                SetPositionAndRotation(CurrentLerp, CurrentCam.transform);
            }
        } else
        {
            SetPositionAndRotation(CurrentLerp, CurrentCam.transform);
        }

        Vector3 startPos = transform.position;

        if (Cam.CurrentCam as OrbitalCam)
        {
            OrbitalCam c = (OrbitalCam)Cam.CurrentCam;
            startPos = c.Center.position + (c.Center.transform.forward * 0.5f);
        }

        Physics.Linecast(transform.position, transform.position + (transform.forward * InteractionRange), out lookAtHit, InteractionLayers);
    }

    private void Update()
    {
        if (lookAtHit.collider) { lookAtHit.collider.SendMessage("OnCameraAim", SendMessageOptions.DontRequireReceiver);
            Debug.Log($"Camera Aim to '{lookAtHit.collider.gameObject.name}'");            }

        if (!string.IsNullOrEmpty(InteractionButtonName))
        {
            if (Input.GetButtonDown(InteractionButtonName))
            {
                if (lookAtHit.collider)
                {
                    lookAtHit.collider.SendMessage("OnCameraInteraction", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        if (!string.IsNullOrEmpty(InteractionButtonNameVariant))
        {
            if (Input.GetButtonDown(InteractionButtonNameVariant))
            {
                if (lookAtHit.collider)
                {
                    lookAtHit.collider.SendMessage("OnCameraInteraction2", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
