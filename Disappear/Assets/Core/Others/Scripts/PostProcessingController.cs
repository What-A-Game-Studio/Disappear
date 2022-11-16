using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace WAG.Core
{
    public class PostProcessingController : MonoBehaviour
    {
        public static PostProcessingController Instance { get; private set; }

        private DepthOfField dof;
        protected Volume volume;

        void Awake()
        {
            if (PostProcessingController.Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (!TryGetComponent(out Volume vol))
            {
                Debug.LogError("PostProcessingController need UnityEngine.Rendering.Volume", this);
                return;
            }

            volume = vol;
        }

        public void SetPostProcessing(VolumeProfile profileVolume)
        {
            volume.profile = profileVolume;
            if (!volume.profile.TryGet(out dof))
            {
                Debug.Log("Volume needs Depth Of Field");
            }

            dof.focalLength.value = 1.0f;
        }

        public void AdaptBlur(float blurValue)
        {
            StartCoroutine(LerpBlur(blurValue, 0.5f));
        }

        public void DeactivateBlur()
        {
            StartCoroutine(LerpBlur(1.0f, 0.5f));
        }

        private IEnumerator LerpBlur(float endValue, float duration)
        {
            float time = 0;
            float startValue = dof.focalLength.value;
            while (time < duration)
            {
                dof.focalLength.value = Mathf.Lerp(startValue, endValue, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            dof.focalLength.value = endValue;
        }
    }
}