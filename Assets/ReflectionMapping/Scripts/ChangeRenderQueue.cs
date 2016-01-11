using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeRenderQueue : MonoBehaviour
{
 
    public int renderQueueModification;
    
	void Start ()
	{
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.material.renderQueue += renderQueueModification;
        }
        
	}
}
