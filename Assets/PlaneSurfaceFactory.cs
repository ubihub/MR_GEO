using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSurfaceFactory : MonoBehaviour, IInputHandler
{
    private Vector3[] vertices;
    int curVertex;

    private bool isMoving = false;
    private IInputSource currentInputSource;
    private uint currentInputSourceId;

    // Start is called before the first frame update
    void Start()
    {
        vertices = new Vector3[4];
        curVertex = 0;
    }

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

    // Update is called once per frame
    private void Update()
    {
        if(isMoving)
        {
            Vector3 inputPosition = Vector3.zero;
            currentInputSource.TryGetGripPosition(currentInputSourceId, out inputPosition);

            Debug.Log("Input position: " + inputPosition);
        }

    }

    public void OnInputDown(InputEventData eventData)
    {
        InteractionSourceInfo sourceKind;
        eventData.InputSource.TryGetSourceKind(eventData.SourceId, out sourceKind);
        Debug.Log("Source Kind: " + sourceKind);

        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.

        Vector3 inputPosition = Vector3.zero;

        currentInputSource = eventData.InputSource;
        currentInputSourceId = eventData.SourceId;

        currentInputSource.TryGetSourceKind(currentInputSourceId, out sourceKind);
        switch (sourceKind)
        {
            case InteractionSourceInfo.Hand:
                currentInputSource.TryGetGripPosition(currentInputSourceId, out inputPosition);
                break;
            case InteractionSourceInfo.Controller:
                currentInputSource.TryGetPointerPosition(currentInputSourceId, out inputPosition);
                break;
        }

        Debug.Log("Input position: " + inputPosition);
        isMoving = true;
    }

    public void OnInputUp(InputEventData eventData)
    {

    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        RaycastHit hit;

        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hit,
                Mathf.Infinity,
                Physics.DefaultRaycastLayers))
        {
            if(curVertex < 3) // user still specifying vertices
            {
                vertices[curVertex] = hit.point;
                curVertex++;
            }
            else
            {
                vertices[curVertex] = hit.point;

                MeshFilter mf = GetComponent<MeshFilter>();
                var mesh = new Mesh();
                mf.mesh = mesh;

                // create the plane mesh
                mesh.vertices = vertices;

                int[] tri = new int[6]; // two triangles to define plane
                tri[0] = 0;
                tri[1] = 2;
                tri[2] = 1;

                tri[3] = 2;
                tri[4] = 3;
                tri[5] = 1;

                mesh.triangles = tri;

                // normals to correctly shade plane with a light
                Vector3[] normals = new Vector3[4]; // point in negative Z
                normals[0] = -Vector3.forward;
                normals[1] = -Vector3.forward;
                normals[2] = -Vector3.forward;
                normals[3] = -Vector3.forward;

                mesh.normals = normals;

                // texture coordinates to let mesh display material correctly
                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(0, 0);
                uv[1] = new Vector2(1, 0);
                uv[2] = new Vector2(0, 1);
                uv[3] = new Vector2(1, 1);

                mesh.uv = uv;
            }

        }


    }
}
