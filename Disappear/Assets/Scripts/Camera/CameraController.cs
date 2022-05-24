using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float minX, maxX;
    private float yRot, xRot;

    private Camera cam;
    public Transform Orientation { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Orientation.position;
        yRot += Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed;
        xRot -= Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed;

        xRot = Mathf.Clamp(xRot, minX, maxX);

        cam.transform.rotation = Quaternion.Euler(xRot, yRot,0);
        Orientation.rotation = Quaternion.Euler(0, yRot,0);
    }
}