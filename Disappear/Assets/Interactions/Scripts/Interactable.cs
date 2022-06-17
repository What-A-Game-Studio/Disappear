using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Overriding UnityEvent to add parameters 
/// </summary>
[System.Serializable]
public class InteractionEvent : UnityEvent<GameObject> {}

public abstract class Interactable : MonoBehaviour
{
    public InteractionEvent onInteract = new InteractionEvent();

    protected virtual void Awake()
    {
        onInteract.AddListener(ActionOnInteract);
    }
    
    protected abstract void ActionOnInteract(GameObject sender);
}
