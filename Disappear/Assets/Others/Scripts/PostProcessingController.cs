using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingController : MonoBehaviour
{
    public static PostProcessingController Instance { get; private set; }

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
}
