using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class TeamController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private TeamData seeker;
    [SerializeField] private TeamData hider;
    [SerializeField] private Transform meshContainer;
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

    public void SetTeamData(bool isSeeker, PlayerAnimationController pac)
    {
        if (isSeeker)
        {
            teamData = seeker;
        }
        else
        {
            teamData = hider;
            SetHider();
        }


        SetModel(pac);
        SetPostProcessingVolume();
        SetSpeedModifier();
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

        Animator anim= go.GetComponent<Animator>();
        pac.SetAnimator(anim);
        go.transform.localPosition = teamData.ModelOffset;
        FootstepEvent = go.GetComponent<FootstepEvent>();
        FootstepEvent.Init(meshContainer.GetChild(1));
        
    }

    private void SetHider()
    {
        HiderController hc = transform.AddComponent<HiderController>();
        transform.AddComponent<HiderInteractable>();
        hc.Init(teamData.TeamMaterial,7f, 0.2f);
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new NotImplementedException();
    }
}