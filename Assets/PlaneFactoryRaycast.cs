using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Examples.InteractiveElements;

namespace HoloToolkit.Unity
{
    public class PlaneFactoryRaycast : MonoBehaviour, IInputClickHandler
    {
        public float depth = 5.0f;
        public GameObject planePrefab;

        public void Activate()
        {
            Debug.Log("Starting plane annotation...");
            InputManager.Instance.PopModalInputStack();
            InputManager.Instance.PushModalInputHandler(this.gameObject);
        }

        public void Deactivate()
        {
            Debug.Log("Stopping plane annotation...");
            InputManager.Instance.PopModalInputHandler();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            Debug.Log("In plane factory raycast oninputclicked");
            RaycastHit hit;
            if (Physics.Raycast(
                    Camera.main.transform.position,
                    Camera.main.transform.forward,
                    out hit,
                    Mathf.Infinity,
                    Physics.DefaultRaycastLayers))
            {
                //Vector3 new_pos = new Vector3(pos.x * depth, pos.y * depth, depth);
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.tag == "Terra")
                {
                    GameObject plane = Instantiate(planePrefab, hit.point, planePrefab.transform.rotation);
                    GameObject tracker = GameObject.FindWithTag("tracker");
                    tracker.GetComponent<TrackPlanes>().registerReference(plane);
                }
                Deactivate();

                //plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

