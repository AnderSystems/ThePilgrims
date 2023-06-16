using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ped : Entity
{
    public PedController controller {get;set;}

    //Multiplayer
    [System.Serializable]public class animations
    {
        [Header("Animations")]
        public float anim_Angle;
        public float anim_CurrentMoveSpeed;
        public float anim_fallSpeed;
        public float anim_MoveX;
        public float anim_MoveZ;
        public bool anim_isGrounded;
        public float anim_talk;
    }
    [SerializeField]
    public animations Animations;


    //Ground Detection
    [SerializeField] [Tooltip("Manage the ground Detection")] public groundDetection GroundDetection;
    /// <summary>
    /// returns if this ped is Grounded
    /// </summary>
    public bool isGrounded { get { return GroundDetection.isGrounded; } }

    public List<Renderer> renderers = new List<Renderer>();

    /// <summary>
    /// Returns the ground hit info when this ped is grounded
    /// </summary>
    public RaycastHit groundHit { get { return GroundDetection.gHit; } }

    [Min(0)] [Tooltip("The speed multipiler of this ped movement")]public float MoveSpeed = 1;
    [Min(0)] [Tooltip("The speed of thurn to look at")][SerializeField] public float ThurnSpeed = 5;
    public float currentThurnSpeed { get; set; }

    //Movement
    public float _currentMoveSpeed { get; set; }
    public Vector3 _moveDirection { get; set; }
    public Vector3 _lookDirection { get; set; }
    public float animationCurrentMoveSpeed { get; set; }
    public float GetMoveSpeedPercent()
    {
        return _currentMoveSpeed / MoveSpeed;
    }
    public void Move(Vector3 direction, float speedMultipiler = 1)
    {
        _moveDirection = Vector3.ClampMagnitude(direction, 1);
        _currentMoveSpeed = MoveSpeed * speedMultipiler;
    }
    public void LookTo(Vector3 worldCoord)
    {
        LookTo(worldCoord, ThurnSpeed);
    }
    public void LookTo(Vector3 worldCoord, float lerp = 3)
    {
        currentThurnSpeed = lerp;
        _lookDirection = worldCoord;
    }
    Vector3 inverseDirection;
    public void OnAnimations()
    {
        animationCurrentMoveSpeed = Mathf.Lerp(animationCurrentMoveSpeed, _moveDirection.magnitude * _currentMoveSpeed, 8 * Time.deltaTime);
        inverseDirection = Vector3.Lerp(inverseDirection, transform.InverseTransformDirection(_moveDirection) * (_currentMoveSpeed / MoveSpeed), 8 * Time.deltaTime);

        //Floats
        Animations.anim_CurrentMoveSpeed = animationCurrentMoveSpeed;
        Animations.anim_fallSpeed = rb.velocity.y;

        Animations.anim_MoveX = inverseDirection.z;
        Animations.anim_MoveZ = inverseDirection.x;

        //Bools
        Animations.anim_isGrounded = isGrounded;

        Vector3 localLookDir = transform.InverseTransformPoint(_lookDirection);
        Animations.anim_Angle = Mathf.Lerp(Animations.anim_Angle, (Mathf.Atan2(localLookDir.x, localLookDir.z) * Mathf.Rad2Deg), 3 * Time.deltaTime);

        //Floats
        int emptyAnim = -1;
        if (anims.Count <= 0)
            return;
        foreach (var anim in anims)
        {
            if (anim != null)
            {
                anim.SetFloat("currentMoveSpeed", Animations.anim_CurrentMoveSpeed, 30, 5);
                anim.SetFloat("fallSpeed", Animations.anim_fallSpeed);
                anim.SetFloat("Angle", Animations.anim_Angle * new Vector2(Animations.anim_MoveX, Animations.anim_MoveZ).normalized.magnitude);

                anim.SetFloat("MoveX", Animations.anim_MoveX);
                anim.SetFloat("MoveZ", Animations.anim_MoveZ);

                anim.SetFloat("Talking", Animations.anim_talk);

                //Bools
                anim.SetBool("isGrounded", Animations.anim_isGrounded);
            } else
            {
                emptyAnim = anims.IndexOf(anim);
            }
        }

        if (emptyAnim > 0)
        {
            anims.RemoveAt(emptyAnim);
        }
    }

    public void HidePedOnThisFrame(bool shadowCaster)
    {
        CancelInvoke("ShowPed");
        foreach (var item in renderers)
        {
            item.enabled = false;
        }
        Invoke("ShowPed", .5f);
    }
    public void ShowPed()
    {
        foreach (var item in renderers)
        {
            item.enabled = true;
        }
    }


    //Mono Behaviour voids
    public void Start()
    {
        InvokeRepeating("AntiVoid", 3, 3);
    }
    public void FixedUpdate()
    {
        if (isFreezed)
            return;
        OnAnimations(); PutOnGroundCorrecly(GroundDetection, this);

        //Make this ped look at
        if (Vector3.Distance(transform.position, _lookDirection) > .4f)
        {
            Vector3 lookRotation = new Vector3(0, Quaternion.LookRotation(_lookDirection-transform.position).eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(lookRotation), currentThurnSpeed * Time.deltaTime);
        }

        //Move this ped
        if (isGrounded)
        {
            rb.velocity = new Vector3(_moveDirection.x * _currentMoveSpeed, 0, _moveDirection.z * _currentMoveSpeed);
        }
        else
        {
            Vector3 vel = new Vector3(_moveDirection.x * _currentMoveSpeed, rb.velocity.y, _moveDirection.z * _currentMoveSpeed);
            rb.velocity = Vector3.Lerp(rb.velocity, vel, 1 * Time.deltaTime);
        }
    }

    Vector3 lastGroundedPos { get; set; }
    public void AntiVoid()
    {
        if (isGrounded)
        {
            lastGroundedPos = transform.position;
        } else
        {
            if (transform.position.y <= -100)
            {
                transform.position = lastGroundedPos;
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 gizmosPos = transform.position + _moveDirection * (_currentMoveSpeed * .4f);
        gizmosPos.y = transform.position.y;

        //Draw Movement Gizmos
        UnityEditor.Handles.color = Color.cyan; UnityEditor.Handles.DrawLine(transform.position, gizmosPos);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(gizmosPos, .35f);

        UnityEditor.Handles.color = Color.red; UnityEditor.Handles.DrawLine(transform.position, transform.position + (transform.forward * (_currentMoveSpeed * .2f)));
        Gizmos.color = new Color(1, 0, 0, 0.2f); Gizmos.DrawSphere(transform.position + transform.forward * (_currentMoveSpeed * .2f), .1f);
        

        //Draw Physics Gizmos
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, .5f);
        UnityEditor.Handles.DrawLine(transform.position, transform.position + (Vector3.down * GroundDetection.detectionDistance));
        UnityEditor.Handles.DrawWireDisc(transform.position + (Vector3.down * GroundDetection.detectionDistance), transform.up, .3f);
    }
#endif
}
