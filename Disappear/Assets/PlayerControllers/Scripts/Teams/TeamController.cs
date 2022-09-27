using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using n;
public class TeamController : MonoBehaviour
{
    [SerializeField] private TeamData seeker;
    [SerializeField] private TeamData hider;
    [SerializeField] private Transform meshContainer;
    private PhotonView pv;
    private TeamData teamData;

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

    public void SetTeamData(bool isSeeker, PhotonView photonView, ref Transform cameraRig)
    {
        teamData = isSeeker ? seeker : hider;
        pv = photonView;

        SkinnedMeshRenderer[] hiderRenderers;
        DecalProjector hiderShadow;
        SetModel(out hiderRenderers, out hiderShadow, ref cameraRig);
        SetPostProcessingVolume();
        SetSpeedModifier();

        if (!isSeeker)
        {
            SetHider(hiderRenderers, hiderShadow);
            if (pv.IsMine)
                MultiplayerManager.Instance.SetGameTitle("Hider");
        }
        else if (pv.IsMine)
        {
            MultiplayerManager.Instance.SetGameTitle("Seeker");
        }
    }


    private void SetSpeedModifier()
    {
        gameObject.GetComponent<PlayerController>().SetTeamSpeedModifier(teamData.SpeedModifier);
    }

    private void SetPostProcessingVolume()
    {
        if (pv.IsMine)
            PostProcessingController.Instance.SetPostProcessing(teamData.PostProcessingVolume);
    }

    private void SetModel(out SkinnedMeshRenderer[] hiderRenderers,
        out DecalProjector hiderShadow, ref Transform cameraRig)
    {
        hiderRenderers = null;
        hiderShadow = null;

        // Destroy(meshContainer.GetChild(0).gameObject);
        GameObject go = Instantiate(teamData.Model, meshContainer);
        cameraRig = go.GetComponent<ModelInfos>().CameraRig;
        SkinnedMeshRenderer[] smr = go.GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < go.transform.childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            if (child.TryGetComponent(out SkinnedMeshRenderer renderer))
            {
                renderer.updateWhenOffscreen = true;
            }

            if (child.TryGetComponent(out DecalProjector proj))
            {
                hiderShadow = proj;
            }
        }

        foreach (SkinnedMeshRenderer item in smr)
        {
            item.updateWhenOffscreen = true;
        }

        hiderRenderers = smr;
        // Animator anim = go.GetComponent<Animator>();
        // pac.SetAnimator(anim);
        // go.transform.localPosition = teamData.ModelOffset;
        // FootstepEvent = go.GetComponent<FootstepEvent>();
        // FootstepEvent.Init(meshContainer.GetChild(1));
    }

    private void SetHider(SkinnedMeshRenderer[] hiderRenderers, DecalProjector hiderShadow)
    {
        HiderController hc = transform.AddComponent<HiderController>();
        hc.Init(3, 7f, 0.2f);
        hc.SetMaterials(hiderRenderers, hiderShadow);
    }
}