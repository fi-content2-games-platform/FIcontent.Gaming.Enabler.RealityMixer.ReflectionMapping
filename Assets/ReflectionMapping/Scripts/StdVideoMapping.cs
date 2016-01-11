using UnityEngine;
using System.Collections;
using Vuforia;

public class StdVideoMapping : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	

	
	// Update is called once per frame
	void Update ()
	{
		if (QCARRenderer.Instance.IsVideoBackgroundInfoAvailable ()) {
			GetComponent<Renderer>().material.mainTexture = SettingsUpdaterAbstract.Instance.GetCurrentVideoStream ();
			QCARRenderer.VideoTextureInfo texInfo = QCARRenderer.Instance.GetVideoTextureInfo ();
			
			float scaleFactorX = (float)texInfo.imageSize.x / (float)texInfo.textureSize.x;
			float scaleFactorY = (float)texInfo.imageSize.y / (float)texInfo.textureSize.y;
			
			Mesh mesh = this.GetComponent<MeshFilter> ().mesh;
			Vector2[] uvs = new Vector2[mesh.vertices.Length];
			float rowAmount = Mathf.Sqrt ((float)mesh.vertices.Length); //rowAmount = 11
			int vertexIndex = 0;
			
			//float px = scaleFactorX / 2.0f;
			//float py = scaleFactorY / 2.0f;
			
			//Set the uv coordinates. Note the obscure format.
			for (int y = 0; y < (int)rowAmount; y++) {
				
				//Start at 0 and end at scaleFactorY
				float origV = ((float)y / (rowAmount - 1)) * scaleFactorY;
					
				// for(int x = (int)rowAmount-1; x >= 0; x--) {
				for (int x = 0; x < (int)rowAmount; x++) {
						
					//Start at scaleFactorX and end at 0
					float origU = ((float)(rowAmount - 1 - x) / (rowAmount - 1)) * scaleFactorX;
					
					
					//float U = px + py - origV;
					//float V = py - px + origU;	
					
					uvs [vertexIndex] = new Vector2 (origU, origV);

					vertexIndex++;
				}
			}
			
			
			mesh.uv = uvs;
		}
	}
}
