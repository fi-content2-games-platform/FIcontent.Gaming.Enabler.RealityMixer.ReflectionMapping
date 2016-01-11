using UnityEngine;
using System.Collections;


[ExecuteInEditMode]	
public class EditorPreviewVideoShader : MonoBehaviour {
    
    public Texture2D mirrorBallDiffusePreviewTexture;
    public Texture2D mirrorBallPreviewTexture;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		#if (UNITY_EDITOR)

		Shader.SetGlobalTexture("_SpecularTex", mirrorBallPreviewTexture);
		Shader.SetGlobalTexture("_DiffuseTex", mirrorBallDiffusePreviewTexture);
		
		Shader.SetGlobalVector("_RegionSpecular", new Vector4(0, 0, 1, 1)); 
		Shader.SetGlobalVector("_RegionDiffuse", new Vector4(0, 0, 1, 1)); 
		
		Shader.SetGlobalVector("_TexCoordScale", new Vector4(1, 1, 0, 0));
		
		Shader.SetGlobalFloat("_SpecularVsDiffuseRatio", 0.0f);
		
		#endif
	}
}
