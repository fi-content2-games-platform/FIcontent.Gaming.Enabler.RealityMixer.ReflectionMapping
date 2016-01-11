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


public class LightProbeTrackingBehaviour : MonoBehaviour, ITrackableEventHandler
{
	

	#region PRIVATE_MEMBER_VARIABLES

	private bool IsTracking = false;
	private TrackableBehaviour mTrackableBehaviour;
	private LightProbePositioning[] lightProbePositions;
	private List<PhysicalLightProbe> lightProbes;

	#endregion // PRIVATE_MEMBER_VARIABLES

	#region UNITY_MONOBEHAVIOUR_METHODS

	void Start ()
	{
		lightProbes = new List<PhysicalLightProbe> ();
        
		GameObject[] lightProbeGOs = GameObject.FindGameObjectsWithTag ("RealLightProbeUpdater");
        
		foreach (GameObject g in lightProbeGOs) {
			lightProbes.Add (g.GetComponent<PhysicalLightProbe> ());
		}
        
		lightProbePositions = GetComponentsInChildren<LightProbePositioning> ();
        
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
			IsTracking = false; // do this here as the OnTrackingLost is called once before the GUIHandler is initialised.
		}
	}

	#endregion // PUBLIC_METHODS



	#region PRIVATE_METHODS


	private void OnTrackingFound ()
	{
        
		foreach (LightProbePositioning lpp in lightProbePositions) {
			lpp.enabled = true;
		}
		foreach (PhysicalLightProbe lp in lightProbes) {
			lp.enabled = true;
		}
        
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer> ();

		// Enable rendering:
		foreach (Renderer component in rendererComponents) {
			component.enabled = true;
		}

	}

	private void OnTrackingLost ()
	{
		DisableProbesAndRenderers ();
	}

	void DisableProbesAndRenderers ()
	{
		foreach (LightProbePositioning lpp in lightProbePositions) {
			lpp.enabled = false;
		}
		foreach (PhysicalLightProbe lp in lightProbes) {
			if (lp)
				lp.enabled = false;
		}
        
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer> ();

		// Disable rendering:
		foreach (Renderer component in rendererComponents) {
			component.enabled = false;
		}
        
		Debug.Log ("Trackable " + mTrackableBehaviour.TrackableName + " lost");
	}

	#endregion // PRIVATE_METHODS

}
