using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class PolylineLocal : MonoBehaviour, IInputClickHandler
{
    public GameObject pointPrefab;
    public GameObject linePrefab;
    private int numPoints = 0;
    private Vector3[] linePositions;
    private LineRenderer newLineRend;

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Click detected");
        RaycastHit hitInfo;

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
                GameObject newLine = Instantiate(linePrefab);
                newLineRend = newLine.GetComponent<LineRenderer>();
                //newLine.AddComponent<LineRenderer>();
                //LineRenderer newLineRend = newLine.GetComponent<LineRenderer>();
                newLineRend.loop = false;
                newLineRend.positionCount = numPoints;
                newLineRend.SetPositions(linePositions); // Take
            }

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.PushModalInputHandler(this.gameObject);
        linePositions = new Vector3[32];
    }

    // Update is called once per frame
    void Update()
    {    
    }

}
