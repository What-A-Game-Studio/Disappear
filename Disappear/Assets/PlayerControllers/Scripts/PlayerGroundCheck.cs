using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class PlayerGroundCheck : GroundCheck<PlayerController>
{
    private FootstepEvent stepEvent;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        
        stepEvent = transform.parent.GetComponent<TeamController>().FootstepEvent;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        
        if (other.TryGetComponent(out SurfaceController sc))
        {
            stepEvent.ChangeSurfaceType(sc.SurfaceType);
        }
    }
}