using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit;
using HoloToolkit.Unity.InputModule;
using UnityEngine.XR.WSA.Input;

public class MenuUnlock : MonoBehaviour
{
    const float DELAY = .5f;

    void Start()
    {
        GestureRecognizer recognizer = new GestureRecognizer();
        recognizer.StartCapturingGestures();

        recognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap);
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            if (tapCount == 1)
                Invoke("SingleTap", DELAY);
            else if (tapCount == 2)
            {
                CancelInvoke("SingleTap");
                DoubleTap();
            }
        };
    }

    void SingleTap()
    {
        Debug.Log("Single Tap");
    }

    void DoubleTap()
    {
        Debug.Log("Double Tap");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
