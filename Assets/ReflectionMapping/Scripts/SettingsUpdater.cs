using UnityEngine;
using System.Collections.Generic;
using Vuforia;

public class SettingsUpdater : SettingsUpdaterAbstract
{
	// To be assigned from the inspector
	public PhysicalLightProbe specularLightProbe;
	public PhysicalLightProbe diffuseLightProbe;

	private Texture2D videoTexture;
	public override Texture GetCurrentVideoStream ()
	{
		return videoTexture;
	}
 
 
	// Other variables
	//Vector3 currentAcceleration, lowPassAccel;
	//float shakingThreshold, lowPassFilter;
	public float diffuseVsSpecularSlider;
    
	protected override void Start ()
	{
		base.Start ();


		diffuseVsSpecularSlider = 0.5f;
     
		// Setup video texture access (on the GPU side):
     
		// Ask the renderer to stop drawing the videobackground.
		backGroundRenderer.enabled = false;

		// Create texture of size 0 that will be updated in the plugin (we allocate buffers in native code)
		videoTexture = new Texture2D (0, 0, TextureFormat.ARGB32, false);
		//videoTexture = new RenderTexture(1024, 512, 1, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
     
		videoTexture.filterMode = FilterMode.Bilinear;
		videoTexture.wrapMode = TextureWrapMode.Clamp;
     
		// Set the native texture ID:
		QCARRenderer.Instance.SetVideoBackgroundTexture (videoTexture, videoTexture.GetNativeTextureID ());
     
		// For updating the scale factor of the video texture (needed for generating correct texture coordinates)
		scaleFactor = new Vector4 (0, 0, 0, 0);
     
		// Set shaking detection variables
		//shakingThreshold = 0.6f;
		//lowPassAccel = ComputeAccurateAcceleration();
		//lowPassFilter = 1.0f / 60.0f;// accelerometer update speed divided by low pass filter kernel width
	}

	public void ResetVideoTexture ()
	{
		QCARRenderer.Instance.SetVideoBackgroundTexture (videoTexture, videoTexture.GetNativeTextureID ());
	}
 
	public Matrix4x4 GetPerspectiveShiftTransformationForSpecular (GameObject g)
	{
		return ComputePerspectiveShift (g.transform.position, specularLightProbe.transform.position);
	}
 
	public Matrix4x4 GetPerspectiveShiftTransformationForDiffuse (GameObject g)
	{
		return ComputePerspectiveShift (g.transform.position, diffuseLightProbe.transform.position);
	}
 
	private Matrix4x4 ComputePerspectiveShift (Vector3 fromPos, Vector3 toPos)
	{
		Matrix4x4 viewMatrix = Camera.main.worldToCameraMatrix;
		Vector3 fromInViewSpace = - (viewMatrix * fromPos);
		Vector3 toInViewSpace = - (viewMatrix * toPos);
		Quaternion quat = Quaternion.FromToRotation (fromInViewSpace.normalized, toInViewSpace.normalized);
     
		return Matrix4x4.TRS (Vector3.zero, quat, Vector3.one);
	}
 
	private void SendUpdatesToAllShaders ()
	{
		Shader.SetGlobalVector ("_TexCoordScale", scaleFactor);
		Shader.SetGlobalFloat ("_SpecularVsDiffuseRatio", diffuseVsSpecularSlider);
	}
 
	public override void OnTrackablesUpdated ()
	{
		// Checking if the iPad is currently shaking (ignored for now)
		//currentAcceleration = ComputeAccurateAcceleration();
		//lowPassAccel = Vector3.Lerp(lowPassAccel, currentAcceleration, lowPassFilter);
		//float accelerationMagn =  (currentAcceleration - lowPassAccel).magnitude;
		//bool isNotShaking = accelerationMagn < shakingThreshold;
     
		// Update tex coord scale parameters
		QCARRenderer.VideoTextureInfo texInfo = QCARRenderer.Instance.GetVideoTextureInfo (); 
        
//        Debug.Log ("imageSize: " + texInfo.imageSize.x + "x" + texInfo.imageSize.y);
//        Debug.Log ("textureSize: " + texInfo.textureSize.x + "x" + texInfo.textureSize.y);
        
     
		scaleFactor.x = (float)texInfo.imageSize.x / (float)texInfo.textureSize.x;
		scaleFactor.y = (float)texInfo.imageSize.y / (float)texInfo.textureSize.y;
     
//        Debug.Log("screen size:" + Screen.width + "x"+ Screen.height);
        
		// Update screen/camera size
		camW = Screen.width;
		camH = Screen.height;
//        Debug.Log ("camH: " + camH);
        
     
		// This updates the data available to all shaders
		SendUpdatesToAllShaders ();
     
     
		// Some debugging info written out
		//Debug.Log( (isTrackedNow ? "Tracking; " : "Lost; ") /*+ (isNotShaking?"Steady; ":"Shaking; ")*/ + "\nSpec: " + (isCurrentSpecularRegionValid?"Y":"N") + "\nDiff: " + (isCurrentDiffuseRegionValid?"Y":"N") /*+"\nAccel. Magn.: " + accelerationMagn*/);
     
		// How to get access to pixels directly on the CPU
		/*Image cameraImage = CameraDevice.Instance.GetCameraImage(mPixelFormat);

        if (cameraImage != null)
        {
            byte[] pixels = cameraImage.Pixels;
            // Do something with the pixels!
        }*/
	}
 
	Vector3 ComputeAccurateAcceleration ()
	{
		Vector3 acceleration = Vector3.zero;
		float time = 0.0f;
     
		foreach (AccelerationEvent e in Input.accelerationEvents) {
			acceleration += e.acceleration * e.deltaTime;
			time += e.deltaTime;
		}
     
		if (Input.accelerationEventCount != 0) {
			acceleration /= time;
		}
     
		return acceleration;
	}
	
}
