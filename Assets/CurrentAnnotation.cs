using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGC.Annotation.Basic
{

    public class CurrentAnnotation : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
                switch (Annotation.annotationType)
                {
                    case Annotation.AnnotationTypes.spot:
                        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                        Sprite sp = sr.GetComponent<Sprite>();
                        sp = Resources.Load("spot") as Sprite;
                        break;
                    case Annotation.AnnotationTypes.polyline:
                        Annotation.annotationType = Annotation.AnnotationTypes.polyline;

                        break;
                    case Annotation.AnnotationTypes.surface:

                        break;
                    case Annotation.AnnotationTypes.done:
                        Annotation.annotationType = Annotation.AnnotationTypes.undefined;
                        Annotation.SetMode("selection", 1);
                        //gameObject.SendMessage("SetMode", "selection");
                        Annotation.Done();
                        //this.gameObject.GetComponent<Renderer>().
                        break;
                    case Annotation.AnnotationTypes.undo:
                        Debug.Log("Undo");
                        if (Annotation.Undo()) Transmitter.Instance.Recall("undo"); ;
                        break;
                    case Annotation.AnnotationTypes.redo:
                        Debug.Log("Redo");
                        if (Annotation.Redo()) Transmitter.Instance.Recall("redo"); ;
                        break;
                    case Annotation.AnnotationTypes.last:
                        Debug.Log("last");
                        Transmitter.Instance.Recall("last"); ;
                        break;

                    default:
                        Annotation.annotationType = Annotation.AnnotationTypes.undefined;
                        break;

                }
        }
    }
}