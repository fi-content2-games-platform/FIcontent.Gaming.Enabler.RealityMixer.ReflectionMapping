#ifndef _VIDEO_TEXTURE_SHADER_DEFINITIONS_
#define _VIDEO_TEXTURE_SHADER_DEFINITIONS_

	/////////////////////////////////////////////////////////////////////////////////////////////////////////
	// This file defines the two texture samplers (for the diffuse and specular light probes repsectively) //
	// along with the the texture coordinates scaling factor - which are all uniforms set through scripts. //
	// It also contains definitions of indexing a region on the mirror ball via direction. The N, V, R, T  //
	// vectors should be set for the other functions to work directly - i.e. reflectionColor(). T		   //
	//                                                                                                     //
	// Code by Dan Andrei Calian - Disney Research                                                         //
	// Fixed use of global N variable for optimser -globals are bad in glsl too, whodathoughit? - Kenny    //
	/////////////////////////////////////////////////////////////////////////////////////////////////////////
	 
	#define PI 3.141592653
	
	sampler2D _DiffuseTex, _SpecularTex;
	float4x4 _PerspectiveShiftMatrixDiff, _PerspectiveShiftMatrixSpec;
	float4x4 _PerspectiveShiftMatrixDiffIT, _PerspectiveShiftMatrixSpecIT;
	
	// Defining the region inside the _VideoTex of the specular and diffuse textures
	float4 _RegionSpecular, _RegionDiffuse;
	
	// Defining the scaling factor needed for correct texture coordinates computation
	float4 _TexCoordScale;
 
    // Defining the distance at each side to the face detection
	//float _RSFactor, _LSFactor;
 
 
	// Viewing, reflected and transmitted/refracted directions
	float3 V;
	float3 R;
	float3 T;
	
	// Texture coordinates mapping function for Vuforia API video texture
	float2 VuforiaTexCoordMap(float x, float y)
	{
		float xr = x * _TexCoordScale.x;
		float yr = y * _TexCoordScale.y;
		
		return float2(xr, yr);
	}
	
	// Indexing into the mirror ball using the passed-in region
	float3 mirrorBallIndexing(sampler2D tex, float3 D, float4 region)
	{
		float3 Dn = normalize(D);
		float x = Dn.x;
		float y = Dn.y;
     
		float u = (x + 1) / 2.0;
		float v = (y + 1) / 2.0;
		
		// To index into the light probe region directly
		u = u * region.z + region.x;
		v = v * region.w + region.y;
		
		return tex2D (tex, VuforiaTexCoordMap(u, v)).rgb;
	}
	
	// Returns refraction color
	float3 refractedColor()
	{
		return mirrorBallIndexing(_SpecularTex, T, _RegionSpecular); 
	}
	
	// Returns reflection color
	float3 reflectionColor(float3 N)
	{
		return mirrorBallIndexing(_SpecularTex, N, _RegionSpecular); 
	}

    // Returns diffuse color
    float3 diffuseColor(float3 N)
    {
        return mirrorBallIndexing(_DiffuseTex, N, _RegionDiffuse);
    }

#endif