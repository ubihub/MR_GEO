using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;


public class OnClickCancel : MonoBehaviour, IInputClickHandler
{
    
    void Start() {
        InputManager.Instance.PushModalInputHandler(this.gameObject);
       // Polyline_test Cancel = gameObject.GetComponent<Polyline_test>();

    }

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
       // Cancel.DestroyAllClones();
    }
}
