/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/
using UnityEngine;
using System;
using Vuforia;

public class VideoTextureBehaviour : MonoBehaviour
{
    #region PUBLIC_MEMBER_VARIABLES

	public Camera m_Camera = null;

    #endregion // PUBLIC_MEMBER_VARIABLES



    #region PRIVATE_MEMBER_VARIABLES
	private QCARRenderer.VideoTextureInfo mTextureInfo;
	private ScreenOrientation mScreenOrientation;
	private int mScreenWidth = 0;
	private int mScreenHeight = 0;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region UNITY_MONOBEHAVIOUR_METHODS

	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
		// Setup the geometry and orthographic camera as soon as the video
		// background info is available.
		if (QCARRenderer.Instance.IsVideoBackgroundInfoAvailable ()) {
			// Check if we need to update the texture:
			QCARRenderer.VideoTextureInfo texInfo = QCARRenderer.Instance.GetVideoTextureInfo ();
			if (!mTextureInfo.imageSize.Equals (texInfo.imageSize) ||
				!mTextureInfo.textureSize.Equals (texInfo.textureSize)) {
                
				GetComponent<Renderer>().material.mainTexture = SettingsUpdaterAbstract.Instance.GetCurrentVideoStream ();    
   
				// Cache the info:
				mTextureInfo = texInfo;

				Debug.Log ("VideoTextureInfo " + texInfo.textureSize.x + " " +
					texInfo.textureSize.y + " " + texInfo.imageSize.x + " " + texInfo.imageSize.y);

				// Create the video mesh
				MeshFilter meshFilter = GetComponent<MeshFilter> ();
				if (meshFilter == null) {
					meshFilter = gameObject.AddComponent<MeshFilter> ();
				}
                
                
				meshFilter.mesh = CreateVideoMesh (10, 10);

				// Position the video mesh
				PositionVideoMesh ();
			} else if (mScreenOrientation != Screen.orientation ||
				mScreenWidth != Screen.width ||
				mScreenHeight != Screen.height) {
				// Position the video mesh
				PositionVideoMesh ();
			}
		}
	}

    #endregion // UNITY_MONOBEHAVIOUR_METHODS



    #region PRIVATE_METHODS

	// Create a video mesh with the given number of rows and columns
	// Minimum two rows and two columns
	private Mesh CreateVideoMesh (int numRows, int numCols)
	{
		Mesh mesh = new Mesh ();

		// Build mesh:
		mesh.vertices = new Vector3[numRows * numCols];
		Vector3[] vertices = mesh.vertices;

		for (int r = 0; r < numRows; ++r) {
			for (int c = 0; c < numCols; ++c) {
				float x = (((float)c) / (float)(numCols - 1)) - 0.5F;
				float z = (1.0F - ((float)r) / (float)(numRows - 1)) - 0.5F;

				vertices [r * numCols + c].x = x * 2.0F;
				vertices [r * numCols + c].y = 0.0F;
				vertices [r * numCols + c].z = z * 2.0F;
			}
		}
		mesh.vertices = vertices;

		// Builds triangles:
		mesh.triangles = new int[numRows * numCols * 2 * 3];
		int triangleIndex = 0;

		// Setup UVs to match texture info:
		float scaleFactorX = (float)mTextureInfo.imageSize.x / (float)mTextureInfo.textureSize.x;
		float scaleFactorY = (float)mTextureInfo.imageSize.y / (float)mTextureInfo.textureSize.y;

		mesh.uv = new Vector2[numRows * numCols];

		int[] triangles = mesh.triangles;
		Vector2[] uvs = mesh.uv;

		for (int r = 0; r < numRows-1; ++r) {
			for (int c = 0; c < numCols-1; ++c) {
				// p0-p3
				// | \ |
				// p2-p1

				int p0Index = r * numCols + c;
				int p1Index = r * numCols + c + numCols + 1;
				int p2Index = r * numCols + c + numCols;
				int p3Index = r * numCols + c + 1;

				triangles [triangleIndex++] = p0Index;
				triangles [triangleIndex++] = p1Index;
				triangles [triangleIndex++] = p2Index;

				triangles [triangleIndex++] = p1Index;
				triangles [triangleIndex++] = p0Index;
				triangles [triangleIndex++] = p3Index;

				uvs [p0Index] = new Vector2 (((float)c) / ((float)(numCols - 1)) * scaleFactorX,
                                                ((float)r) / ((float)(numRows - 1)) * scaleFactorY);

				uvs [p1Index] = new Vector2 (((float)(c + 1)) / ((float)(numCols - 1)) * scaleFactorX,
                                ((float)(r + 1)) / ((float)(numRows - 1)) * scaleFactorY);

				uvs [p2Index] = new Vector2 (((float)c) / ((float)(numCols - 1)) * scaleFactorX,
                            ((float)(r + 1)) / ((float)(numRows - 1)) * scaleFactorY);

				uvs [p3Index] = new Vector2 (((float)(c + 1)) / ((float)(numCols - 1)) * scaleFactorX,
                            ((float)r) / ((float)(numRows - 1)) * scaleFactorY);
			}
		}

		mesh.triangles = triangles;
		mesh.uv = uvs;

		mesh.normals = new Vector3[mesh.vertices.Length];
		mesh.RecalculateNormals ();

		return mesh;
	}

	// Scale and position the video mesh to fill the screen
	private void PositionVideoMesh ()
	{
		// Cache the screen orientation and size

		mScreenOrientation = Screen.orientation;

		mScreenWidth = Screen.width;
		mScreenHeight = Screen.height;

		#if !UNITY_EDITOR
        // Reset the rotation so the mesh faces the camera
        gameObject.transform.localRotation = Quaternion.AngleAxis(270.0f, Vector3.right);

        // Adjust the rotation for the current orientation
        if (mScreenOrientation == ScreenOrientation.Landscape)
        {
            gameObject.transform.localRotation *= Quaternion.identity;
        } else if (mScreenOrientation == ScreenOrientation.Portrait)
        {
            gameObject.transform.localRotation *= Quaternion.AngleAxis(90.0f, Vector3.up);
        } else if (mScreenOrientation == ScreenOrientation.LandscapeRight)
        {
            gameObject.transform.localRotation *= Quaternion.AngleAxis(180.0f, Vector3.up);
        } else if (mScreenOrientation == ScreenOrientation.PortraitUpsideDown)
        {
            gameObject.transform.localRotation *= Quaternion.AngleAxis(270.0f, Vector3.up);
        }
#endif
        
		// Scale game object for full screen video image:
		gameObject.transform.localScale = new Vector3 (1, 1, 1 * (float)mTextureInfo.imageSize.y / (float)mTextureInfo.imageSize.x);
        
		if (m_Camera.orthographic) {
			// Set the scale of the orthographic camera to match the screen size:

			// Visible portion of the image:
			float visibleHeight;
			if (ShouldFitWidth ()) {
				visibleHeight = (float)mScreenHeight / (float)mScreenWidth;
			} else {
				visibleHeight = 1.0f;
			}
            
			m_Camera.orthographicSize = visibleHeight;
            
		} else {
			// Position the mesh at the far end of the perspective frustum

			// Choose a point almost at the far clip plane
			float dist = m_Camera.farClipPlane * 0.99f;

			// Define a plane at the chosen distance
			Plane farPlane = new Plane (m_Camera.transform.forward, m_Camera.transform.position + m_Camera.transform.forward * dist);

			// Cast a ray along the lower left and upper right edges of the frustum
			Ray ray1 = m_Camera.ScreenPointToRay (new Vector3 (0, 0, 0));
			Ray ray2 = m_Camera.ScreenPointToRay (new Vector3 (m_Camera.pixelWidth, m_Camera.pixelHeight, 0));

			// Find the points at which the rays intersect the plane
			float rayDist = 0.0f;
			farPlane.Raycast (ray1, out rayDist);
			Vector3 p1 = m_Camera.transform.InverseTransformPoint (ray1.GetPoint (rayDist));
			farPlane.Raycast (ray2, out rayDist);
			Vector3 p2 = m_Camera.transform.InverseTransformPoint (ray2.GetPoint (rayDist));

			// Position the video mesh at the chosen distance
			gameObject.transform.localPosition = new Vector3 (0, 0, dist);

			// Scale the video mesh to stretch between the intersection points
			if (ShouldFitWidth ()) {
				gameObject.transform.localScale *= (p2.x - p1.x) / 2.0f;
			} else {
				gameObject.transform.localScale *= (p2.y - p1.y) / 2.0f;
			}
		}
	}

	// Returns true if the video mesh should be scaled to match the width of the screen
	// Returns false if the video mesh should be scaled to match the height of the screen
	private bool ShouldFitWidth ()
	{
		if (mScreenWidth > mScreenHeight) {
			float height = mTextureInfo.imageSize.y * (mScreenWidth / (float)
                            mTextureInfo.imageSize.x);
			return (height >= mScreenHeight);
		} else {
			float width = mTextureInfo.imageSize.y * (mScreenHeight / (float)
                            mTextureInfo.imageSize.x);
			return (width < mScreenWidth);
		}
	}

    #endregion // PRIVATE_METHODS
}
