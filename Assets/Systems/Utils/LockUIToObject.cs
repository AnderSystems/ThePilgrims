using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockUIToObject : MonoBehaviour
{
    public Transform target;
    public float Lerping = -1;
    public float Width = -1;
    public float Height = -1;
    public bool Use = true;
    public static void LockUI(Transform targetUI, Transform origin, float lerping = -1,float width = -1, float height = -1)
    {

        Vector3 pos = renderCam.UnityCam.WorldToScreenPoint(origin.transform.position);

        if (width > 0)
        {
            pos.x = Mathf.Clamp(pos.x, -width, width);
        }

        if (height > 0)
        {
            pos.y = Mathf.Clamp(pos.x, -height, height);
        }

        if (lerping < 0)
        {
            targetUI.position = pos;
        } else
        {
            targetUI.position = Vector3.Lerp(targetUI.position, pos, lerping * Time.deltaTime);
        }

    }

    public void UseLockUI(bool use)
    {
        Use = use;
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void LateUpdate()
    {
        if (!Use)
            return;
        LockUI(transform, target, Lerping, Width, Height);
    }
}
