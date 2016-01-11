using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorExtensions
{
    
    [MenuItem("GameObject/Create Empty Under Selected %t", false, 0)]
    static void CreateNewGameObjectAtOriginOfSelected()
    {
        GameObject go = new GameObject("GameObject");
        go.transform.parent = Selection.activeTransform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        Selection.activeGameObject = go;
    }
    
    static Quaternion rotationClipboard;
    
    [MenuItem("CONTEXT/Transform/Copy Local Rotation")]
    static void CopyTransformLocalRotation(MenuCommand command)
    {
        Transform t = (Transform)command.context;
        rotationClipboard = t.localRotation;
    }
    
    [MenuItem("CONTEXT/Transform/Paste Local Rotation")]
    static void PasteTransformLocalRotation(MenuCommand command)
    {
        Transform t = (Transform)command.context;
        t.localRotation = rotationClipboard;
    }
    
}
