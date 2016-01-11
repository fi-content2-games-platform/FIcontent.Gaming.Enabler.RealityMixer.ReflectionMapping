using UnityEngine;
using System.Collections;
using Vuforia;

public class TrackableDetector : MonoBehaviour, ITrackableEventHandler
{
	public TrackableBehaviour mTrackableBehaviour;
	public bool detected;

	public event TrackerDetectedHandler OnFound, OnLost;
	public delegate void TrackerDetectedHandler ();

	void Start ()
	{
		if (!mTrackableBehaviour)
			mTrackableBehaviour = GetComponent<TrackableBehaviour> ();

		if (mTrackableBehaviour) {
			mTrackableBehaviour.RegisterTrackableEventHandler (this);
		} else {
			Debug.LogError ("No trackable behaviour found for this object!", this);
		}
	}

    #region ITrackableEventHandler implementation

	public void OnTrackableStateChanged (TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
	{
		this.detected = newStatus == TrackableBehaviour.Status.TRACKED;

		if (this.detected) {
			if (OnFound != null)
				OnFound ();
		} else {
			if (OnLost != null)
				OnLost ();
		}
	}

    #endregion

}
