using BGC.Annotation.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGC.Annotation.Basic
{
    public class AnnotationUpdater : MonoBehaviour
    {
        AnnotationEntity newAnnotationEntity;

        private AnnotationUpdater()
        {
        }

        public static AnnotationUpdater Instance { get { return Nested.instance; } }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly AnnotationUpdater instance = new AnnotationUpdater();
        }

        void Start()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }

        }

        // Update is called once per frame
        void Update()
        {
            try {

            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
        }

        public void RemakeAnnotations(BgcAnnotation bgcAnnotation)
        {
            try
            {
                Annotation.SetBGCAnnotation(bgcAnnotation);
                Debug.Log("JSONParser.FromJSON.Count= " + bgcAnnotation.annotationEntities.Count);
                foreach (AnnotationEntity annotationEntity in bgcAnnotation.annotationEntities)
                {
                    Annotation.AnnotationTypes tempAnnotationType = (Annotation.AnnotationTypes)Enum.Parse(typeof(Annotation.AnnotationTypes), annotationEntity.type.ToLower());
                    Debug.Log("JSONParser.FromJSON.annotation.tempAnnotationType = " + tempAnnotationType);

                    switch (tempAnnotationType)
                    {
                        case Annotation.AnnotationTypes.spot:
                            Annotation.annotationType = Annotation.AnnotationTypes.spot;
                            newAnnotationEntity = Spot.Instance.InstantiateFromEntity(annotationEntity);
                            if (newAnnotationEntity != annotationEntity)
                            {
                                bgcAnnotation.annotationEntities[bgcAnnotation.annotationEntities.FindIndex(ind => ind.Equals(annotationEntity))] = newAnnotationEntity;
                            }
                            break;
                        case Annotation.AnnotationTypes.polyline:
                            Annotation.annotationType = Annotation.AnnotationTypes.polyline;

                            break;
                        case Annotation.AnnotationTypes.surface:

                            break;
                        case Annotation.AnnotationTypes.free:

                            break;
                        default: break;

                    }
                }
                //Vector3 position = JSONParser.Instance.JSON2Vector3(result);
                //                    Annotation annotation = Annotation.Instance;
                //                    annotation.Instantiate2(position);
            }
            catch (Exception ex)
            {
                ExceptionHandler eh = ExceptionHandler.Instance;
                eh.GetException(ex);
            }
            finally
            {
                bgcAnnotation = BgcAnnotation.Instance;
                Annotation.SetBGCAnnotation(bgcAnnotation);
            }
        }
    }
}
