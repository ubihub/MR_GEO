using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;


namespace BGC.Annotation.Basic
{


    public class SymbolHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, UnityEngine.EventSystems.ISelectHandler
    {

        public static Sprite[] Industrial_Symbols;
        public static Annotation.AnnotationTypes currentType;
        public bool toggle = true;
        public bool changed = false;
        public static GameObject staticGameObject;
        public static Sprite sprite { get; set; }
        public static string symbolNumber { get; set; }
        private int flag = 0;

        public static Sprite GetSprite()
        {
            return sprite;
        }

        public static void SetSprite(Sprite spr)
        {
            sprite = spr;
        } 

        void Start()
        {
            Industrial_Symbols = Resources.LoadAll<Sprite>("Industrial_Symbols");
            sprite = Industrial_Symbols[0];
        }

        public static SymbolHandler Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly SymbolHandler instance = new SymbolHandler();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("pointer enter");
            //Debug.Log("SymbolHandler,Annotation.annotationType = " + Annotation.annotationType);
            //Debug.Log("SymbolHandler,Annotation.mode = " + Annotation.mode);
            currentType = Annotation.annotationType;
            //if (Annotation.annotationType != Annotation.AnnotationTypes.undefined)
            {
                Annotation.SetMode("selection", 0);
                Annotation.mode = "selection";
                flag = 0;
                //Debug.Log("OnPointerEnter(PointerEventData eventData)count = " + Annotation.count);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("pointer exit");
            //Debug.Log("SymbolHandler.Annotation.annotationType = " + Annotation.annotationType);
            //Debug.Log("SymbolHandler.Annotation.mode = " + Annotation.mode);
            //if (Annotation.annotationType != Annotation.AnnotationTypes.undefined)
            {
                Annotation.SetMode("creation", 2);
                //Annotation.count = 2;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            try
            {
                Debug.Log("OnSelect = " + flag);
                if ( flag < 1  ) { flag++; return; } 
                else
                {
                    //Debug.Log("SymbolHandler.OnSelect.this.gameObject.name.ToLower() = " + this.gameObject.name.ToLower());
                    //Debug.Log("SymbolHandler.staticGameObject = " + staticGameObject);
                    //if (staticGameObject == null) return;
                    Debug.Log("SymbolHandler.staticGameObject = " + staticGameObject);
                    Debug.Log("symbolNumber = " + symbolNumber);
                    symbolNumber = this.gameObject.name.ToLower();
                    toggle = true;
                    changed = true;
                    sprite = GetSprite(symbolNumber);
                    Debug.Log("SymbolHandler.sprite = " + sprite);
                    //Annotation.SetMode("creation", 2);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
        }

        public static Sprite GetSprite(string symbolNumber)
        {
            Industrial_Symbols = Resources.LoadAll<Sprite>("Industrial_Symbols");
            Sprite sp = Industrial_Symbols[0];
            if (symbolNumber.Equals("symbol_1"))
            {
                sp = Industrial_Symbols[1];
            }
            else if (symbolNumber.Equals("symbol_2"))
            {
                sp = Industrial_Symbols[2];

            }
            else if (symbolNumber.Equals("symbol_3"))
            {
                sp = Industrial_Symbols[3];

            }
            else if (symbolNumber.Equals("symbol_4"))
            {
                sp = Industrial_Symbols[4];
            }
            else if (symbolNumber.Equals("symbol_5"))
            {
                sp = Industrial_Symbols[5];

            }
            //Debug.Log("SymbolHandler.GetSrptie() = " + symbolNumber + " " + sp);
            return sp;
        }




        /*
                public GameObject SetSpotInstance(GameObject spotInstance)
                {
                    staticGameObject = spotInstance;
                    Debug.Log("SymbolHandler.spotInstance = " + staticGameObject);
                    return staticGameObject;
                }

                //       public Sprite GetSymbolSprite(string symbolName)
                // {



                //}


                public void LoadSymbols()
                {
                    Industrial_Symbols = Resources.LoadAll<Sprite>("Industrial_Symbols");
                    //Debug.Log("Spt.Start.Industrial_Symbols.GetChild = " + Industrial_Symbols.Length);
                    //GameObject[] allSymbolButtons = UnityEngine.GameObject.FindGameObjectsWithTag("symbol");
                    //Button[] allSymbolButtons = gameObject.GetComponentsInChildren<Button>();
                    //UnityEngine.Component[] allSymbolButtons = gameObject.GetComponentsInChildren(typeof(Button));
                    UnityEngine.UI.Button[] allSymbolButtons = this.GetComponentsInChildren<Button>();
                    //Debug.Log("Spt.Start.allSymbolButtons.GetChild = " + allSymbolButtons.Length);
                    //foreach (Button symbolButton in allSymbolButtons)
                    //{
                        //Button b = symbolButton.GetComponent<Button>();
                        //b.image.sprite = Industrial_Symbols[1];
                    //}

                }
        */
    }
}