using UnityEngine;
using System.Collections;

public class DisableComponentForSeconds : MonoBehaviour
{
    public MonoBehaviour componentToDisable;
    public float time;

	void OnEnable ()
	{
	    StartCoroutine(DisableComponent());
	}
	
	IEnumerator DisableComponent ()
	{
	    componentToDisable.enabled = false;
        yield return new WaitForSeconds(time);
        componentToDisable.enabled = true;
        enabled = false;
	}
}
