using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class Plane2 : MonoBehaviour, IInputClickHandler
{
    public int width;
    public int height;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("On plane input clicked");
        SetPlane();
    }

    public void Activate()
    {
        Debug.Log("Starting plane annotation...");
        InputManager.Instance.PushModalInputHandler(this.gameObject);
    }

    public void Deactivate()
    {
        Debug.Log("Stopping plane annotation...");
        InputManager.Instance.PopModalInputStack();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlane()
    {
        RaycastHit hit;
        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hit,
                Mathf.Infinity,
                Physics.DefaultRaycastLayers))
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            var mesh = new Mesh();
            mf.mesh = mesh;

            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(width, width / 2, 0);
            vertices[2] = new Vector3(0, height, height / 2);
            vertices[3] = new Vector3(width, height, 0);

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
