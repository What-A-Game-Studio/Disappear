using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelInfos : MonoBehaviour
{
    [SerializeField] private Transform cameraRig;

    public Transform CameraRig => cameraRig;
}
