using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed { get; set; }
    [SerializeField] private float minX, maxX;
    private float yRot, xRot;

    public bool CanRotate { get; set; } = true;
    
    private Camera cam;
    private Transform orientation;
    private bool hasOrientatin = false;

    public void SetOrientation(Transform o)
    {
        orientation = o;
        hasOrientatin = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        Debug.Log("Locked by ", this);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasOrientatin)
            return;
        
        transform.position = orientation.position;
        
        if(!CanRotate)
            return;
        
        yRot += Input.GetAxisRaw("Mouse X") * Time.deltaTime * Speed;
        xRot -= Input.GetAxisRaw("Mouse Y") * Time.deltaTime * Speed;

        xRot = Mathf.Clamp(xRot, minX, maxX);

        cam.transform.rotation = Quaternion.Euler(xRot, yRot,0);
        orientation.rotation = Quaternion.Euler(0, yRot,0);
    }

    public Quaternion GetOrientationRotation()
    {
        return orientation.rotation;
    }
}