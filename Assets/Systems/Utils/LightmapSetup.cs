using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightmapSetup : MonoBehaviour
{
    [Range(0,10)][SerializeField] float lightStrenght;

    public void SetLightInfluence(float influence)
    {
        Shader.SetGlobalFloat("_LightMapEmission", influence);
    }
    private void OnValidate()
    {
        SetLightInfluence(lightStrenght);
    }
}
