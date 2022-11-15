using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSnap : MonoBehaviour
{
    [SerializeField] private Transform positionToSnap;

    void Awake()
    {
        if (!positionToSnap)
        {
            Debug.LogError("Need positionToSnap", this);
            Debug.Break();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = positionToSnap.position;
    }
}