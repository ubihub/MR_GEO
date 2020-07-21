﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
namespace BGC.Annotation.Basic
{

    public class Point : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IInputClickHandler
    {
        private GameObject MainCamera;
        private float Speed = 0.000001f;
        private bool IsSelected = false;
        public GameObject buttonDelete;
        public GameObject contextMenu;
        public static GameObject instanceButton;
        public static int countDown = -1;
        RaycastHit hitInfo;
        // Start is called before the first frame update
        void Start()
        {
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            // IsSelected = false;
        }

        void SetRotate(GameObject toRotate, GameObject camera)
        {
            transform.rotation = Quaternion.Lerp(toRotate.transform.rotation, camera.transform.rotation, Speed * Time.deltaTime);
        }

        // Update is called once per frame
        void Update()
        {
            SetRotate(this.gameObject, MainCamera);
            if (countDown > 0 ) countDown--;
            else if (countDown == 0)
            {
                Annotation.Done();
                countDown = -1;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("pointer enter in spotObject");
            this.GetComponent<Renderer>().material.color = Color.red;
            //if (IsSelected) this.GetComponent<Renderer>().material.color = Color.blue;
            // this.GetComponent<BoxCollider>().transform.
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("pointer exit fro m spotObject");
            if (IsSelected) this.GetComponent<Renderer>().material.color = Color.blue;
            else this.GetComponent<Renderer>().material.color = Color.white;
            if (countDown == -2) countDown = 500;
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (!Annotation.GetMode().Equals("selection")) return;
            Transmitter.Instance.Pause();
            countDown = -2;
            if (!IsSelected)
            {
                IsSelected = true;
                this.GetComponent<Renderer>().material.color = Color.blue;
                //buttonDelete = UnityEngine.GameObject.FindGameObjectWithTag("ButtonDelete");
                //DestroyImmediate(instanceButton);
                //instanceButton = Instantiate(buttonDelete);

                if (buttonDelete != null)
                {
                    //instanceButton.SetActive(true);
                }
                Annotation.addSelectedAnnotation(this.gameObject);
            }
            else
            {
                IsSelected = false;
                this.GetComponent<Renderer>().material.color = Color.blue;
                Annotation.removeSelectedAnnotation(this.gameObject);
            }
            if (Physics.Raycast(
                     Camera.main.transform.position,
                     Camera.main.transform.forward,
                     out hitInfo,
                     Mathf.Infinity,
                     Physics.DefaultRaycastLayers))
            {
                Debug.Log("Polyline Selected");
                Vector3 v3 = hitInfo.point;
                v3.y = v3.y + 0.15f;
                v3.x = v3.x + 0.15f;
                //instanceButton.transform.position = v3;
            }

        }

        //void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawWireCube(transform.position, new Vector3(0.05f, 0.05f, 0.05f));
        //}

    }
}