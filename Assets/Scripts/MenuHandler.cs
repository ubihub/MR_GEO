using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ISelectHandler = UnityEngine.EventSystems.ISelectHandler;
using BGC.Annotation.Basic;
using System;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;

namespace BGC.Annotation.Basic
{


    public class MenuHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IInputClickHandler
    {
        public static Annotation.AnnotationTypes currentType;


        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("pointer enter");
            //Debug.Log("MenuHandler,Annotation.annotationType = " + Annotation.annotationType);
            //Debug.Log("MenuHandler,Annotation.mode = " + Annotation.mode);
            currentType = Annotation.annotationType;
            if (Annotation.annotationType != Annotation.AnnotationTypes.undefined)
            {
                Annotation.SetMode("selection", 0);
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("pointer exit");
            //Debug.Log("MenuHandler.Annotation.annotationType = " + Annotation.annotationType);
            //Debug.Log("MenuHandler.Annotation.mode = " + Annotation.mode);
            if (Annotation.annotationType == Annotation.AnnotationTypes.polyline || Annotation.annotationType == Annotation.AnnotationTypes.polygon || Annotation.annotationType == Annotation.AnnotationTypes.spot)
            {
                Annotation.SetMode("creation", 2);
            }
            else if (Annotation.annotationType == Annotation.AnnotationTypes.undefined)
            {
                Annotation.SetMode("creation", 1);
            }
        }

        void Start()
        {
            SpatialMappingManager spatialMappingManager;
            spatialMappingManager = SpatialMappingManager.Instance;
            if (spatialMappingManager != null) spatialMappingManager.DrawVisualMeshes = false;
            //Transmitter transmitter = Transmitter.Instance;
            //SpatialMappingManager.Instance.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            try
            {

               
                //Annotation.AnnotationTypes tempAnnotationType = (Annotation.AnnotationTypes)Enum.Parse(typeof(Annotation.AnnotationTypes), this.gameObject.name.ToLower());
                char[] delimeter = { '(', ' ' };
                string[] intervalString = this.gameObject.name.ToLower().Split(delimeter);
                Annotation.AnnotationTypes tempAnnotationType = (Annotation.AnnotationTypes)Enum.Parse(typeof(Annotation.AnnotationTypes), intervalString[0]);
                Debug.Log("MenuHandler.OnSelect. : " + tempAnnotationType);
                //Annotation.count++;
                switch (tempAnnotationType)
                {
                    case Annotation.AnnotationTypes.spot:
                        Annotation.annotationType = Annotation.AnnotationTypes.spot;
                        Annotation.count = 2;
                        Annotation.SetMode("creation", 2);
                        //                        Debug.Log("Annotation.annotationType = " + Annotation.annotationType);
                        break;
                    case Annotation.AnnotationTypes.polyline:
                        Annotation.annotationType = Annotation.AnnotationTypes.polyline;
                        Debug.Log("MenuHandler.Annotation.AnnotationTypes.polyline =====" + Annotation.count);
                        Polyline.Instance.Init();
                        Transmitter.Instance.Pause();
                        Annotation.count = 2;
                        Annotation.SetMode("creation", 2);

                        break;
                    case Annotation.AnnotationTypes.polygon:
                        Annotation.annotationType = Annotation.AnnotationTypes.polygon;
                        Debug.Log("polygon.Annotation.count =====" + Annotation.count);
                        Polygon.Instance.Init();
                        Transmitter.Instance.Pause();
                        Annotation.count = 2;
                        Annotation.SetMode("creation", 2);

                        break;
                    case Annotation.AnnotationTypes.surface:
                        Annotation.SetMode("creation", 2);

                        break;
                    case Annotation.AnnotationTypes.done:
                        Debug.Log("Annotation.AnnotationTypes.done:");
                        Annotation.SetMode("done", 1);
                        if (Annotation.annotationType == Annotation.AnnotationTypes.polyline ) Polyline.Done();
                        else if (Annotation.annotationType == Annotation.AnnotationTypes.polygon) Polygon.Done();
                        else if (Annotation.annotationType == Annotation.AnnotationTypes.spot) Spot.Done();
                        Debug.Log("Finalize from Annotation.AnnotationTypes.done");
                        Annotation.Finalize();
                        //Annotation.Done();
                        Annotation.SetMode("selection", 1);
                        Transmitter.Instance.Resume();
                        Annotation.annotationType = Annotation.AnnotationTypes.undefined;
                        Annotation.undoStack.Clear();
                        Annotation.redoStack.Clear();
                        break;
                    case Annotation.AnnotationTypes.cancel:
                        Annotation.SetMode("cancel", 1);
                        Polyline.Cancel();
                        Annotation.SetMode("selection", 1);
                        Transmitter.Instance.Resume();
                        Annotation.undoStack.Clear();
                        Annotation.redoStack.Clear();
                        break;
                    case Annotation.AnnotationTypes.undo:
                        if (Annotation.annotationType == Annotation.AnnotationTypes.polyline  && Polyline.Undo())
                        {
                            Debug.Log("Annotation.AnnotationTypes.polyline.undo");
                            Annotation.SetMode("undo", 2);
                            Transmitter.Instance.Resume();
                            Transmitter.Instance.Recall("undo");
                            Annotation.redoStack.Clear();
                        } else if (Annotation.annotationType == Annotation.AnnotationTypes.polygon && Polygon.Undo())
                        {
                            Debug.Log("Annotation.AnnotationTypes.polygon.undo");
                            Annotation.SetMode("undo", 2);
                            Transmitter.Instance.Resume();
                            Transmitter.Instance.Recall("undo"); ;
                            Annotation.redoStack.Clear();
                        }
                        else if(Annotation.annotationType == Annotation.AnnotationTypes.spot)
                        {
                            Debug.Log("Annotation.AnnotationTypes.spot");
                            Annotation.SetMode("undo", 2);
                            Transmitter.Instance.Resume();
                            Transmitter.Instance.Recall("undo"); ;
                        }
                        break;
                    case Annotation.AnnotationTypes.redo:
                        if (Annotation.annotationType == Annotation.AnnotationTypes.polyline && Polyline.Redo())
                        {
                            Annotation.SetMode("redo", 1);
                            Transmitter.Instance.Resume();
                            Transmitter.Instance.Recall("redo");
                            Annotation.undoStack.Clear();
                        }
                        else if (Annotation.annotationType == Annotation.AnnotationTypes.polygon && Polygon.Redo())
                        {
                            Annotation.SetMode("redo", 1);
                            Transmitter.Instance.Resume();
                            Transmitter.Instance.Recall("redo");
                            Annotation.undoStack.Clear();
                        }
                        else if (Annotation.annotationType == Annotation.AnnotationTypes.spot)
                        {
                            Annotation.SetMode("redo", 1);
                            Transmitter.Instance.Resume();
                            Transmitter.Instance.Recall("redo");
                        }
                        break;
                    case Annotation.AnnotationTypes.last:
                        Debug.Log("last");
                        Transmitter.Instance.Recall("last"); ;
                        break;

                    case Annotation.AnnotationTypes.delete:
                        Debug.Log("Annotation.AnnotationTypes.delete");
                        Annotation.Delete();
                        break;

                    default:
                        Annotation.annotationType = Annotation.AnnotationTypes.undefined;
                        Annotation.SetMode("selection", 0);
                        break;

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
        }

    }
}