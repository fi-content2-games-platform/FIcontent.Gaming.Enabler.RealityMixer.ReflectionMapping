using UnityEngine;
using System.Collections;
using Vuforia;

public class LightProbePositioning : MonoBehaviour
{
	public Vector3 defaultProbePos;
	public Vector3 defaultProbeScale;
	public ProbeSettings pbSettings;
	private SettingsSerializer<ProbeSettings> pbSettingsSerializer;
	public string settingsName;
	public PhysicalLightProbe lightProbe;
	public string associatedLightProbe;
	public ImageTargetBehaviour imageTarget;
	private bool tracked;

	private MoveLightProbe moveLP;
 
	void Awake ()
	{
		if (lightProbe == null) {
			lightProbe = PhysicalLightProbe.Instances [associatedLightProbe];
		}

		imageTarget = this.transform.parent.GetComponent<ImageTargetBehaviour> ();

		moveLP = lightProbe.GetComponent<MoveLightProbe> ();

		pbSettingsSerializer = new SettingsSerializer<ProbeSettings> (settingsName);

		defaultProbePos = lightProbe.transform.position;
		defaultProbeScale = lightProbe.transform.localScale;
	}

	void Update ()
	{
		if (moveLP.activated)
			return;

		if (imageTarget.CurrentStatus == TrackableBehaviour.Status.TRACKED && !tracked) {
			tracked = true;
			lightProbe.transform.position = this.pbSettings.Position;
			lightProbe.transform.localScale = this.pbSettings.Radius;
		} else 
			tracked = false;
	}
    
	void OnEnable ()
	{                   
		try {
			this.pbSettings = pbSettingsSerializer.Load ();
		} catch {

			//file not found
			this.pbSettings = new ProbeSettings ();         
			this.pbSettings.Position = defaultProbePos;
			this.pbSettings.radius = defaultProbeScale.x;
		}    

		lightProbe.transform.position = this.pbSettings.Position;
		lightProbe.transform.localScale = this.pbSettings.Radius;
                
	}

	[ContextMenu("Reset settings")]
	public void ResetSettings ()
	{
		lightProbe.transform.position = defaultProbePos;
		lightProbe.transform.localScale = defaultProbeScale;
	}

	[ContextMenu("Save settings")]
	public void SaveSettings ()
	{
		pbSettings = new ProbeSettings ();
		pbSettings.Position = lightProbe.transform.position;
		pbSettings.Radius = lightProbe.transform.localScale;
		pbSettingsSerializer.Save (pbSettings);
	}
}
