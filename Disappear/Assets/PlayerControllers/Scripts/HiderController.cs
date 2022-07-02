using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class HiderController : Interactable
{
    private int HiderLife;

    public PlayerController pc { get; private set; }
    private List<Material> hiderMaterial = new List<Material>();

    private float transparencyThreshold;
    private float maxVelocity;
    private float transparency;
    private float oldTransparency = 0;
    private static readonly int Opacity = Shader.PropertyToID("_Opacity");

    public bool IsMine()
    {
        return pc.IsMine();
    }

    protected override void Awake()
    {
        base.Awake();
        pc = GetComponent<PlayerController>();
    }
    
    public void Init(int maxLife, float maxSpeed, float threshold)
    {
        HiderLife = maxLife;
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
    
    protected override void ActionOnInteract(GameObject sender)
    {
        HiderLife--;
        if (HiderLife > 0)
        {
            pc.Teleport();
        }
        else
        {
            GameManager.Instance.HiderQuit(this, false);
        }
    }
}