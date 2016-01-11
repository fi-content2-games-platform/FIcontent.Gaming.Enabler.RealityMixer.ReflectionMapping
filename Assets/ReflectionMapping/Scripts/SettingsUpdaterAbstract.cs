using UnityEngine;
using System.Collections.Generic;
using Vuforia;

public abstract class SettingsUpdaterAbstract : SingletonBehaviour<SettingsUpdaterAbstract>
{
	protected bool m_HideAll;
	public bool hideAll {
		get { return m_HideAll; }
	}
	public float camW, camH;
	public Texture2D circleContourTexture;
	public Renderer backGroundRenderer;

	// Settings to be read by other gameObjects (if needed):
	protected Vector4 scaleFactor;

	protected virtual void Start ()
	{
		m_HideAll = true;

		// Register itself to receive updates about when tracking data completed
		FindObjectOfType<QCARBehaviour> ().RegisterTrackablesUpdatedCallback (OnTrackablesUpdated);
	}

	public void ShowPointOnScreen (Vector2[] ps)
	{
		float screenRadius = 15;
		foreach (Vector2 p in ps) {
			GUI.DrawTexture (new Rect (p.x - screenRadius, p.y - screenRadius, screenRadius * 2, screenRadius * 2), circleContourTexture, ScaleMode.ScaleToFit, true);
		}
	}

	public void ToggleGUI ()
	{
		m_HideAll = !m_HideAll;

		GameObject [] regions = GameObject.FindGameObjectsWithTag ("Hide");
		
		foreach (GameObject r in regions) {
			r.GetComponent<Renderer>().enabled = !SettingsUpdaterAbstract.Instance.hideAll;
		}
	}

	public Vector4 GetTextureScaleFactor ()
	{
		return scaleFactor;
	}

	public abstract Texture GetCurrentVideoStream ();

	public abstract void OnTrackablesUpdated ();

	// Displying a 2D contour over the light probes on the screen (2D)
	protected virtual void OnGUI ()
	{
		if (!m_HideAll) {
			if (GUI.Button (new Rect (Screen.width - 350, Screen.height - 200, 150, 80), "Toggle previews")) {
				GameObject [] regions = GameObject.FindGameObjectsWithTag ("Hide");
				foreach (GameObject r in regions) {
					r.GetComponent<MeshRenderer> ().enabled = !r.GetComponent<MeshRenderer> ().enabled; 
				}
			}
			if (GUI.Button (new Rect (Screen.width - 350, Screen.height - 100, 150, 80), "Hide GUI")) {
				ToggleGUI ();
			}
			
			if (GUI.Button (new Rect (Screen.width - 350, Screen.height - 400, 150, 50), "Store Settings")) {
				var lpps = FindObjectsOfType (typeof(LightProbePositioning)) as LightProbePositioning[];
				foreach (var l in lpps)
					l.SaveSettings ();
			}
			if (GUI.Button (new Rect (Screen.width - 350, Screen.height - 350, 150, 50), "Reset Settings")) {
				var lpps = FindObjectsOfType (typeof(LightProbePositioning)) as LightProbePositioning[];
				foreach (var l in lpps)
					l.ResetSettings ();
			}
			
			//diffuseVsSpecularSlider = GUI.HorizontalSlider(new Rect(50, Screen.height - 300, 600, 50), diffuseVsSpecularSlider, 0.0f, 1.0f);
		}
	}
}