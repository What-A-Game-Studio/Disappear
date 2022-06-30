using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    [SerializeField] private TeamData seeker;
    [SerializeField] private TeamData hider;
    [SerializeField] private Transform meshContainer;
    private TeamData teamData;

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
        teamData = isSeeker? seeker : hider;

        SetModel(pac);

        SetPostProcessingVolume();

        SetSpeedModifier();
    }

    private void SetSpeedModifier()
    {
        PlayerController.MainPlayer.SetTeamSpeedModifier(teamData.SpeedModifier);
    }

    private void SetPostProcessingVolume()
    {
        PostProcessingController.Instance.SetPostProcessing(teamData.PostProcessingVolume);
    }

    private void SetModel(PlayerAnimationController pac)
    {
        for (int i = 0; i < meshContainer.childCount; i++)
            Destroy(meshContainer.GetChild(i).gameObject);

        GameObject go = Instantiate(teamData.Model, meshContainer);
        SkinnedMeshRenderer[] smr = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer item in smr)
        {
            item.updateWhenOffscreen = true;
        }

        go.transform.localPosition = teamData.ModelOffset;
        Animator animator = go.AddComponent<Animator>();
        pac.SetAnimator(animator);
        animator.runtimeAnimatorController = teamData.AnimatorController;
    }
}