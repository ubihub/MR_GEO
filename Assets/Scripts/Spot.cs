using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// James look at the note I added at the bottom if you are trying to add exeptions righ now
namespace BGC.Annotation.Basic
{
    /**
     * 
     * 
     * */
    public class Spot : Annotation, IInputClickHandler
    {

        public GameObject spotSymbol;
        static public GameObject spotInstance { get; set; }
        RaycastHit hitInfo;
        static string type;
        Sprite[] Industrial_Symbols;
        bool KeepGoing = false;

        private void Awake()
        {
            // Industrial_Symbols = Resources.LoadAll<Sprite>("Industrial_Symbols");
        }

        private Spot()
        {
            spotInstance = Resources.Load("spotObject") as GameObject;

            /*           Industrial_Symbols = Resources.LoadAll<Sprite>("Industrial_Symbols");
                       Debug.Log("Spt.Start.Industrial_Symbols.GetChild = " + Industrial_Symbols.Length);
                       GameObject[] allSymbolButtons = UnityEngine.GameObject.FindGameObjectsWithTag("symbol");
                       Debug.Log("Spt.Start.allSymbolButtons.GetChild = " + allSymbolButtons.Length);
                       foreach (GameObject symbolButton in allSymbolButtons)
                       {
                           Button  b = symbolButton.GetComponent<Button>();
                           b.image.sprite = Industrial_Symbols[1];
                       }
                       */
        }

        public static Spot Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Spot instance = new Spot();
        }

