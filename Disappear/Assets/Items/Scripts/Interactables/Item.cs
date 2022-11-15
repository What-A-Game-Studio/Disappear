using UnityEngine;
using WAG.Interactions;

public abstract class Item : Interactable
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int Value { get; private set; }
    [field: SerializeField] public int Weight { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (Name == "")
        {
            Name = gameObject.name;

        }
    }
}
