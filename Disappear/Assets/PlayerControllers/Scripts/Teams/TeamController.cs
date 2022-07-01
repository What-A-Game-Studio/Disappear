using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    [SerializeField] private TeamData seeker;
    [SerializeField] private TeamData hider;
    [SerializeField] private Transform meshContainer;
    private TeamData teamData;
    private SkinnedMeshRenderer[] hiderRenderer;
    public FootstepEvent FootstepEvent { get; protected set; }

    private void Awake()
    {
        if (seeker == null)
        {
            Debug.LogError("Seeker can not be null", this);
            return;
        }

        if (hider == null)
        {
            Debug.LogError("Hider can not be null", this);
            return;
        }
    }

    public void SetTeamData(bool isSeeker, PlayerAnimationController pac)
    {
        teamData = isSeeker ? seeker : hider;


        SetModel(pac);
        SetPostProcessingVolume();
        SetSpeedModifier();

        if (!isSeeker)
        {
            SetHider();
        }
    }

    private void SetSpeedModifier()
    {
        gameObject.GetComponent<PlayerController>().SetTeamSpeedModifier(teamData.SpeedModifier);
    }

    private void SetPostProcessingVolume()
    {
        PostProcessingController.Instance.SetPostProcessing(teamData.PostProcessingVolume);
    }

    private void SetModel(PlayerAnimationController pac)
    {
        Destroy(meshContainer.GetChild(0).gameObject);

        GameObject go = Instantiate(teamData.Model, meshContainer);
        SkinnedMeshRenderer[] smr = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer item in smr)
        {
            item.updateWhenOffscreen = true;
        }

        hiderRenderer = smr;
        Animator anim = go.GetComponent<Animator>();
        pac.SetAnimator(anim);
        go.transform.localPosition = teamData.ModelOffset;
        FootstepEvent = go.GetComponent<FootstepEvent>();
        FootstepEvent.Init(meshContainer.GetChild(1));
    }

    private void SetHider()
    {
        HiderController hc = transform.AddComponent<HiderController>();
        HiderHealthSystem hhs = transform.AddComponent<HiderHealthSystem>();
        hc.Init(7f, 0.2f);
        hc.SetMaterial(hiderRenderer);
        hhs.Init(3);
    }
}