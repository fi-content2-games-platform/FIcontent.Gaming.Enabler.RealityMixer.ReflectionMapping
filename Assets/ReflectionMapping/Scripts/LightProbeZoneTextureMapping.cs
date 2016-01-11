using UnityEngine;
using System.Collections;

public class LightProbeZoneTextureMapping : MonoBehaviour
{
	
	public PhysicalLightProbe lightProbe;
	Vector2[] origUVs;
	
	void Start ()
	{
		origUVs = GetComponent<MeshFilter> ().mesh.uv;
	}
	
	// Input: x,y both in [0, 1]
	// Output: mapped to index correctly
	Vector2 VuforiaTexCoordMap (Vector2 uv)
	{
		float x = uv.x * scaleFactorX;
		float y = uv.y * scaleFactorY;
		
		return new Vector2 (x, y);
	}
	
	Vector2 ZoneTexCoordMap (Vector2 uv) // probably should combine it with the one above
	{
		float x = uv.x * _w + _x;
		float y = uv.y * _h + _y;
		
		return new Vector2 (x, y);
	}
	
	string ToString (Vector2 a)
	{
		return "(" + a.x + ", " + a.y + ")";
	}
	
	private float _x, _y, _w, _h;
	private float scaleFactorX, scaleFactorY;
				
	// Update is called once per frame
	void Update ()
	{
		if (!lightProbe) {
			Debug.LogError ("No Probe setup for preview: " + name, gameObject);
		}
		if (lightProbe.Initialised) {
			GetComponent<Renderer>().material.mainTexture = lightProbe.GetVideoTextureReference ();
			Vector4 region = lightProbe.GetRegion ();
			_x = region.x;
			_y = region.y;
			_w = region.z;
			_h = region.w;
			
			Vector4 scale = SettingsUpdaterAbstract.Instance.GetTextureScaleFactor ();
			scaleFactorX = scale.x;
			scaleFactorY = scale.y;
			
			
			Mesh mesh = this.GetComponent<MeshFilter> ().mesh;
			Vector2[] uvs = new Vector2[mesh.vertices.Length];
            
			for (int i=0; i< origUVs.Length; ++i) {
				uvs [i] = VuforiaTexCoordMap (ZoneTexCoordMap (origUVs [i]));
			}
			
			mesh.uv = uvs;
		}
	}
}
