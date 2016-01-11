using UnityEngine;
using System.Collections;

public class MoveLightProbe : MonoBehaviour
{
	
	// Set a text from the inspector to display position and radius information
	public int xsetInspector;
	
	void Start ()
	{
		distnow = 0;
		distbefore = 0;
		started = false;
		
		activated = false;
	}
	
	public bool activated;
	
	private bool started;
	private float distnow;
	private float distbefore;
	
	
	public void deactivate ()
	{
		activated = false;
		started = false;
		distbefore = 0;
		distnow = 0;
	}
	
	
	void OnGUI ()
	{
		if (!SettingsUpdaterAbstract.Instance.hideAll) {
			if (activated) {
				GUILayout.Label (name + " position: " + transform.localPosition);
				GUILayout.Label (name + " scale: " + transform.localScale);
			}
			if (GUI.Button (new Rect (xsetInspector, Screen.height / 2, 150, 80), gameObject.name)) {
				activated = !activated;
				
				if (!activated) {
					started = false;
					distbefore = 0;
					distnow = 0;
					//Debug.Log (name + " " + transform.localPosition + " " + transform.localScale);
				}
				
				if (activated) {
					// iterate through all other light probes and deactivate them!
					GameObject [] os = GameObject.FindGameObjectsWithTag ("RealLightProbeUpdater");
					foreach (GameObject g in os) {
						if (g != this.gameObject) {
							Debug.Log (g);
							g.GetComponent<MoveLightProbe> ().deactivate ();							
						}
					}
				}
			}
			
		}
	}
	
	
	// Calibrating light probe position in terms of marker position using X,Y one finger swipes for moving and two-finger pinching for radius control
	void FixedUpdate ()
	{
		float dist;

#if UNITY_EDITOR
		if (activated)
			transform.Translate (new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0f));
#endif
		
		if (activated) {
			if (Input.touchCount > 0 && Input.touchCount < 2 && Input.GetTouch (0).phase == TouchPhase.Moved) {
				
				Vector3 cup = Camera.main.transform.up;
				Vector3 cright = Camera.main.transform.right;
				
				Vector3 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
				
//				Debug.Log ("touch delta pos " + touchDeltaPosition);
//				Debug.Log ("cam up " + cup);
//				Debug.Log ("cam right " + cright);
				
				Vector3 toTranslate = cup * (touchDeltaPosition.y * 0.01f) + cright * (touchDeltaPosition.x * 0.01f);
				
				transform.Translate (toTranslate);
			}
		
			if (Input.touchCount == 2) {
				if (!started && Input.GetTouch (0).phase == TouchPhase.Moved && Input.GetTouch (1).phase == TouchPhase.Moved) {
					dist = Vector2.Distance (Input.GetTouch (0).position, Input.GetTouch (1).position) * 0.15f;		
				
					distbefore = dist;
					distnow = dist;
					started = true;
				}
				
				if (started) {
					dist = Vector2.Distance (Input.GetTouch (0).position, Input.GetTouch (1).position) * 0.15f;		
				
					distbefore = distnow;
					distnow = dist;
					
					if (Mathf.Abs (distnow - distbefore) > 0.01f) {
						float diff = distnow - distbefore;
						
						float currentScale = transform.localScale.x;
							
						transform.localScale = Vector3.one * (currentScale + diff);			
					}
				}
				
				if (Input.GetTouch (0).phase == TouchPhase.Ended || Input.GetTouch (1).phase == TouchPhase.Ended) {
					started = false;
					distbefore = 0;
					distnow = 0;
				}
			}		
		}
	}
		
	
    
	void OnDrawGizmos ()
	{
		Gizmos.DrawSphere (transform.position, transform.localScale.x / 2);
	}
}
