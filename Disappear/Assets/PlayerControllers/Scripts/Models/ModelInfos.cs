using UnityEngine;

public class ModelInfos : MonoBehaviour
{
    [SerializeField] private Transform cameraRig;
    public Transform CameraRig => cameraRig;
    
    [SerializeField] private Transform objectHolder;
    public Transform ObjectHolder => objectHolder;
        
}
