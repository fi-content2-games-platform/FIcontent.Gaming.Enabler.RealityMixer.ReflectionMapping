using UnityEngine;
using System.Collections;

public class ApplyPerspectiveShift : MonoBehaviour
{
	private SettingsUpdater settingsUpd;
	
	// Use this for initialization
	void Start ()
	{
		settingsUpd = GameObject.FindGameObjectWithTag ("SettingsUpdater").GetComponent<SettingsUpdater> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Matrix4x4 mat = settingsUpd.GetPerspectiveShiftTransformationForDiffuse (this.gameObject);
		GetComponent<Renderer>().material.SetMatrix ("_PerspectiveShiftMatrixDiff", mat);
	
		mat = settingsUpd.GetPerspectiveShiftTransformationForSpecular (this.gameObject);
		GetComponent<Renderer>().material.SetMatrix ("_PerspectiveShiftMatrixSpec", mat);
		GetComponent<Renderer>().material.SetMatrix ("_PerspectiveShiftMatrixSpecIT", mat.inverse.transpose);
	}
}
