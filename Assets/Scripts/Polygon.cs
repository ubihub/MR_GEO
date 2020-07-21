using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace BGC.Annotation.Basic
{

    public class Polygon : Annotation
    {
        static public GameObject pointPrefab;
        static public GameObject linePrefab;
        static public GameObject polylineObject;
        static public GameObject instance;
        static private int numPoints = 0;
        static private Vector3[] linePositions = new Vector3[32];
        static private LineRenderer newLineRend;
        static private GameObject newLine;


        private Polygon()
        {
        }

        public static Polygon Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Polygon instance = new Polygon();
        }

        public GameObject InstantiateFromEventData(InputClickedEventData eventData)
        {
            RaycastHit hitInfo;
            pointPrefab = Resources.Load("Point") as GameObject;
            linePrefab = Resources.Load("Line") as GameObject;
            if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hitInfo,
                Mathf.Infinity,
                Physics.DefaultRaycastLayers))
            {
                // user placed a new point
                GameObject newPoint = Instantiate(pointPrefab, hitInfo.point, Quaternion.identity);
                numPoints++;
                linePositions[numPoints - 1] = hitInfo.point;

                if (numPoints > 2) // re-render line with new point as vertex
                {
                    // GameObject[] allPoints = GameObject.FindGameObjectsWithTag("PointMarker");
                    newLineRend.positionCount = numPoints;
                    newLineRend.SetPosition(numPoints - 1, hitInfo.point);
                }
                else if (numPoints == 2) // render new line
                {
                    newLine = Instantiate(linePrefab);
                    newLineRend = newLine.GetComponent<LineRenderer>();
                    //newLine.AddComponent<LineRenderer>();
                    //LineRenderer newLineRend = newLine.GetComponent<LineRenderer>();
                    newLineRend.loop = false;
                    newLineRend.positionCount = numPoints;
                    newLineRend.SetPositions(linePositions); // Take
                }
                if (newLine != null) newLine.transform.parent = instance.transform;
                if (newPoint != null)
                {
                    newPoint.transform.parent = instance.transform;
                    undoStack.Push(newPoint);
                }
                instance.tag = "annotation";
            }
            return linePrefab;

        }

        public void Init()
        {
            numPoints = 0;
            polylineObject = Resources.Load("polylineObject") as GameObject;
            instance = Instantiate(polylineObject, new Vector3(), Quaternion.identity);

        }

        public static void Done()
        {
            Debug.Log("Polygon.instance.transform.childCount= " + instance.transform.childCount);
            if (instance != null & instance.transform.childCount > 0)
            {
                //Vector3 firstPosition = new Vector3(linePositions[0]);
                Vector3 firstPosition = linePositions[0];
                newLineRend.positionCount = numPoints + 1;
                newLineRend.SetPosition(numPoints , firstPosition);

                instance.name = "polygon";
                instance.tag = "annotation";
                int count = instance.transform.childCount;
                Debug.Log("Polygon.instance.transform.childCount = " + count);
                for (int i = 0; i < count; i++)
                {
                    Transform child = instance.transform.GetChild(i);
                    Debug.Log("Polygon.Done.child.name = " + child.name);
                    child.tag = "annotation";
                }
            }

        }

        public AnnotationEntity InstantiateFromEntity(AnnotationEntity annotationEntity)
        {
            Init();
            pointPrefab = Resources.Load("Point") as GameObject;
            linePrefab = Resources.Load("Line") as GameObject;
            int children = annotationEntity.children.Count;
            //Debug.Log("children = " + children);
            for (int i = 0; i < children; i++)
            {
                AnnotationEntity ae = annotationEntity.children[i];
                //Debug.Log("AnnotationEntity ae= " + ae);
                Vector3 position;
                position.x = ae.position.x;
                position.y = ae.position.y;
                position.z = ae.position.z;
                //Debug.Log("pointPrefab, position, " + pointPrefab + " " + position);
                GameObject newPoint = Instantiate(pointPrefab, position, Quaternion.identity);
                //Debug.Log("pointPrefab, position, " + pointPrefab + " " + position);
                numPoints++;
                linePositions[numPoints - 1] = position;

                if (numPoints > 2) // re-render line with new point as vertex
                {
                    //Debug.Log("numPoints" + numPoints);
                    newLineRend.positionCount = numPoints;
                    //Debug.Log("newLineRend.SetPosition( position);" + position);
                    newLineRend.SetPosition(numPoints - 1, position);
                }
                else if (numPoints == 2) // render new line
                {
                    //Debug.Log("newLine = Instantiate(linePrefab);" + linePrefab);
                    newLine = Instantiate(linePrefab);
                    newLineRend = newLine.GetComponent<LineRenderer>();
                    //newLine.AddComponent<LineRenderer>();
                    //LineRenderer newLineRend = newLine.GetComponent<LineRenderer>();
                    newLineRend.loop = false;
                    newLineRend.positionCount = numPoints;
                    //Debug.Log("newLineRend.SetPositions(linePositions); " + linePositions);
                    newLineRend.SetPositions(linePositions); // Take
                }
                if (newLine != null)
                {
                    newLine.transform.parent = instance.transform;
                    newLine.tag = "annotation";
                }
                if (newPoint != null)
                {
                    newPoint.transform.parent = instance.transform;
                    newPoint.tag = "annotation";
                }
            }
            Vector3 firstPosition = linePositions[0];
            newLineRend.positionCount = numPoints + 1;
            newLineRend.SetPosition(numPoints, firstPosition);

            instance.tag = "annotation";
            instance.name = "polygon";
            numPoints = 0;
            return annotationEntity;
        }


        public AnnotationEntity MakeAnnotationEntity(GameObject gameObject)
        {
            AnnotationEntity annotationEntity = new AnnotationEntity();
            annotationEntity.tag = "annotation";
            annotationEntity.type = "polygon";
            //annotationEntity.symbol = SymbolHandler.symbolNumber;
            annotationEntity.instanceID = gameObject.GetInstanceID();
            numPoints = 0;

            int children = gameObject.transform.childCount;
            //Debug.Log("children = " + children);
            for (int i = 0; i < children; i++)
            {
                Transform child = gameObject.transform.GetChild(i);
                if (child.name.Contains("Point"))
                {
                    AnnotationEntity point = new AnnotationEntity();
                    point.type = "point";
                    point.tag = "annotation";
                    Position position = new Position();
                    position.x = child.position.x;
                    position.y = child.position.y;
                    position.z = child.position.z;
                    point.position = position;
                    annotationEntity.children.Add(point);
                }

            }
            return annotationEntity;

        }

        public static void Cancel()
        {

            GameObject[] allAnnotations = UnityEngine.GameObject.FindGameObjectsWithTag("new");
            foreach (GameObject annotation in allAnnotations)
            {
                //Debug.Log("Destroy = "  + annotation.name);
                DestroyImmediate(annotation);
            }

        }


        public static bool Undo()
        {
            if (undoStack.Count <= 0) { return true; }
            if (undoStack.Count > 0)
            {
                GameObject point = (GameObject)undoStack.Pop();
                //GameObject clone = point;
                Vector3 position = point.transform.position;
                redoStack.Push(position);
                DestroyImmediate(point);
                newLineRend.positionCount = undoStack.Count;
                numPoints--;
            }
            return false;
        }

        public static bool Redo()
        {
            GameObject newPoint;
            linePrefab = Resources.Load("Line") as GameObject;
            //pointPrefab = Resources.Load("Point") as GameObject;
            if (redoStack.Count <= 0) { return true; }
            if (redoStack.Count > 0)
            {
                //if (redoStack != null) Debug.Log("redo" + redoStack.Count);
                Vector3 position = (Vector3)redoStack.Pop();
                //if (redoStack != null) Debug.Log("point----------+ " + position.ToString());

                // GameObject clone = position;
                //undoStack.Push(clone);
                newPoint = Instantiate(pointPrefab, position, Quaternion.identity);
                numPoints++;
                if (numPoints > 2) // re-render line with new point as vertex
                {
                    // GameObject[] allPoints = GameObject.FindGameObjectsWithTag("PointMarker");
                    newLineRend.positionCount = numPoints;
                    newLineRend.SetPosition(numPoints - 1, position);
                }
                else if (numPoints == 2) // render new line
                {
                    newLine = Instantiate(linePrefab);
                    newLineRend = newLine.GetComponent<LineRenderer>();
                    //newLine.AddComponent<LineRenderer>();
                    //LineRenderer newLineRend = newLine.GetComponent<LineRenderer>();
                    newLineRend.loop = false;
                    newLineRend.positionCount = numPoints;
                    newLineRend.SetPositions(linePositions); // Take
                }
                if (newLine != null) newLine.transform.parent = instance.transform;
                if (newPoint != null)
                {
                    newPoint.transform.parent = instance.transform;
                    undoStack.Push(newPoint);
                }
                instance.tag = "annotation";
            }

            return false;
        }

        // Start is called before the first frame update
        void Start()
        {
            InputManager.Instance.PushModalInputHandler(this.gameObject);

        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
