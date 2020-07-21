using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.Rendering;

public class SpotTemp : MonoBehaviour, IInputClickHandler
{

    public GameObject spotPrefab;
    LineRenderer lineRender;
    public Transform[] points;
    private Vector3[] vPoints;
    int segments = 20;

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.PushModalInputHandler(this.gameObject);
        lineRender = GetComponent<LineRenderer>();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {

        RaycastHit hitInfo;
        GameObject c = new GameObject();
        segments = points.Length;
        vPoints = new Vector3[points.Length];

        if (Physics.Raycast(
             Camera.main.transform.position,
             Camera.main.transform.forward,
             out hitInfo,
             Mathf.Infinity,
             Physics.DefaultRaycastLayers)){
            
            Debug.Log("we did it");

            c = Instantiate(spotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            c.transform.position = hitInfo.point;

            /*
            for (int i = 0; i < points.Length; i++)
            {
                vPoints[i] = points[i].position;

            }
            for (int i = 0; i < segments; i++){
                float t = i / (float)segments;
                lineRender.SetVertexCount(segments);
                lineRender.SetPositions(vPoints);
            }

*/
        }

            //lineRender.SetPosition(1, new Vector3(c.transform.position.x, c.transform.position.y, c.transform.position.z));

    }
}
