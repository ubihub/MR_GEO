using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Examples.InteractiveElements;

namespace HoloToolkit.Unity
{
    public class PlaneFactory : MonoBehaviour, IInputClickHandler
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
            Debug.Log("In plane factory oninputclicked");
            var cameraTransform = CameraCache.Main.transform;
            var headPosition = cameraTransform.position;
            var forward = cameraTransform.forward;
            var scenePosition = headPosition + (depth * forward);
            
            //Vector3 new_pos = new Vector3(pos.x * depth, pos.y * depth, depth);
            GameObject plane = Instantiate(planePrefab, scenePosition, Quaternion.identity);
            //Deactivate();

            //plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
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

