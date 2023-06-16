using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IK;

public class IK_anim : MonoBehaviour
{
    public IK IK;
    public Animator anim;

    public float Clamp = 270;
    public float Power = 1.5f;


    private void Awake()
    {
        if (!IK)
        {
            IK = GetComponentInParent<IK>();
        }

        if (!anim)
        {
            anim = GetComponent<Animator>();
        }
    }
    public float lookAtAngle { get; set; }
    public float lookAtClamp { get; set; }

    private void OnAnimatorIK(int layerIndex)
    {
        //lookAtClamp = (Mathf.Abs(Mathf.Atan2(IK.Sync.LookAtPos.x, IK.Sync.LookAtPos.z)) * Mathf.Rad2Deg);
        //lookAtClamp = 1 - Mathf.Clamp(Mathf.Pow((lookAtClamp / Clamp), Power), 0, 1);

        Vector3 look = transform.TransformPoint(IK.Sync.LookAtPos);
        lookAtAngle = Mathf.Abs(Mathf.Atan2(-IK.Sync.LookAtPos.x, -IK.Sync.LookAtPos.z) * Mathf.Rad2Deg);
        lookAtClamp = Mathf.Pow((lookAtAngle / 90),2);

        //Apply
        float anim_IK_Head = anim.GetFloat("IK_Head");
        //anim.SetLookAtWeight(IK.Sync.LookAtWeight * lookAtClamp * anim_IK_Head, 0.1f, 1, 1, 0.2f);
        anim.SetLookAtWeight(lookAtClamp, 0.02f, 0.7f * lookAtClamp, 1, 0.1f);
        anim.SetLookAtPosition(look);
    }
}
