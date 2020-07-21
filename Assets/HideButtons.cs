using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideButtons : MonoBehaviour
{
    public void hide()
    {
        // hide all children buttons
        transform.Find("Scale").localScale = new Vector3(0, 0, 0);
        transform.Find("MoveToggle").localScale = new Vector3(0, 0, 0);
        transform.Find("RotateX").localScale = new Vector3(0, 0, 0);
        transform.Find("RotateY").localScale = new Vector3(0, 0, 0);
        transform.Find("RotateZ").localScale = new Vector3(0, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
