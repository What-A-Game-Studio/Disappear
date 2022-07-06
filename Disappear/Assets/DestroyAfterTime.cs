using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DestroyAfterTime : MonoBehaviour
{
    private DecalProjector dp;
    [SerializeField] private float destroyDelay;

    private void Awake()
    {
        dp = GetComponent<DecalProjector>();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeOutFootprints(destroyDelay, 0));
    }

    private IEnumerator FadeOutFootprints(float duration, float endValue)
    {
        float time = 0;
        float startValue = dp.fadeFactor;
        while (time < duration)
        {
            dp.fadeFactor = Mathf.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        dp.fadeFactor = endValue;
        Destroy(gameObject);
    }
}