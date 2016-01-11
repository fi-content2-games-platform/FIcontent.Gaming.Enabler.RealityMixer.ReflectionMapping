using UnityEngine;
using System.Collections.Generic;

public class VisualDebugging : MonoBehaviour
{
    public static VisualDebugging instance;
    Dictionary<string, GameObject> lines;
    public Material mat;
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            lines = new Dictionary<string, GameObject>();
        }
    }
    
    public void AddLine(string key, Vector3[] points, Color color)
    {  
        GameObject go;
        LineRenderer lr;
        
        if (lines.ContainsKey(key))
        {
            go = lines[key];
            lr = go.GetComponent<LineRenderer>();
        }
        else
        {
            go = new GameObject("vdLine_" + key);
            lr = go.AddComponent<LineRenderer>();
            lr.material = mat;
            lr.SetWidth(2, 2);
            lines.Add(key, go);
        }
        lr.SetColors(color, color);
        lr.SetVertexCount(points.Length);
        for (int i=0; i<points.Length; ++i)
        {
            lr.SetPosition(i, points[i]);
        }
    }
    
    public void RemoveLine(string key)
    {
        if (lines.ContainsKey(key))
        {
            Destroy(lines[key]);
            lines.Remove(key);
        }
    }
}
