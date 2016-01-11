using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

public class PhysicalLightProbe : MonoBehaviour//, ITrackerEventHandler
{
	private static Dictionary<string,PhysicalLightProbe> instances = new Dictionary<string,PhysicalLightProbe> ();
    
	public static Dictionary<string,PhysicalLightProbe> Instances {
		get { return instances; }
	}
    
	bool videoFitWidth;
	float scaleModification;
    
	private bool isCurrentRegionValid;
	private RenderTexture lastValidVideoTexture;
	private Vector4 lastValidRegion;
	private Vector4 currentRegion;
    
	public string nameForShaders;
	public float minimalRadius;
	public float maximalRadius;
	public Vector2 a, b, c, d; // corners
	public Vector2 center;
	public bool isEntireBoundingRectWithinScreenBoundaries;
	public bool isBeingCopied;

	public bool Initialised {
		get{ return textureInitialised; }
	}
	private bool textureInitialised = false;
    
	public void OnInitialized ()
	{
	}
    
	void Start ()
	{
		instances.Add (nameForShaders, this);
	}
    
	void OnDestroy ()
	{
		// This isn't a likely case but unregister as an instance if destroyed
		instances.Remove (nameForShaders);
	}
    
	void OnDisable ()
	{
		InvalidateCircleInfo ();
	}
 
