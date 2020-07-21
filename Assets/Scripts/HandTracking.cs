using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit;
using HoloToolkit.Unity.InputModule;
using UnityEngine.XR.WSA.Input;

public class HandTracking : MonoBehaviour, IInputSourceInfoProvider
{
    public IInputSource InputSource => throw new System.NotImplementedException();

    public uint SourceId => throw new System.NotImplementedException();

    // Start is called before the first frame update


    private void Awake()
    {
        
    }

    private void InteractionManager_SourceUpdated(InteractionSourceUpdatedEventArgs hand)
    {
        if (hand.state.source.kind == InteractionSourceKind.Hand)
        {
            Vector3 handPosition;
            hand.state.sourcePose.TryGetPosition(out handPosition);
            Debug.Log(handPosition);
        }

    }

    private void InteractionManager_SourceDetected(InteractionSourceState hand)
    {
        Debug.Log("Source detected!");
    }

    private void InteractionManager_SourceLost(InteractionSourceState hand)
    {
        Debug.Log("Source lost!");
    }
}