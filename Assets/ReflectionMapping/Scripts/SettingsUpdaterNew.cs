using UnityEngine;
using System.Collections.Generic;
using Vuforia;

public class SettingsUpdaterNew : SettingsUpdaterAbstract
{

	public override Texture GetCurrentVideoStream ()
	{
		return backGroundRenderer.material.mainTexture;
	}

	public override void OnTrackablesUpdated ()
	{
		QCARRenderer.VideoTextureInfo texInfo = QCARRenderer.Instance.GetVideoTextureInfo (); 
		
		scaleFactor.x = (float)texInfo.imageSize.x / (float)texInfo.textureSize.x;
		scaleFactor.y = (float)texInfo.imageSize.y / (float)texInfo.textureSize.y;
		
		// Update screen/camera size
		camW = Screen.width;
		camH = Screen.height;
		
		// This updates the data available to all shaders
		Shader.SetGlobalVector ("_TexCoordScale", scaleFactor);
		Shader.SetGlobalFloat ("_SpecularVsDiffuseRatio", .5f);
	}

	protected override void Start ()
	{
		base.Start ();
	}
}