using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTestCube : Interactable
{
    protected override void ActionOnInteract(GameObject sender)
    {
        Debug.Log("Interacted with " + sender.name);
    }
}
