using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class SurfaceDraw : MonoBehaviour, IInputHandler
{
    private bool isDrawing;
    public GameObject drawPrefab;

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.PushModalInputHandler(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrawing == true)
        {
            Drawing();
            drawPrefab.SetActive(true);
        }
        else
            return;
    }


    void Drawing() {

        RaycastHit hitInfo;

        if (Physics.Raycast(
           Camera.main.transform.position,
           Camera.main.transform.forward,
           out hitInfo,
           Mathf.Infinity,
           Physics.DefaultRaycastLayers))
        {
            drawPrefab.transform.position = hitInfo.point;
        }

    }
    

    public void OnInputDown(InputEventData eventData)
    {
        isDrawing = true;
    }

    public void OnInputUp(InputEventData eventData)
    {
        isDrawing = false;
    }
}