        public static void Done()
        {
            try
            {
                if (Transmitter.staticGameObject != null )Transmitter.staticGameObject.tag = "annotation";
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
        }


        public void OnInputClicked(InputClickedEventData eventData)
        {
            try
            {
                //Transmitter.Instance.SetLabel();
                Debug.Log(this.name + " : " + annotationType);
                Debug.Log(this.name + ".mode = " + mode + " : count = " + count);
                if (KeepGoing)
                {
                    clonedGameObject = InstantiateFromEventData(eventData);
                    clonedGameObject = Transmitter.Instance.SetGameObject(clonedGameObject);

                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
            //        throw new System.NotImplementedException();
        }
        //public void SetType()
        //{
        //annotationType = (Annotation.AnnotationTypes)Enum.Parse(typeof(Annotation.AnnotationTypes), type.ToLower());
        //}

        // It is needed this for the clone of the spot
        //return to parent after
        //[SerializeField] private UnityEngine.UI.Image image = null;
        public GameObject InstantiateFromEventData(InputClickedEventData eventData)
        {
            //if (!type.Equals("spot")) return null;
            spotSymbol = Resources.Load("spotObject") as GameObject;
            if (Physics.Raycast(
                 Camera.main.transform.position,
                 Camera.main.transform.forward,
                 out hitInfo,
                 Mathf.Infinity,
                 Physics.DefaultRaycastLayers))
            {
                //Debug.Log("Spot.InstantiateFromEventData.spotSymbol = " + spotSymbol);
                if (Annotation.annotationType != Annotation.AnnotationTypes.spot || mode != "creation") return null;
                //spotSymbol = Resources.Load("spotObject") as GameObject;
                //Texture2D m_Image = Resources.Load<Texture2D>("proposed_borehole_colourdrawing.png");
                //Debug.Log("Spot : spotSymbol.tag = " + spotSymbol.tag);
                if (spotSymbol != null)
                {
                    spotInstance = Instantiate(spotSymbol, hitInfo.point, Quaternion.identity);
                    spotInstance.name = "spot";
                    spotInstance.tag = "new";
                    SpriteRenderer sr = spotInstance.GetComponent<SpriteRenderer>();
                    //spotInstance.GetComponent<RawImage>().texture = m_Image;
                    // Debug.Log("SpriteRenderer sr =" + SymbolHandler.GetSprite());
                    //Debug.Log("SymbolHandler.GetSprite()" + SymbolHandler.GetSprite());
                    sr.sprite = SymbolHandler.GetSprite();
                }
                if (spotInstance.transform.childCount > 0)
                {
                    GameObject g = spotInstance.transform.GetChild(0).gameObject;
                    TextMesh t = g.transform.GetComponent<TextMesh>();
                    t.text = "";
                }
            }
            return spotInstance;

        }

        /*
                public int SetSpotSymbol(Sprite sprite)
                {
                    SpriteRenderer sr = spotInstance.GetComponent<SpriteRenderer>();
                    sr.sprite = sprite;

                    return 0;
                }
        */
        public AnnotationEntity MakeAnnotationEntity(GameObject instance)
        {
            //Debug.Log("MakeAnnotationEntity(GameObject instance) = " + instance);
            AnnotationEntity annotationEntity = new AnnotationEntity();
            if (instance == null) return annotationEntity;
            annotationEntity.tag = "annotation";
            annotationEntity.type = "spot";
            //annotationEntity.symbol = SymbolHandler.symbolNumber;
            //annotationEntity.instanceID = instance.GetInstanceID();
            Position position = new Position();
            position.x = instance.transform.position.x;
            position.y = instance.transform.position.y;
            position.z = instance.transform.position.z;
            annotationEntity.position = position;
            GameObject g = instance.transform.GetChild(0).gameObject;
            TextMesh t = g.transform.GetComponent<TextMesh>();
            annotationEntity.label = t.text;
            SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
            //spotInstance.GetComponent<RawImage>().texture = m_Image;
            //Debug.Log("SpriteRenderer sr =" + SymbolHandler.GetSprite());
            //sr.sprite = SymbolHandler.GetSprite();
            annotationEntity.symbol = sr.sprite.name;
            return annotationEntity;
            //throw new System.NotImplementedException();
        }

        /*
                public void InstantiateFromPosition(Vector3 position)
                {
                    {
                        if (spotSymbol != null && position != null)
                        Instantiate(spotSymbol, position, Quaternion.identity);

                    }
                }
        */

        public AnnotationEntity InstantiateFromEntity(AnnotationEntity annotationEntity)
        {
            GameObject g;
            Vector3 position = new Vector3(annotationEntity.position.x, annotationEntity.position.y, annotationEntity.position.z);
            Quaternion quaternion = new Quaternion(annotationEntity.rotation.x, annotationEntity.rotation.y, annotationEntity.rotation.z, annotationEntity.rotation.w);
            spotSymbol = Resources.Load("spotObject") as GameObject;
            //Debug.Log("Spot.InstantiateFromEntity.spotSymbol = " + spotSymbol);
            if (spotSymbol != null)
            {
                spotInstance = Instantiate(spotSymbol, position, quaternion);
                spotInstance.tag = "annotation";
                spotInstance.name = "spot";
                if (spotInstance.transform.childCount > 0)
                {
                    g = spotInstance.transform.GetChild(0).gameObject;
                    TextMesh t = g.transform.GetComponent<TextMesh>();
                    t.text = annotationEntity.label;
                }
                if (!annotationEntity.symbol.Equals(""))
                {
                    SpriteRenderer sr = spotInstance.GetComponent<SpriteRenderer>();
                    //spotInstance.GetComponent<RawImage>().texture = m_Image;
                    //Debug.Log("SpriteRenderer sr =" + SymbolHandler.GetSprite());
                    sr.sprite = SymbolHandler.GetSprite(annotationEntity.symbol);
                }
            }
            return annotationEntity;
        }

        // Start is called before the first frame update
        void Start()
        {
            InputManager.Instance.PushModalInputHandler(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (annotationType == AnnotationTypes.spot && GetMode() == "creation")
            {
                KeepGoing = true;
            }
            else if (annotationType == AnnotationTypes.spot && GetMode() == "done")
            {
                KeepGoing = false;

                SpriteRenderer sr = spotInstance.GetComponent<SpriteRenderer>();
                sr.sprite = Industrial_Symbols[0];
                Debug.Log("Finalize from Spot.Update");

                Finalize();
            }
            else if (annotationType == AnnotationTypes.spot && GetMode() == "cancel")
            {
                KeepGoing = false;
                Cancel();
            }
            else if (annotationType == AnnotationTypes.spot && GetMode() == "undo")
            {
                KeepGoing = false;
                Undo();
            }
            else if (annotationType == AnnotationTypes.spot && GetMode() == "redo")
            {
                KeepGoing = false;
                Redo();
            }

        }

        //Hi James
        //Sorry I added this because I could not test with the errors this cs file was giving us.
        public void OnDeselect(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnSelect(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}