using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float sensibilityX;
    [SerializeField] private float sensibilityY;

    private Camera cam;

    private float mouseX;
    private float mouseY;

    private float multiplier = 1f;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Camera tmpCam = GetComponentInChildren<Camera>();
        if (tmpCam == null)
        {
            Debug.LogError("PlayerLook required a Camera in children");
            Debug.DebugBreak();
        }

        cam = tmpCam;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Inputs();

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void Inputs()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensibilityX * multiplier;
        xRotation -= mouseY * sensibilityY * multiplier;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}
