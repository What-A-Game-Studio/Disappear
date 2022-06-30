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
    private Material playerMaterial;

    private float transparency;
    private float oldTransparency = 0;
    private static readonly int Opacity = Shader.PropertyToID("_Opacity");

    public void Init(Material mat, float maxSpeed, float threshold)
    {
        
        pc = GetComponent<PlayerController>();
        playerMaterial = mat;
        maxVelocity = maxSpeed;
        transparencyThreshold = threshold;
    }

    private void Update()
    {
        transparency = Mathf.Floor(10 * pc.PlayerVelocity.magnitude / maxVelocity) / 10.0f;
        if (Mathf.Abs(transparency - oldTransparency) > transparencyThreshold)
        {
            playerMaterial.SetFloat(Opacity, Mathf.Clamp(1 - transparency, 0, 1));
            oldTransparency = transparency;
        }
    }
}