using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private ItemDataSO dataSo;
    
    private Rigidbody rb;
    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out rb))
        {
            throw new Exception("ItemsController need a Rigidbody");
        }

        if (dataSo == null)
        {
            throw new Exception("ItemsController need data");
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // MeshFilter mf = CopyComponent<MeshFilter>(data.Model.GetComponent<MeshFilter>(), gameObject);
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        mf.mesh = dataSo.Model.GetComponent<MeshFilter>().sharedMesh;
        MeshRenderer mr = CopyComponent<MeshRenderer>(dataSo.Model.GetComponent<MeshRenderer>(), gameObject);
        // GameObject item = Instantiate(data.Model, transform.position,quaternion.identity);
        // item.transform.parent = transform;
        // item.AddComponent<BoxCollider>();
    }

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        var fields = type.GetFields(flags);
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        FieldInfo[] finfos = type.GetFields(flags);
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos) {
            if (pinfo.CanWrite) {
                try {
                    pinfo.SetValue(dst, pinfo.GetValue(original, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        return dst as T;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}