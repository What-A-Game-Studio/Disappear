using UnityEngine;
using UnityEngine.Rendering.Universal;
using WAG.Audio.Footsteps;
using WAG.Core;
using WAG.Player.Models;

namespace WAG.Player.Teams
{
    public class NGOTeamController : MonoBehaviour
    {
        [SerializeField] private bool isSeeker = true;
        [SerializeField] private Transform meshContainer;

        [Header("Seeker Parameters")] [SerializeField]
        private TeamData seeker;

        [Header("Hider Parameters")] [SerializeField]
        private TeamData hider;

        [SerializeField] private int hiderLife;
        [SerializeField] private float hiderTransparencySpeed;
        [SerializeField] private float hiderTransparencyThreshold;

        private TeamData teamData;
        private NGOPlayerController pc;

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

        public ModelInfos SetTeamData(bool isSeeker, NGOPlayerController pc)
        {
            this.pc = pc;
            this.isSeeker = isSeeker;
            teamData =  isSeeker ? seeker : hider;

            SkinnedMeshRenderer[] hiderRenderers;
            DecalProjector hiderShadow;
            ModelInfos mi = SetModel(out hiderRenderers, out hiderShadow);
            SetPostProcessingVolume();
            SetSpeedModifier();

            if (!isSeeker)
                SetHider(hiderRenderers, hiderShadow);

            return mi;
        }

        private void SetSpeedModifier()
        {
            gameObject.GetComponent<NGOPlayerController>().SetTeamSpeedModifier(teamData.SpeedModifier);
        }

        private void SetPostProcessingVolume()
        {
            if (pc.IsMine() && PostProcessingController.Instance)
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
            go.transform.localPosition = teamData.ModelOffset;
            FootstepEvent = go.GetComponent<FootstepEvent>();
            // FootstepEvent.Init(meshContainer.GetChild(1));
            return go.GetComponent<ModelInfos>();
        }

        private void SetHider(SkinnedMeshRenderer[] hiderRenderers, DecalProjector hiderShadow)
        {
            HiderController hc = gameObject.AddComponent<HiderController>();
            hc.Init(hiderLife, hiderTransparencySpeed, hiderTransparencyThreshold);
            hc.SetMaterials(hiderRenderers, hiderShadow);
        }
    }
}