using System;
using UnityEditor;
using UnityEngine;

namespace WAG.Core
{
    public class ScriptableObjectIdAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
    public class ScriptableObjectIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            if (string.IsNullOrEmpty(property.stringValue))
            {
                Debug.Log("ID generated"); 
                property.stringValue = Guid.NewGuid().ToString().Replace("-","");
            }

            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}