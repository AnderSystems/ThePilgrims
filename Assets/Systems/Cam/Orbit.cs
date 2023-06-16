using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    //Static
    /// <summary>
    /// Convert euler angles to mouse input
    /// </summary>
    /// <param name="Euler"></param>
    /// <returns></returns>
    public static Vector2 Euler2MouseInput(Vector3 Euler)
    {
        Vector2 r = Vector2.zero;

        r.x = Euler.y;
        r.y = Euler.x;

        return r;
    }
    public static Orbit activeOrbit { get { return Cam.CurrentCam._orbit; } }


    [Tooltip("The object to follow with damping etc...")]public Transform Reference;
    public Transform _currentReference { get; set; }
    public static Transform _directReference;
    [Space]
    [Tooltip("The position follow lerp")]public float positionDamping = 3;
    [Tooltip("The rotation follow lerp (leave this value 0 to disable)")]public float rotationDamping = 3;
    [Header("Mouse Look")]
    [Tooltip("The master sensitivity of this mouse")]public float Sensitivity = 1;
    [Tooltip("The Vertical limits")]public float YLimits = 80;
    [Tooltip("The time to reset this (useful to use with rotation damping)")]public float ResetTimeOut = 1;
    [Space]
    [SerializeField] float DefaultLerp = 5;
    /// <summary>
    /// The actual lerp operations value
    /// </summary>
    public float _currentLerp { get; set; }

    /// <summary>
    /// The direct input of this script (use "SetEuler" and freeze this object to set this)
    /// </summary>
    public static Vector2 _directMouseInput {get; private set;}
    /// <summary>
    /// The current mouse position (use "SetEuler" and freeze this object to set this)
    /// </summary>
    public static Vector2 mousePosition { get; set; }
    /// <summary>
    /// The current input of mouse
    /// </summary>
    public Vector2 mouseLookInput { get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Sensitivity; } }
    /// <summary>
    /// The mouse currently mooving? (the false value depends of "ResetTimeOut")
    /// </summary>
    public bool _mouseIsMooving { get; set; }

    /// <summary>
    /// is freezed?
    /// </summary>
    public float _freezePriority { get; private set; } = -1;


    //Performs
    /// <summary>
    /// Perform the damping operations (called on FixedUpdate)
    /// </summary>
    public void CameraFollow()
    {
        transform.position = Vector3.Lerp(transform.position, _directReference.position, positionDamping * Time.deltaTime);

        if (rotationDamping > 0 && !_mouseIsMooving)
        {
            Quaternion rot = Quaternion.LookRotation(_directReference.forward);
            Quaternion result = Quaternion.Lerp(transform.rotation, rot, rotationDamping * Time.deltaTime);

            SetEuler(result.eulerAngles);
        }
    }

    /// <summary>
    /// Perform the mouselook operations (Called on Update)
    /// </summary>
    public void MouseLook()
    {
        mousePosition = new Vector2(
            mousePosition.x + mouseLookInput.x,
            Mathf.Clamp(mousePosition.y -mouseLookInput.y, -YLimits, YLimits));
        _directMouseInput = mousePosition;
    }



    //Utils
    /// <summary>
    /// Set a new euler angle of this orbit
    /// </summary>
    /// <param name="Euler"></param>
    public void SetEuler(Vector3 Euler)
    {
        _directMouseInput = Euler2MouseInput(Euler);
    }
    /// <summary>
    /// Set a new Euler angle of this orbit using lerp
    /// </summary>
    /// <param name="Euler"></param>
    /// <param name="Lerp"></param>
    public void SetEuler(Vector3 Euler, float Lerp)
    {
        _currentLerp = Lerp;
        SetEuler(Euler);
    }
    /// <summary>
    /// Set the value for lerp operations
    /// </summary>
    /// <param name="Lerp"></param>
    public void SetLerp(float Lerp)
    {
        _currentLerp = Lerp;
    }
    /// <summary>
    /// Look for some coordinates
    /// </summary>
    /// <param name="worldCoord"></param>
    public void LookAt(Vector3 worldCoord)
    {
        Quaternion rot = Quaternion.LookRotation(worldCoord - transform.position);
        SetEuler(rot.eulerAngles);
    }
    /// <summary>
    /// Freeze this
    /// </summary>
    /// <param name="priority"></param>
    public void Freeze(float priority)
    {
        if (priority > _freezePriority)
        {
            _freezePriority = priority;
        }
    }
    /// <summary>
    /// Unfreeze this (only if priority is equals or larger than _FreezePriority)
    /// </summary>
    /// <param name="priority"></param>
    public void UnFreeze(float priority)
    {
        if (priority >= _freezePriority)
        {
            _freezePriority = -1;
        }
    }
    /// <summary>
    /// Reset this
    /// </summary>
    public void Reset()
    {
        _mouseIsMooving = false;
    }
    public void SetReference(Transform newReference)
    {
        _currentReference = newReference;
    }
    public void ResetReference()
    {
        _currentReference = Reference;
    }

    //Mono behaviour
    private void Awake()
    {
        SetReference(Reference);
        _currentLerp = DefaultLerp;
        if (positionDamping > 0)
        {
            transform.parent = null;
        }
    }
    private void Update()
    {
        if (Cam.CurrentCam._orbit != this)
            return;
        if (mouseLookInput.magnitude != 0)
        {
            CancelInvoke("Reset");
            _mouseIsMooving = true;
            Invoke("Reset", ResetTimeOut);
        }


        if (Sensitivity > 0 && _mouseIsMooving && _freezePriority <= 0)
        {
            MouseLook();
        } else
        {
            mousePosition = new Vector2(
                Mathf.LerpAngle(mousePosition.x, _directMouseInput.x, _currentLerp / 100),
                Mathf.LerpAngle(mousePosition.y, _directMouseInput.y, _currentLerp / 100));
        }
    }
    private void FixedUpdate()
    {
        if (Cam.CurrentCam._orbit != this)
            return;
        transform.eulerAngles = new Vector3(mousePosition.y, mousePosition.x, 0);

        if (activeOrbit)
        {
            _directReference = activeOrbit._currentReference;
        }

        CameraFollow();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.Contains(GetComponentInParent<Transform>()))
        {
            Gizmos.color = new Color(0, 1, 1, 0.3f);
        }
        else
        {
            Gizmos.color = Color.cyan;
        }

        Gizmos.DrawWireSphere(transform.position, 0.5f);
#endif
    }

    public static void DrawAllOrbitGizmos(Transform Selection)
    {

    }
}
