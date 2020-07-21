using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;


public class ButtonIgnoreRaycast : MonoBehaviour, IInputClickHandler
{
    public GameObject DrawTool;

    void Start() {

     //  GameObject DrawTool = GameObject.Find("SurfaceDrawDone");
    }
    
    public void OnInputClicked(InputClickedEventData eventData)
    {
        RaycastHit hit;
        if (Physics.Raycast(
              Camera.main.transform.position,
              Camera.main.transform.forward,
              out hit,
              Mathf.Infinity,
              Physics.DefaultRaycastLayers))
        {
            if (hit.collider.tag == "drawButtonIgnore")
            {
                DrawTool.SetActive(false);
            }   
        }
    }
}