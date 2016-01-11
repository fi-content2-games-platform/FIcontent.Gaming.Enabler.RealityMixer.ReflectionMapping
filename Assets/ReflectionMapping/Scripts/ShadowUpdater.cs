using UnityEngine;
using System.Collections.Generic;

public class ShadowUpdater : MonoBehaviour
{
    
    SortedList<float,GameObject> shadows;
    public LightDirectionDetection lightDirectionComponent;
    
    void Start ()
    {
        shadows = new SortedList<float, GameObject>();
        
        foreach (Transform t in transform)
        {
            shadows.Add(t.localEulerAngles.y, t.gameObject);
        }
    }
    
    void OnPostRender ()
    {
        
//        VisualDebugging.instance.AddLine("x",new Vector3[]{transform.position, transform.position + transform.right*50}, Color.red);
//        VisualDebugging.instance.AddLine("y",new Vector3[]{transform.position, transform.position + transform.up*50}, Color.green);
//        VisualDebugging.instance.AddLine("z",new Vector3[]{transform.position, transform.position + transform.forward*50}, Color.blue);
//        
        Vector3 lightDirection = lightDirectionComponent.ComputeMainLightSourceDirection();
        
        if (lightDirection == Vector3.zero)
        {
            return;
        }
        
        Vector3 lightDirectionFlat = lightDirection;
        lightDirectionFlat.y = 0;
        lightDirectionFlat.Normalize();
        
//        
//        VisualDebugging.instance.AddLine("a",new Vector3[]{transform.position, transform.position + lightDirection*100},Color.magenta);
//        VisualDebugging.instance.AddLine("b",new Vector3[]{transform.position, transform.position + lightDirectionFlat*100},Color.yellow);
        
		
        lightDirection = transform.TransformDirection(lightDirection);
        lightDirectionFlat = transform.TransformDirection(lightDirectionFlat);
        foreach ( GameObject shadow in shadows.Values )
        {
            shadow.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
        
        float angle = Vector3.Angle(Vector3.forward, lightDirectionFlat);
        
        int index = (int)(angle / (360f/shadows.Count));
        //Debug.Log("calculated index: " + index, gameObject);
        
        // if index is last then find closest between first and last
        if ( index == shadows.Keys.Count-1)
        {
            if ( Mathf.Abs( (angle - 360)) < Mathf.Abs(angle - shadows.Keys[index]))
            {
                index = 0;
            }
        }
        // else we compare current and next
        else
        {
            if(Mathf.Abs(shadows.Keys[index] - angle) > Mathf.Abs(shadows.Keys[index+1] - angle))
            {
                index++;
            }
        }
        
        shadows.Values[index].GetComponentInChildren<MeshRenderer>().enabled = true;
        
//        Debug.Log("ld: " + lightDirection + "\nldf: " + lightDirectionFlat + "\nangle: " + angle);
//        
//        // try to find the angle in the list if it's not there then we check the index before and after the location to get the closest
//        int index = List.BinarySearch(shadows.Keys,angle);
//        
//        if (index < 0)
//        {
//            Debug.Log("Key: " + ~index + " Value: " + shadows.Keys[~index]);
//        }
        
        
        
        
    }
}
