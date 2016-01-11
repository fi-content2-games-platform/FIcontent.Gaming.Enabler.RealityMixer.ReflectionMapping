using UnityEngine;
using System.Collections;

public class SwitchMarkers : MonoBehaviour
{
    public GameObject[] markers;
    int currentActive = 0;
    
    void Start()
    {
        markers[0].SetActive(true);
        for ( int i = 1; i< markers.Length; i++)
        {
            markers[i].SetActive(false);
        }
    }
	
    void OnClick()
    {
        markers[currentActive].SetActive(false);
        currentActive = (currentActive + 1) % markers.Length;
        
        markers[currentActive].SetActive(true);
    }
}
