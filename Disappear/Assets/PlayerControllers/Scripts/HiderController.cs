using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class HiderController : MonoBehaviour
{
    private float transparencyThreshold;
    private float maxVelocity;

    private PlayerController pc;
    private List<Material> hiderMaterial = new List<Material>();


    private float transparency;
    private float oldTransparency = 0;
    private static readonly int Opacity = Shader.PropertyToID("_Opacity");

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    public void Init(float maxSpeed, float threshold)
    {
        maxVelocity = maxSpeed;
        transparencyThreshold = threshold;
    }

    public void SetMaterial(SkinnedMeshRenderer[] renderers)
    {
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            Material newMat = renderer.material;
            hiderMaterial.Add(newMat);
        }
    }

    private void Update()
    {
        
        transparency = Mathf.Floor(10 * pc.PlayerVelocity.magnitude / maxVelocity) / 10.0f;
        if (Mathf.Abs(transparency - oldTransparency) > transparencyThreshold)
        {
            foreach (Material mat in hiderMaterial)
                mat.SetFloat(Opacity, Mathf.Clamp(1 - transparency, 0, 1));
            oldTransparency = transparency;
        }
    }
}