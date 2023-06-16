using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomizeTransform : MonoBehaviour
{
    public bool randomizeScale;
    public float ScaleMin;
    public float ScaleMax;
    [Space]
    public bool randomizeRotation;

    public void RandomizeScale()
    {
        transform.localScale = Vector3.one * Random.Range(ScaleMin, ScaleMax);
        randomizeScale = false;
    }

    public void RandomizeRotation()
    {
        transform.eulerAngles = new Vector3(0,Random.Range(-360f,360f),0);
        randomizeRotation = false;
    }

    private void OnValidate()
    {
        if (randomizeRotation)
        {
            RandomizeRotation();
        }

        if (randomizeScale)
        {
            RandomizeScale();
        }
    }
}
