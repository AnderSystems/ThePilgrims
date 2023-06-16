using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMainCameraRotation : MonoBehaviour
{
    [System.Serializable]
    public enum method
    {
        CopyRotation,
        LookAt
    }
    [SerializeField]
    public method Method;

    public Transform m_target { get; set; }
    public float ScaleFactor = 0;
    [Range(0f, 10f)] public float Lerp = 0;
    public bool YOnly;

    private void Awake()
    {
        renderCam.GetMain();
    }
    void Update()
    {
        CopyCameraRotation(renderCam.UnityCam.transform);
    }

    void CopyCameraRotation(Transform target)
    {
        m_target = target;

        //Rotation
        Quaternion rot = m_target.rotation;
        if (Method == method.LookAt) { rot = Quaternion.LookRotation(transform.position - m_target.position); }
        if (YOnly) { rot = Quaternion.Euler(0f, rot.eulerAngles.y, 0f); }
        if (Lerp != 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Mathf.Abs(Lerp) * Time.deltaTime);
        }
        else
        {
            transform.rotation = rot;
        }

        //Scale
        if (ScaleFactor != 0)
        {
            Vector3 scl = Vector3.one * (Vector3.Distance(transform.position, m_target.position) * ScaleFactor);

            if (ScaleFactor < 0)
            {
                scl = Vector3.one / (Vector3.Distance(transform.position, m_target.position) * -ScaleFactor);
            }

            if (Lerp > 0)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, scl, Lerp * Time.deltaTime);
            } else
            {
                transform.localScale = scl;
            }
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;
        if (UnityEditor.Selection.activeObject == transform.IsChildOf(this.transform))
        {
            CopyCameraRotation(Camera.current.transform);
        }
#endif
    }
}
