
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemController : MonoBehaviour
{
    [SerializeField] private float forceAtSpawn = 1.2f;
    private Rigidbody rb;
    [SerializeField] private float timeToCheckIfItemStill = 1f;
    [SerializeField] private float minimalAngularVelocityMagnitude = 1f;
    [SerializeField] private float currentAngularVelocityMagnitude;
    public ItemDataSO ItemData { get; set; }

    private void Awake()
    {
        tag = "Interactable";
    }

    private void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        gameObject.AddComponent<BoxCollider>();
        PickableItem pui = gameObject.AddComponent<PickableItem>();
        pui.ItemController = this;
        rb.AddTorque(GetRdmVector(-1,1) * forceAtSpawn, ForceMode.Impulse);
        // transform.Rotate(GetRdmVector(0,360f));
    }

    private void FixedUpdate()
    {
        currentAngularVelocityMagnitude = rb.angularVelocity.magnitude;
    }

    IEnumerator CheckItemStill()
    {
        yield return new WaitForSeconds(timeToCheckIfItemStill);
        //if(rb.angularVelocity.magnitude)
    }

    private Vector3 GetRdmVector(float min, float max)
    {
        return new Vector3(Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max));
    }
}