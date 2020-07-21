using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;

public class TrackPlanes : MonoBehaviour
{
    private GameObject activePlane;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void registerReference(GameObject plane)
    {
        activePlane = plane;
    }

    public GameObject getActivePlane()
    {
        return activePlane;
    }

    public void clearActivePlaneListeners()
    {
        activePlane.GetComponent<TwoHandManipulatablePlanes>().ClearAllListeners();
    }
}
