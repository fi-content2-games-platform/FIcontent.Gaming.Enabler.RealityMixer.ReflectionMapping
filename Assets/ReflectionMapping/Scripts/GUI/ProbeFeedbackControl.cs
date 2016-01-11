using UnityEngine;
using System.Collections;

public class ProbeFeedbackControl : MonoBehaviour
{
	public Camera worldCamera;
	//public UIAtlas atlas;
	Camera guiCamera;
    
	BoxCollider edgeCollider;

	struct LightProbeMarkerArrows
	{
		public Transform lightProbe;
		//        public UISprite arrow;
		//        public LightProbeMarkerArrows(Transform lp, UISprite a)
		//        {
		//            lightProbe = lp;
		//            arrow = a;
		//        }
	}

	LightProbeMarkerArrows[] lightProbes;

	void Start ()
	{
//        guiCamera = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("NGUI"));
//        
//        GameObject[] lpGOs = GameObject.FindGameObjectsWithTag("RealLightProbeUpdater");
//        
//        lightProbes = new LightProbeMarkerArrows[lpGOs.Length];
//        
//        for ( int i = 0; i < lpGOs.Length; ++i)
//        {
//            lightProbes[i] = new LightProbeMarkerArrows(lpGOs[i].transform, NGUITools.AddSprite(gameObject, atlas, "Arrow"));
//            NGUITools.MakePixelPerfect(lightProbes[i].arrow.cachedTransform);
//        }
//        
//        GameObject colliderGO = new GameObject("edgeCollider");
//        colliderGO.transform.parent = transform;
//        colliderGO.transform.localPosition = Vector3.zero;
//        colliderGO.transform.localRotation = Quaternion.identity;
//        colliderGO.transform.localScale = new Vector3((1536.0f / Screen.height * Screen.width) - 60, 1536 - 60, 20);
//        
//	    edgeCollider = colliderGO.AddComponent<BoxCollider>();
	}

	void Update ()
	{
//        if (GUIHandler.instance.isTracking)
//        {
//    	    foreach (LightProbeMarkerArrows lp in lightProbes)
//            {
//                Vector3 rayPosition = worldCamera.WorldToScreenPoint(lp.lightProbe.localPosition); // using AR camera get screen location of probe.
//                rayPosition.z = 0; // zero the z to get the point into the camera plane;
//                rayPosition = guiCamera.ScreenToWorldPoint(rayPosition); // get the world position relative to the GUI camera.
//                
//                Ray ray = new Ray(rayPosition, -rayPosition);
//                Debug.DrawRay(rayPosition, -rayPosition, Color.white);
//                RaycastHit hit = new RaycastHit();
//                if (edgeCollider.Raycast(ray,out hit,rayPosition.magnitude))
//                {
//                    lp.arrow.enabled = true;
//                    lp.arrow.cachedTransform.position = hit.point;
//                    
//                    Vector3 nDirection = lp.arrow.cachedTransform.localPosition.normalized;
//                    
//                    lp.arrow.cachedTransform.localRotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, nDirection), Vector3.Cross(Vector3.up, nDirection));
//                }
//                else
//                {
//                    lp.arrow.enabled = false;
//                }
//            }
//        }
//        else
//        {
//            foreach (LightProbeMarkerArrows lp in lightProbes)
//            {
//                lp.arrow.enabled = false;
//            }
//        }
	}
}
