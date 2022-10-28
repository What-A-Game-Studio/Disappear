
using Audio.Scripts.Footsteps;
using Photon.Pun;
using SteamAudio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class TeamController : MonoBehaviour
{
    [SerializeField] private bool isSeeker = true;
    [SerializeField] private Transform meshContainer;

    [Header("Seeker Parameters")]
    [SerializeField] private TeamData seeker;
    
    [Header("Hider Parameters")]
    [SerializeField] private TeamData hider;

    [SerializeField] private int hiderLife;
    [SerializeField] private float hiderTransparencySpeed;
    [SerializeField] private float hiderTransparencyThreshold;
    
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

        teamData = seeker;
    }

    public ModelInfos SetTeamData(PhotonView photonView)
    {
        teamData = isSeeker ? seeker : hider;
        pv = photonView;

        SkinnedMeshRenderer[] hiderRenderers;
        DecalProjector hiderShadow;
        ModelInfos mi = SetModel(out hiderRenderers, out hiderShadow);
        SetPostProcessingVolume();
        SetSpeedModifier();

        if (!isSeeker)
        {
            SetHider(hiderRenderers, hiderShadow);
        }

        return mi;
    }


    private void SetSpeedModifier()
    {
        gameObject.GetComponent<PlayerController>().SetTeamSpeedModifier(teamData.SpeedModifier);
    }

    private void SetPostProcessingVolume()
    {
        if (pv.IsMine && PostProcessingController.Instance)
                PostProcessingController.Instance.SetPostProcessing(teamData.PostProcessingVolume);
    }

    private ModelInfos SetModel(out SkinnedMeshRenderer[] hiderRenderers,
        out DecalProjector hiderShadow)
    {
        hiderRenderers = null;
        hiderShadow = null;

        // Destroy(meshContainer.GetChild(0).gameObject);
        GameObject go = Instantiate(teamData.Model, meshContainer);
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
        return go.GetComponent<ModelInfos>();
    }

    private void SetHider(SkinnedMeshRenderer[] hiderRenderers, DecalProjector hiderShadow)
    {
        HiderController hc = transform.AddComponent<HiderController>();
        hc.Init(hiderLife, hiderTransparencySpeed, hiderTransparencyThreshold);
        hc.SetMaterials(hiderRenderers, hiderShadow);
    }
}