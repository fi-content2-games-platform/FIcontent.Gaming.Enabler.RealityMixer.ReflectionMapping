using UnityEngine;
using System.Collections;
using Vuforia;

public class UseLightProbeForTexture : MonoBehaviour
{

	// Input: x,y both in [0, 1]
	// Output: mapped to index correctly
	Vector2 VuforiaTexCoordMap (float x, float y, float scaleX, float scaleY)
	{
		float xr = (1.0f - y) * scaleX;
		float yr = (1.0f - x) * scaleY;
		
		return new Vector2 (xr, yr);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (QCARRenderer.Instance.IsVideoBackgroundInfoAvailable ()) {
			GetComponent<Renderer>().material.mainTexture = SettingsUpdaterAbstract.Instance.GetCurrentVideoStream ();	
			Vector4 scale = SettingsUpdaterAbstract.Instance.GetTextureScaleFactor ();
			float scaleFactorX = scale.x;
			float scaleFactorY = scale.y;
			
			
			Mesh mesh = this.GetComponent<MeshFilter> ().mesh;
			Vector2[] uvs = new Vector2[mesh.vertices.Length];
			float rowAmount = Mathf.Sqrt ((float)mesh.vertices.Length); //rowAmount = 11
			int vertexIndex = 0;
			
            
			//Set the uv coordinates
			for (int y = 0; y < (int)rowAmount; y++) {
				
				for (int x = 0; x < (int)rowAmount; x++) {
					
					float r = rowAmount;
					float tcY = y / (r - 1);
					float tcX = x / (r - 1);
					
					uvs [vertexIndex] = VuforiaTexCoordMap (tcX, tcY, scaleFactorX, scaleFactorY);

					vertexIndex++;
				}
			}
			
			mesh.uv = uvs;
		}
	}
}
