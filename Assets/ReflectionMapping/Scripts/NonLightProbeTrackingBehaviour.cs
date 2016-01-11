/*==============================================================================
            Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
            All Rights Reserved.
            Qualcomm Confidential and Proprietary
==============================================================================*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A custom handler that implements the ITrackableEventHandler interface.
using Vuforia;


public class NonLightProbeTrackingBehaviour : MonoBehaviour,
                                           ITrackableEventHandler
{
	private bool IsTracking = false;
	public Texture2D diffuseLightProbeTexture;
	public Texture2D specularLightProbeTexture;

	#region PRIVATE_MEMBER_VARIABLES

	private TrackableBehaviour mTrackableBehaviour;
	Vector4 region = new Vector4 (0, 0, 1, 1);

	#endregion // PRIVATE_MEMBER_VARIABLES

	#region UNITY_MONOBEHAVIOUR_METHODS

	void Start ()
	{
		mTrackableBehaviour = GetComponent<TrackableBehaviour> ();
		if (mTrackableBehaviour) {
			mTrackableBehaviour.RegisterTrackableEventHandler (this);
		}

		OnTrackingLost ();
	}

	#endregion // UNITY_MONOBEHAVIOUR_METHODS



	#region PUBLIC_METHODS

	// Implementation of the ITrackableEventHandler function called when the
	// tracking state changes.
	public void OnTrackableStateChanged (
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
		    newStatus == TrackableBehaviour.Status.TRACKED) {
			OnTrackingFound ();
			IsTracking = true;
		} else {
			OnTrackingLost ();
			IsTracking = false;
		}
	}

	#endregion // PUBLIC_METHODS



	#region PRIVATE_METHODS


	private void OnTrackingFound ()
	{
		Shader.SetGlobalTexture ("_DiffuseTex", diffuseLightProbeTexture);
		Shader.SetGlobalTexture ("_SpecularTex", specularLightProbeTexture);
        
		Shader.SetGlobalVector ("_RegionSpecular", region);
		Shader.SetGlobalVector ("_RegionDiffuse", region);
        
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer> ();

		// Enable rendering:
		foreach (Renderer component in rendererComponents) {
			component.enabled = true;
		}

		Debug.Log ("Trackable " + mTrackableBehaviour.TrackableName + " found");
	}

	private void OnTrackingLost ()
	{
		DisableRenderers ();
	}

	void DisableRenderers ()
	{
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer> ();

		// Disable rendering:
		foreach (Renderer component in rendererComponents) {
			component.enabled = false;
		}

	}

	#endregion // PRIVATE_METHODS
}
