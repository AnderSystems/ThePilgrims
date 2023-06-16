using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_LookToMouse : MonoBehaviour
{
    public Animator m_anim { get; set; }

    public Camera cam;

    public Vector3 screenPosition;
    public Vector3 worldPosition;

    public Transform debugTransform;

    void Update()
    {
        screenPosition = Input.mousePosition;
        screenPosition.z = cam.nearClipPlane;

        worldPosition = cam.ScreenToWorldPoint(screenPosition);
    }

    private void Start()
    {
        m_anim = GetComponent<Animator>();
        debugTransform = new GameObject().transform;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        debugTransform.position = worldPosition;
        

        //m_anim.SetLookAtPosition(cam.transform.InverseTransformPoint(worldPosition));
        m_anim.SetLookAtPosition(worldPosition);
        m_anim.SetLookAtWeight(1, 0.1f, 0.5f, 1, 0.1f);
    }
}
