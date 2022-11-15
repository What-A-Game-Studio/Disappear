using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RendomiserTransform))]
public class RendomiserTransform: EditorWindow
{
    
    // private bool positionX, positionY, positionZ; 
    private bool rotationX, rotationY, rotationZ;
    // private bool scaleX, scaleY,scaleZ;
    [MenuItem("Utils/Randomiser Transform")]
    static void Init()
    {

        RendomiserTransform window =(RendomiserTransform)GetWindow(typeof(RendomiserTransform));
        window.Show();
    }
    private void OnGUI()
    {
        GUILayout.Label("Randomise selected objects", EditorStyles.boldLabel);
        GUILayout.Label("Rotation");
        rotationX = EditorGUILayout.Toggle("X", rotationX);
        rotationY = EditorGUILayout.Toggle("Y", rotationY);
        rotationZ = EditorGUILayout.Toggle("Z", rotationZ);

        if (GUILayout.Button("Randomise Rotation"))
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                go.transform.rotation = quaternion.Euler(GetRdmRotation(go.transform.rotation.eulerAngles));
            }
        }
    }

    private Vector3 GetRdmRotation(Vector3 currentRot)
    {
        return new Vector3(
            rotationX ? UnityEngine.Random.Range(0f, 360f) : currentRot.x,
            rotationY ? UnityEngine.Random.Range(0f, 360f) : currentRot.y,
            rotationZ ? UnityEngine.Random.Range(0f, 360f) : currentRot.y);
    }
        
}