using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhysicalLightProbe))]
public class LightDirectionDetection : MonoBehaviour
{
    
    RenderTexture downsampleRT;
    Texture2D downsampleTexture;
    Texture2D debugTexture;
    int lightSearchSize = 32;
    public Material croppingMaterial;
    public GameObject previewObject;
    public GameObject debugPreviewObject;
    public Camera ARCamera;
    
    void Start()
    {
        debugTexture = new Texture2D(lightSearchSize,lightSearchSize);
        downsampleRT = RenderTexture.GetTemporary(lightSearchSize, lightSearchSize);
        downsampleTexture = new Texture2D(lightSearchSize, lightSearchSize);
        
        Color[] debugColors = new Color[lightSearchSize*lightSearchSize];
        
        for (int i=0; i< debugColors.Length; ++i)
        {
            debugColors[i] = new Color(0.2f,0.5f,0.8f);
        }
        debugTexture.SetPixels(debugColors);
        debugTexture.Apply();
    }
    
//    void Update()
//    {
//        ComputeMainLightSourceDirection();
//    }
    
    bool WithinCircle(float x, float y, float diameter)
    {
        float radius = diameter * 0.5f;
        return Vector2.Distance(new Vector2(radius, radius), new Vector2(x, y)) <= radius;
    }
    
    private float Luminance(Color col)
    {
        return ((0.2126f * col.r) + (0.7152f * col.g) + (0.0722f * col.b));// / 255.0f;
    }
 
    float Gaussian(float x)
    {
        return Mathf.Exp((-0.5f * x * x) / 0.1f);
    }
 
    Vector3 SphericalToCartesian(float theta, float phi)
    {
        return new Vector3(Mathf.Sin(phi) * Mathf.Cos(theta), Mathf.Sin(phi) * Mathf.Sin(theta), Mathf.Cos(phi));
    }
    
    Vector3 ThetaPhiSolidAngle(float x, float y, int size)
    {
        Vector3 result = new Vector3();
        
        // Theta, phi:
        // To normalised coords [-1, 1]
        float s = (float)(size - 1);
        float u = (x / s) * 2 - 1.0f;
        float v = (y / s) * 2 - 1.0f;
        
        // To spherical coords [theta, phi]
        float thetaForSolidAngle = Mathf.Atan2(v, u);
        float theta = thetaForSolidAngle + Mathf.PI;
        float phi = Mathf.PI * Mathf.Sqrt(u * u + v * v);
        
        result.x = theta;
        result.y = phi;
        
        // Solid angle
        float p = (2.0f * Mathf.PI) / (float)size;
        p *= p;
        result.z = p * (Mathf.Sin(thetaForSolidAngle) / thetaForSolidAngle);
        
        return result;
    }

    Vector3 PixelPositionToDirection(float x, float y, int size)
    {
        Vector3 r = ThetaPhiSolidAngle(x, y, size);
        float theta = r.x;
        float phi = r.y;
     
        Vector3 res = SphericalToCartesian(theta, phi);
        //text.text = "Main light direction:\nTheta: " + Mathf.Rad2Deg * theta + "; Phi: " + Mathf.Rad2Deg * phi + "\nVector: " + res;
     
        return res;
    }
    
    public Vector3 ComputeMainLightSourceDirection()
    {
        // Take specular region texture and copy paste into another downsampled buffer.
        
        PhysicalLightProbe lightProbe = GetComponent<PhysicalLightProbe>();
        croppingMaterial.SetVector("_Region", lightProbe.GetRegion());
        
        Graphics.Blit(lightProbe.GetVideoTextureReference(), downsampleRT, croppingMaterial);
    
        RenderTexture.active = downsampleRT;
        downsampleTexture.ReadPixels(new Rect(0, 0, lightSearchSize, lightSearchSize), 0, 0, false); // this probably also copies texture to CPU
        downsampleTexture.anisoLevel = 0;
        downsampleTexture.filterMode = FilterMode.Point;
        downsampleTexture.Apply(); 
    
        // Iterate through it and find the maximum value
        float maxL = -1.0f;// avgL = 0.0f;
        int maxX = lightSearchSize / 2, maxY = lightSearchSize / 2;
    
    
        int pixelIndex = 0;
        Color[] pixels = downsampleTexture.GetPixels();
    
        // Find max and average
        for (int y = 0; y < lightSearchSize; ++y)
        {
            for (int x = 0; x < lightSearchSize; ++x)
            {
                if (WithinCircle(x, y, lightSearchSize))
                {
                    float l = Luminance(pixels[pixelIndex]);// * SolidAngle(x, y, lightSearchSize);
             
                    if (l > maxL)
                    {
                        maxL = l;
                        maxX = x;
                        maxY = y;
                    }
                }
                ++pixelIndex;
            }
        }
    
        // Find all places of >90% of maxL - extremely crude approximation  
        float areax = 0, areay = 0;
        float avgW = 0;
        float avgLhere = 0;
        pixelIndex = 0;
    
        //int[] sections
    
        for (int y = 0; y < lightSearchSize; ++y)
        {
            for (int x = 0; x < lightSearchSize; ++x)
            {
                if (WithinCircle(x, y, lightSearchSize))
                {
                    float l = Luminance (pixels[pixelIndex]);// * SolidAngle(x, y, lightSearchSize);
             
                    //if (l > maxL * 0.90f && Vector2.Distance(new Vector2(maxX, maxY), new Vector2(x,y)) < lightSearchSize * 0.15f)
                    {
                        float weightL = Gaussian(maxL - l);
                        float weightD = Gaussian(Vector2.Distance(new Vector2(maxX, maxY), new Vector2(x, y)) / lightSearchSize);
                 
                        float w = weightL * 1.0f + weightD * 0.0f;
                        debugTexture.SetPixel(x,y, new Color(w,w,w));
                 
                        //float weight =  use a weight based on distance from max, and also on similarity in luminance to max; then areax and areay are weighted averages! 
                        avgLhere += (l * w);
                        areax += (x * w);
                        areay += (y * w);
                        avgW += w;
                 
                 
                        downsampleTexture.SetPixel(x, y, Color.green);
                    }
                }
                else
                {
                    downsampleTexture.SetPixel(x, y, Color.black);
                }
         
                ++pixelIndex;
            }
     
        }
    
        avgLhere /= avgW;
        areax /= avgW;
        areay /= avgW;
    
    
        downsampleTexture.SetPixel((int)areax, (int)areay, Color.blue);
        downsampleTexture.SetPixel(maxX, maxY, Color.red);
    
        downsampleTexture.Apply();
        debugTexture.Apply();
     
        if (debugPreviewObject)
        {
            debugPreviewObject.GetComponent<Renderer>().material.mainTexture = debugTexture;
        }
        if (previewObject)
        {
            previewObject.GetComponent<Renderer>().material.mainTexture = downsampleTexture;
        }
        
        // Map position of max value back to a 3d direction - or spherical coordinates and from that to a vector
        // Return that vector (to be used in an additional shader pass for making shadows)
        //return PixelPositionToDirection(maxX, maxY, lightSearchSize);
        return ARCamera.transform.InverseTransformDirection(PixelPositionToDirection(areax, areay, lightSearchSize));
    }
}
