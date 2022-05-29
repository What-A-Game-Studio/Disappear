using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemController : MonoBehaviour
{
    [SerializeField] private float forceAtSpawn = 1.2f;
    private void Awake()
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        gameObject.AddComponent<BoxCollider>();
        transform.Rotate(GetRdmVector(0,360f));
    }

    private Vector3 GetRdmVector(float min, float max)
    {
        return new Vector3(Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max));
    }
}