	void Update ()
	{
		if (textureInitialised) {
			if (QCARRenderer.Instance.IsVideoBackgroundInfoAvailable ()) {
				// Update the properties of the light probe regions
				ComputeCircleInfo ();
             
				// Update the regions of the light probes to be used in shaders
				UpdateRegion ();
             
				// Saving textures here
				UpdateTexture ();
                
				SendUpdatesToAllShaders ();
                
			}
		} else {
			if (QCARRenderer.Instance.IsVideoBackgroundInfoAvailable ()) {
				QCARRenderer.VideoTextureInfo texInfo = QCARRenderer.Instance.GetVideoTextureInfo (); 
				lastValidVideoTexture = new RenderTexture (texInfo.textureSize.x, texInfo.textureSize.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
				lastValidVideoTexture.filterMode = FilterMode.Bilinear;
				lastValidVideoTexture.wrapMode = TextureWrapMode.Clamp;
                
				lastValidVideoTexture.Create ();
                
				// We have to handle the case where the video aspect and screen aspect don't match
				float camAspect = SettingsUpdaterAbstract.Instance.camW / SettingsUpdaterAbstract.Instance.camH;
				float vidAspect = (float)texInfo.imageSize.x / texInfo.imageSize.y;
                
				videoFitWidth = camAspect > vidAspect;
                
				if (videoFitWidth) {
					scaleModification = 1.0f / camAspect * vidAspect;
				} else {
					scaleModification = camAspect * (1.0f / vidAspect);
				}
                
				textureInitialised = true;
			}
		}
        
	}
    
	void OnGUI ()
	{
		if (!SettingsUpdaterAbstract.Instance.hideAll) {
			// Drawing the rectangle on top using the radius as half side length
			Rect probeContour = new Rect (0, 0, minimalRadius * 2, minimalRadius * 2);
			probeContour.center = center;
			GUI.DrawTexture (probeContour, SettingsUpdaterAbstract.Instance.circleContourTexture, ScaleMode.ScaleToFit, true);
     
			SettingsUpdaterAbstract.Instance.ShowPointOnScreen (new Vector2[]{a, b, c, d});
		}
	}
    
	/// <summary>
	/// Computes the circle on the texture that represents the light probes area.
	/// </summary>
	public void ComputeCircleInfo ()
	{
		a = GetScreenPoint (transform.localPosition + Camera.main.transform.up * transform.localScale.x);
		b = GetScreenPoint (transform.localPosition - Camera.main.transform.up * transform.localScale.x);
		c = GetScreenPoint (transform.localPosition + Camera.main.transform.right * transform.localScale.x);
		d = GetScreenPoint (transform.localPosition - Camera.main.transform.right * transform.localScale.x);
     
		// Center of "circle"
		center = (a + b + c + d) / 4.0f;
     
		// Weird scaling issue fixed here
		HalfenLengthFromOrigin (center, ref a);
		HalfenLengthFromOrigin (center, ref b);
		HalfenLengthFromOrigin (center, ref c);
		HalfenLengthFromOrigin (center, ref d);
     
		// Radiuses of "circle"
		float da = Vector2.Distance (a, center);
		float db = Vector2.Distance (b, center);
		float dc = Vector2.Distance (c, center);
		float dd = Vector2.Distance (d, center);
     
		minimalRadius = Mathf.Min (da, db, dc, dd);
		maximalRadius = Mathf.Max (da, db, dc, dd);
     
		// Is the entire bounding "square" contained within screen boundaries
		isEntireBoundingRectWithinScreenBoundaries = 
                IsPointWithinScreenBoundaries (a) && 
			IsPointWithinScreenBoundaries (b) && 
			IsPointWithinScreenBoundaries (c) && 
			IsPointWithinScreenBoundaries (d);
        
		isCurrentRegionValid = isEntireBoundingRectWithinScreenBoundaries;
		isBeingCopied = isEntireBoundingRectWithinScreenBoundaries;
	}
    
	/// <summary>
	/// Call this to invalidate the info created by ComputeCircleInfo when the image is no longer tracked.
	/// </summary>
	public void InvalidateCircleInfo ()
	{
		isCurrentRegionValid = false;
		isBeingCopied = false;
	}
 
	public void UpdateRegion ()
	{
		// Setting the texture coordinates of the region of the passed in center with radius
		currentRegion.x = (center.x - minimalRadius) / SettingsUpdaterAbstract.Instance.camW;
		currentRegion.y = (center.y + minimalRadius) / SettingsUpdaterAbstract.Instance.camH;
		currentRegion.z = (minimalRadius * 2.0f) / SettingsUpdaterAbstract.Instance.camW;
		currentRegion.w = -(minimalRadius * 2.0f) / SettingsUpdaterAbstract.Instance.camH;
        
		if (videoFitWidth) { // in this case the video texture needs to fit to the width of the screen
			currentRegion.y = ((currentRegion.y - 0.5f) * scaleModification) + 0.5f; // scale region start point from the center
			currentRegion.w *= scaleModification;
		} else { // in this case the video texture needs to fit to the height of the screen.
			currentRegion.x = ((currentRegion.x - 0.5f) * scaleModification) + 0.5f; // scale region start point from the center
			currentRegion.z *= scaleModification;
		}
        
	}
    
	public void UpdateTexture ()
	{
		if (isCurrentRegionValid) {
			lastValidRegion = currentRegion;
			Texture video = SettingsUpdaterAbstract.Instance.GetCurrentVideoStream ();
            
			Graphics.Blit (video, lastValidVideoTexture);
		}
	}
    
	public void SendUpdatesToAllShaders ()
	{
		// Send data to all shaders
		Shader.SetGlobalTexture ("_" + nameForShaders + "Tex", GetVideoTextureReference ());
     
		Shader.SetGlobalVector ("_Region" + nameForShaders, GetRegion ());
	}
 
	public Vector4 GetRegion ()
	{
		return lastValidRegion;
	}
 
	private bool IsPointWithinScreenBoundaries (Vector2 p)
	{
		return p.x >= 0 && p.y >= 0 && p.x <= Screen.width && p.y <= Screen.height;
	}

	public Texture GetVideoTextureReference ()
	{
		return lastValidVideoTexture;
	}
    
	public Vector2 GetScreenPoint (Vector3 worldPoint)
	{
		Vector3 a = Camera.main.WorldToScreenPoint (worldPoint);
		a.y = Camera.main.pixelHeight - a.y;
		return a;
	}   
    
	public void HalfenLengthFromOrigin (Vector2 origin, ref Vector2 p)
	{   
		Vector2 dif = p - origin;
		float dist = dif.magnitude / 2.0f;
		dif.Normalize ();
		dif *= dist;
		p = origin + dif;
	}
}
