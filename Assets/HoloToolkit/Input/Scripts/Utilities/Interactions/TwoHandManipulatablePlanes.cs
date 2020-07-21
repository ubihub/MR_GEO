// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity.UX;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HoloToolkit.Unity.InputModule.Utilities.Interactions
{
    /// <summary>
    /// This script allows for an object to be movable, scalable, and rotatable with one or two hands. 
    /// You may also configure the script on only enable certain manipulations. The script works with 
    /// both HoloLens' gesture input and immersive headset's motion controller input.
    /// See Assets/HoloToolkit-Examples/Input/Readme/README_TwoHandManipulationTest.md
    /// for instructions on how to use the script.
    /// </summary>
    public class TwoHandManipulatablePlanes : MonoBehaviour, IInputHandler, ISourceStateHandler
    {
        public Material reg;
        public Material sel;

        [SerializeField]
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform = null;

        public Transform HostTransform
        {
            get { return hostTransform; }
            set { hostTransform = value; }
        }

        [SerializeField]
        [Tooltip("To visualize the object bounding box, drop the HoloToolkit/UX/Prefabs/BoundingBoxes/BoundingBoxBasic.prefab here. This is optional.")]
        private BoundingBox boundingBoxPrefab = null;

        /// <summary>
        /// Reference to the Prefab from which clone is instantiated.
        /// </summary>
        public BoundingBox BoundingBoxPrefab
        {
            set { boundingBoxPrefab = value; }
            get { return boundingBoxPrefab; }
        }

        [SerializeField]
        [Tooltip("What manipulation will two hands perform?")]
        private ManipulationMode manipulationMode = ManipulationMode.None;

        public ManipulationMode ManipulationMode
        {
            get { return manipulationMode; }
            set { manipulationMode = value; }
        }

        [SerializeField]
        [Tooltip("Constrain rotation along an axis")]
        private AxisConstraint rotationConstraint = AxisConstraint.None;

        public AxisConstraint RotationConstraint
        {
            get { return rotationConstraint; }
            set
            {
                rotationConstraint = value;
                rotateLogic = new TwoHandRotateLogic(rotationConstraint);
            }
        }

        [SerializeField]
        [Tooltip("If true, grabbing the object with one hand will initiate movement.")]
        private bool enableOneHandMovement = false;

        public bool EnableEnableOneHandedMovement
        {
            get { return enableOneHandMovement; }
            set { enableOneHandMovement = value; }
        }

        // Private fields that store transform information.
        #region Transform Info

        private BoundingBox boundingBoxInstance;
        private ManipulationMode currentState;
        private TwoHandMoveLogic moveLogic;
        private TwoHandScaleLogic scaleLogic;
        private TwoHandRotateLogic rotateLogic;

        #endregion Transform Info

        /// <summary>
        /// Maps input id -> position of hand.
        /// </summary>
        private readonly Dictionary<uint, Vector3> handsPressedLocationsMap = new Dictionary<uint, Vector3>();

        /// <summary>
        /// Maps input id -> input source. Then obtain position of input source using currentInputSource.TryGetGripPosition(currentInputSourceId, out inputPosition);
        /// </summary>
        private readonly Dictionary<uint, IInputSource> handsPressedInputSourceMap = new Dictionary<uint, IInputSource>();

        /// <summary>
        /// Property that turns on and off the Visibility of the BoundingBox cloned from the BoundingBoxPrefab reference.
        /// </summary>
        private bool ShowBoundingBox
        {
            set
            {
                if (boundingBoxPrefab != null)
                {
                    if (boundingBoxInstance == null)
                    {
                        // Instantiate Bounding Box from the Prefab
                        boundingBoxInstance = Instantiate(boundingBoxPrefab) as BoundingBox;
                    }

                    if (value)
                    {
                        boundingBoxInstance.Target = HostTransform.gameObject;
                        boundingBoxInstance.gameObject.SetActive(true);
                    }
                    else
                    {
                        boundingBoxInstance.Target = null;
                        boundingBoxInstance.gameObject.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Change the manipulation mode.
        /// </summary>
        [System.Obsolete("Use ManipulationMode.")]
        public void SetManipulationMode(ManipulationMode mode)
        {
            manipulationMode = mode;
        }

        private void Awake()
        {
            moveLogic = new TwoHandMoveLogic();
            rotateLogic = new TwoHandRotateLogic(rotationConstraint);
            scaleLogic = new TwoHandScaleLogic();
        }

        private Interactive scaleScript;
        private Interactive moveScript;
        private InteractiveToggle moveToggleScript;
        private Interactive rotateXScript;
        private Interactive rotateYScript;
        private Interactive rotateZScript;

        private void Start()
        {
            if (hostTransform == null)
            {
                hostTransform = transform;
            }

            // show scale, rotate button and register listeners
            GameObject buttons = GameObject.Find("Buttons"); // parent GO of buttons
            //buttons.transform.position = transform.position;

            GameObject scaleButton = buttons.transform.Find("Scale").gameObject;
            scaleButton.transform.localScale = new Vector3(1, 1, 1); // unhide

            //GameObject moveButton = buttons.transform.Find("Move").gameObject;
            //moveButton.transform.localScale = new Vector3(1, 1, 1);

            GameObject moveToggleButton = buttons.transform.Find("MoveToggle").gameObject;
            moveToggleButton.transform.localScale = new Vector3(1, 1, 1);

            GameObject rotateXButton = buttons.transform.Find("RotateX").gameObject;
            rotateXButton.transform.localScale = new Vector3(1, 1, 1);

            GameObject rotateYButton = buttons.transform.Find("RotateY").gameObject;
            rotateYButton.transform.localScale = new Vector3(1, 1, 1);

            GameObject rotateZButton = buttons.transform.Find("RotateZ").gameObject;
            rotateZButton.transform.localScale = new Vector3(1, 1, 1);

            // assign this object as listener to buttons
            scaleScript = scaleButton.GetComponent<Interactive>();
            scaleScript.OnDownEvent.AddListener(SetScale);

            //moveScript = moveButton.GetComponent<Interactive>();
            //moveScript.OnDownEvent.AddListener(SetMoveOn);

            moveToggleScript = moveToggleButton.GetComponent<InteractiveToggle>();
            moveToggleScript.OnSelection.AddListener(SetMoveOn);
            moveToggleScript.OnDeselection.AddListener(SetMoveOff);

            rotateXScript = rotateXButton.GetComponent<Interactive>();
            rotateXScript.OnDownEvent.AddListener(SetXRotation);

            rotateYScript = rotateYButton.GetComponent<Interactive>();
            rotateYScript.OnDownEvent.AddListener(SetYRotation);

            rotateZScript = rotateZButton.GetComponent<Interactive>();
            rotateZScript.OnDownEvent.AddListener(SetZRotation);
        }

        public void ClearAllListeners()
        {
            scaleScript.OnDownEvent.RemoveAllListeners();
            //moveScript.OnDownEvent.RemoveAllListeners();
            moveToggleScript.OnSelection.RemoveAllListeners();
            rotateXScript.OnDownEvent.RemoveAllListeners();
            rotateYScript.OnDownEvent.RemoveAllListeners();
            rotateZScript.OnDownEvent.RemoveAllListeners();
        }

        public void SetScale() 
        {
            Debug.Log("Setting scale...");
            ManipulationMode = ManipulationMode | ManipulationMode.Scale;
        }

        public void SetMoveOn()
        {
            Debug.Log("Turning on move...");
            enableOneHandMovement = true;
        }

        public void SetMoveOff()
        {
            Debug.Log("Turning off move...");
            enableOneHandMovement = false;
        }

        public void SetXRotation()
        {
            Debug.Log("Setting X rotation...");
            ManipulationMode = ManipulationMode | ManipulationMode.Rotate;
            rotateLogic = new TwoHandRotateLogic(AxisConstraint.XAxisOnly);
        }

        public void SetYRotation()
        {
            Debug.Log("Setting Y rotation...");
            ManipulationMode = ManipulationMode | ManipulationMode.Rotate;
            rotateLogic = new TwoHandRotateLogic(AxisConstraint.YAxisOnly);
        }

        public void SetZRotation()
        {
            Debug.Log("Setting Z rotation...");
            ManipulationMode = ManipulationMode | ManipulationMode.Rotate;
            rotateLogic = new TwoHandRotateLogic(AxisConstraint.ZAxisOnly);
        }

        private void Update()
        {
            // Update positions of all hands
            foreach (var key in handsPressedInputSourceMap.Keys)
            {
                var inputSource = handsPressedInputSourceMap[key];
                Vector3 inputPosition;
                if (inputSource.TryGetGripPosition(key, out inputPosition))
                {
                    handsPressedLocationsMap[key] = inputPosition;
                }
            }

            if (currentState != ManipulationMode.None)
            {
                UpdateStateMachine();
            }
        }

        private Vector3 GetInputPosition(InputEventData eventData)
        {
            Vector3 result;
            eventData.InputSource.TryGetGripPosition(eventData.SourceId, out result);
            return result;
        }

        private void RemoveSourceIdFromHandMap(uint sourceId)
        {
            if (handsPressedLocationsMap.ContainsKey(sourceId))
            {
                handsPressedLocationsMap.Remove(sourceId);
            }

            if (handsPressedInputSourceMap.ContainsKey(sourceId))
            {
                handsPressedInputSourceMap.Remove(sourceId);
            }
        }

        /// <summary>
        /// Event Handler receives input from inputSource.
        /// </summary>
        public void OnInputDown(InputEventData eventData)
        {
            // Add to hand map
            handsPressedLocationsMap[eventData.SourceId] = GetInputPosition(eventData);
            handsPressedInputSourceMap[eventData.SourceId] = eventData.InputSource;
            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// Event Handler receives input from inputSource.
        /// </summary>
        public void OnInputUp(InputEventData eventData)
        {
            RemoveSourceIdFromHandMap(eventData.SourceId);
            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// OnSourceDetected Event Handler.
        /// </summary>
        public void OnSourceDetected(SourceStateEventData eventData) { }

        /// <summary>
        /// OnSourceLost Event Handler.
        /// </summary>
        public void OnSourceLost(SourceStateEventData eventData)
        {
            RemoveSourceIdFromHandMap(eventData.SourceId);
            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// Updates the state machine based on the current state and anything that might have changed with the hands.
        /// </summary>
        private void UpdateStateMachine()
        {
            var handsPressedCount = handsPressedLocationsMap.Count;
            ManipulationMode newState = currentState;

            switch (currentState)
            {
                case ManipulationMode.None:
                case ManipulationMode.Move:
                    if (handsPressedCount == 0)
                    {
                        newState = ManipulationMode.None;
                    }
                    else if (handsPressedCount == 1)
                    {
                        newState = enableOneHandMovement ? ManipulationMode.Move : ManipulationMode.None;
                    }
                    else if (handsPressedCount > 1)
                    {
                        newState = manipulationMode;
                    }
                    break;
                case ManipulationMode.Scale:
                case ManipulationMode.Rotate:
                case ManipulationMode.MoveAndScale:
                case ManipulationMode.MoveAndRotate:
                case ManipulationMode.RotateAndScale:
                case ManipulationMode.MoveScaleAndRotate:
                    if (handsPressedCount == 0)
                    {
                        newState = ManipulationMode.None;
                    }
                    else if (handsPressedCount == 1)
                    {
                        newState = enableOneHandMovement ? ManipulationMode.Move : ManipulationMode.None;
                    }
                    break;
            }

            InvokeStateUpdateFunctions(currentState, newState);
            currentState = newState;
        }

        private void InvokeStateUpdateFunctions(ManipulationMode oldState, ManipulationMode newState)
        {
            if (newState != oldState)
            {
                switch (newState)
                {
                    case ManipulationMode.None:
                        OnManipulationEnded();
                        break;
                    case ManipulationMode.Move:
                        OnOneHandMoveStarted();
                        break;
                    case ManipulationMode.Scale:
                    case ManipulationMode.Rotate:
                    case ManipulationMode.MoveAndScale:
                    case ManipulationMode.MoveAndRotate:
                    case ManipulationMode.RotateAndScale:
                    case ManipulationMode.MoveScaleAndRotate:
                        OnTwoHandManipulationStarted(newState);
                        break;
                }

                switch (oldState)
                {
                    case ManipulationMode.None:
                        OnManipulationStarted();
                        break;
                    case ManipulationMode.Move:
                        break;
                    case ManipulationMode.Scale:
                    case ManipulationMode.Rotate:
                    case ManipulationMode.MoveAndScale:
                    case ManipulationMode.MoveAndRotate:
                    case ManipulationMode.RotateAndScale:
                    case ManipulationMode.MoveScaleAndRotate:
                        OnTwoHandManipulationEnded();
                        break;
                }
            }
            else
            {
                switch (newState)
                {
                    case ManipulationMode.None:
                        break;
                    case ManipulationMode.Move:
                        OnOneHandMoveUpdated();
                        break;
                    case ManipulationMode.Scale:
                    case ManipulationMode.Rotate:
                    case ManipulationMode.MoveAndScale:
                    case ManipulationMode.MoveAndRotate:
                    case ManipulationMode.RotateAndScale:
                    case ManipulationMode.MoveScaleAndRotate:
                        OnTwoHandManipulationUpdated();
                        break;
                }
            }
        }

        private void OnTwoHandManipulationUpdated()
        {
#if UNITY_2017_2_OR_NEWER
            var targetRotation = hostTransform.rotation;
            var targetPosition = hostTransform.position;
            var targetScale = hostTransform.localScale;

            if ((currentState & ManipulationMode.Move) > 0)
            {
                targetPosition = moveLogic.Update(GetHandsCentroid(), targetPosition);
            }

            if ((currentState & ManipulationMode.Rotate) > 0)
            {
                targetRotation = rotateLogic.Update(handsPressedLocationsMap, hostTransform, targetRotation);
            }

            if ((currentState & ManipulationMode.Scale) > 0)
            {
                targetScale = scaleLogic.UpdateMap(handsPressedLocationsMap);
            }

            hostTransform.position = targetPosition;
            hostTransform.rotation = targetRotation;
            hostTransform.localScale = targetScale;
#endif // UNITY_2017_2_OR_NEWER
        }

        private void OnOneHandMoveUpdated()
        {
            var targetPosition = moveLogic.Update(handsPressedLocationsMap.Values.First(), hostTransform.position);

            hostTransform.position = targetPosition;
        }

        private void OnTwoHandManipulationEnded()
        {
            Material[] mats = GetComponent<Renderer>().materials;
            Debug.Log("Two hand manipulation ended" + mats);
            mats[0] = reg;
            GetComponent<Renderer>().materials = mats;
#if UNITY_2017_2_OR_NEWER
            // This implementation currently does nothing
#endif // UNITY_2017_2_OR_NEWER
        }

        private Vector3 GetHandsCentroid()
        {
            Vector3 result = handsPressedLocationsMap.Values.Aggregate(Vector3.zero, (current, state) => current + state);
            return result / handsPressedLocationsMap.Count;
        }

        private void OnTwoHandManipulationStarted(ManipulationMode newState)
        {
            Material[] mats = GetComponent<Renderer>().materials;
            Debug.Log("Two hand manipulation started" + mats);
            mats[0] = sel;
            GetComponent<Renderer>().materials = mats;
#if UNITY_2017_2_OR_NEWER
            if ((newState & ManipulationMode.Rotate) > 0)
            {
                rotateLogic.Setup(handsPressedLocationsMap, hostTransform);
            }

            if ((newState & ManipulationMode.Move) > 0)
            {
                moveLogic.Setup(GetHandsCentroid(), hostTransform);
            }

            if ((newState & ManipulationMode.Scale) > 0)
            {
                scaleLogic.Setup(handsPressedLocationsMap, hostTransform);
            }
#endif // UNITY_2017_2_OR_NEWER
        }

        private void OnOneHandMoveStarted()
        {
            Assert.IsTrue(handsPressedLocationsMap.Count == 1);

            moveLogic.Setup(handsPressedLocationsMap.Values.First(), hostTransform);
        }

        private void OnManipulationStarted()
        {
            InputManager.Instance.PushModalInputHandler(gameObject);

            // Show Bounding Box visual on manipulation interaction
            ShowBoundingBox = true;
        }

        private void OnManipulationEnded()
        {
            InputManager.Instance.PopModalInputHandler();

            // Hide Bounding Box visual on release
            ShowBoundingBox = false;
        }
    }
}
