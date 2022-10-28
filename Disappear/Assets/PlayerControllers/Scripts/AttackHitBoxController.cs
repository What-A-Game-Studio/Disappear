using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

    public class AttackHitBoxController : MonoBehaviour
    {
        [SerializeField] private LayerMask layerToDetect;
        public GameObject PlayerInRange { get; private set; }
        
        private void OnTriggerEnter(Collider other)
        {
            if (IsLayerToDetect(other))
            {
                Ray r = new Ray(transform.position, (other.transform.position - transform.position).normalized);
                if (Physics.Raycast(r, out RaycastHit hitInfo))
                {
                    if (hitInfo.transform.gameObject.GetInstanceID() == other.gameObject.GetInstanceID())
                    {
                        Debug.Log(other.gameObject.name);
                        PlayerInRange = other.gameObject;
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        
        {
            if (IsLayerToDetect(other))
            {
                if (PlayerInRange && PlayerInRange.GetInstanceID() == other.gameObject.GetInstanceID())
                {
                    PlayerInRange = null;
                }
            }
        }
        
        private bool IsLayerToDetect(Collider other)
        {
            return (layerToDetect.value & (1 << other.gameObject.layer)) > 0;
        }

    }