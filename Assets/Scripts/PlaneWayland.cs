using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit;
using HoloToolkit.Unity.InputModule;

public class PlaneWayland : MonoBehaviour, IInputHandler
{
   public GameObject prefabPlane;
    // Start is called before the first frame update
    void OnEnable()
    {
        InputManager.Instance.PushModalInputHandler(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.PushModalInputHandler(this.gameObject);
    }

    public void OnInputDown(InputEventData eventData)
    {
        Spot();
    }

    public void OnInputUp(InputEventData eventData)
    {
        return;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spot()
    {

        GameObject SpotClone;
        RaycastHit hit;
        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hit,
                Mathf.Infinity,
                Physics.DefaultRaycastLayers))
        {
            if (hit.collider.tag == "Terra")
            {
                        SpotClone = (GameObject)Instantiate(prefabPlane, hit.point, Quaternion.LookRotation(hit.normal));      
            }

        }

    }
}
