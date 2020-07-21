using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class SurfaceDraw1 : MonoBehaviour, IInputHandler, IInputClickHandler
{
    public GameObject drawPrefab;
    private Transform drawTransform;
    private Vector3 cursorOriginalPos;
    public Color col;
    public GameObject cursor;

    void Start()
    {
        InputManager.Instance.PushModalInputHandler(this.gameObject);
        cam = Camera.main;
        drawTransform = this.gameObject.transform;
        cursorOriginalPos = this.transform.position + transform.forward;
    }

    private Camera cam;
    bool isDraw = false;

    void Update()
    {
           
        if (isDraw == true)
        {
            Draw2D();
        }

        if (isDraw == false)
        {
            Debug.Log(isDraw);
            drawPrefab.transform.position = Camera.main.transform.position;
            cursor.transform.position = this.transform.position + transform.forward;

        }


    }

    void OnEnable() {
        InputManager.Instance.PushModalInputHandler(this.gameObject);
        cam = Camera.main;
        drawTransform = this.gameObject.transform;
        cursorOriginalPos = this.transform.position + transform.forward;


    }
    void OnDisable()
    {
        isDraw = false;
        cursorOriginalPos = this.transform.position + transform.forward;
    }

    void Draw2D()
    {
        RaycastHit hit;
        if ((Physics.Raycast(
           drawTransform.position,
           drawTransform.forward,
           out hit,
           Mathf.Infinity,
           Physics.DefaultRaycastLayers)))
        {

            Renderer rend = hit.transform.GetComponent<Renderer>();
            MeshCollider meshCollider = hit.collider as MeshCollider;
            BoxCollider boxCollider = hit.transform.GetComponent<BoxCollider>();



            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                return;

            cursor.transform.position = hit.point;


            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width + drawTransform.position.x;
            pixelUV.y *= tex.height + drawTransform.position.y;



            Circle(tex, (int)pixelUV.x, (int)pixelUV.y, (int)2.5, col);

            tex.Apply();

        }
    }

    public void Circle(Texture2D tex, int cx, int cy, int r, Color col)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tex.SetPixel(px, py, col);
                tex.SetPixel(nx, py, col);
                
                tex.SetPixel(px, ny, col);
                tex.SetPixel(nx, ny, col);
                // remove function for SetPixel
                //manipulate alpha channels SetPixels32
                //how to save these coordinates to a JSON file

            }
        }
    }

    public Color ColorPicked() {
        RaycastHit hit;
        if ((Physics.Raycast(
    Camera.main.transform.position,
     Camera.main.transform.forward,
    out hit,
    Mathf.Infinity,
    Physics.DefaultRaycastLayers)))
        {
            if (hit.collider.tag == "black")
            {
                col = Color.black;
                Debug.Log(col);
            }
            if (hit.collider.tag == "white")
            {
                col = Color.white;
                Debug.Log(col);

            }
            if (hit.collider.tag == "red")
            {
                col = Color.red;
                Debug.Log(col);

            }
            if (hit.collider.tag == "yellow")
            {
                col = Color.yellow;
                Debug.Log(col);

            }
            if (hit.collider.tag == "blue")
            {

                col = Color.blue;
                Debug.Log(col);

            }
            if (hit.collider.tag == "green")
            {
                col = Color.green;
                Debug.Log(col);

            }
            Debug.Log(col);

        }
        return col;
    }
    public void OnInputDown(InputEventData eventData)
    {
        RaycastHit hit;
        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hit,
                Mathf.Infinity,
                Physics.DefaultRaycastLayers))
        {
            if (hit.collider.tag == "terra") ;
            isDraw = true;
        }
        else
            isDraw = false;
    }

    public void OnInputUp(InputEventData eventData)
    {
        isDraw = false;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        ColorPicked();
    }
}