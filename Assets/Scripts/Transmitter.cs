using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System;
using System.Globalization;
using UnityEngine.Networking;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;

namespace BGC.Annotation.Basic
{

    public class Transmitter : MonoBehaviour
    {
        static string protocol = "http://";
        static string host = "localhost";
        static string php = "/repeater/index.php";
        static string group = "mrgeo";
        static string user = "James";
        static string basemap = "map001";
        static float init_time = 1.0f;
        static float interval = 1.0f;
        static string previousInterval = "";
        static StringContent stringContent ;
        public static string previousLabel = "";
        public static  GameObject staticGameObject;
        public static  GameObject updatedGameObject;
        public static string previousjson = "";
        public static string label = "";
        public static string label2 = "";
        public static Transmitter transmitter;
        public static bool serverStatus = false;
        public static bool networkStatus = false;
        private static readonly string ip = "https://jamestestsitedotblog.wordpress.com/";
        public static int count = 1;


        private  Transmitter()
        {
            //Debug.Log("Tranmitter = " + count++);
            if (!serverStatus)
            {
                GetAddress();
                Transmit("");
            }
            if (previousLabel.Equals("")) GetLabel();
        }

        public static Transmitter Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Transmitter instance = new Transmitter();
        }



        async Task<bool> GetAddress()
        {
            HttpClientHandler handler = new HttpClientHandler();
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    //Debug.Log("ObtainAddress.networkStatus = " + networkStatus);
                    //if (previousLabel.Equals(""))
                    //{
                    //string host = ip;
                    var result = await client.GetAsync(ip);
                    if (result.IsSuccessStatusCode) 
                    {
                        string res = await client.GetStringAsync(ip);
                        if (res.Contains("<title"))
                        {
                            //Debug.Log("ObtainAddress.result = " + res);
                            int title = res.IndexOf("<title>");
                            string ip = res.Remove(0, title + 7);

                            char[] delimeter = { '<' };
                            string[] intervalString = ip.Split(delimeter);
                            Debug.Log("header : " + intervalString[0]);
                            host = intervalString[0];
                            networkStatus = true;
                        }
                    }
                    //Debug.Log("ObtainAddress.networkStatus = " + serverStatus);
                    if (networkStatus)
                    {
                        //host = "10.159.23.55";
                        //host = "google.com";
                        var response = await client.GetAsync(protocol + host);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            //Debug.Log("responseBody : " + responseBody);
                            serverStatus = true;
                            Resume();
                        }
                    }
                }
                catch (Exception ex)
                {
                    networkStatus = false;
                    serverStatus = false;
                    //ExceptionHandler.Instance.GetException(ex);
                }
                finally
                {
                    handler.Dispose();
                    client.Dispose();
                }
            }
            //Debug.Log("ObtainAddress.serverStatus = " + serverStatus);
            return serverStatus;
        }




        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Transmitter.Start()");
            InvokeRepeating("RequestInterval", 0.0f, 600.00f);
            //InvokeRepeating("GetAnnotationLabel", init_time, interval);
            //InvokeRepeating("Check", init_time, interval);
            if (this == null) Debug.Log("Transmittrer is Null---");
            else
            {
                transmitter = this;
                transmitter.InvokeRepeating("Loop", init_time, interval);
            }
            Pause();
        }

        // Update is called once per frame
        void Update()
        {

        }

        async public void RequestInterval()
        {
            if (!serverStatus) return;
            HttpClient client = new HttpClient();
            string result = "";
            try
            {
                result = await client.GetStringAsync(protocol + host + "/repeater/request_control.php?group_id=mrgeo&action=request_interval&user_id=134562");
                if (!result.Equals(previousInterval))
                {
                    previousInterval = result;
                    char[] delimeter = { '.' };
                    string[] intervalString = result.Split(delimeter);
                    init_time = float.Parse(intervalString[0], CultureInfo.InvariantCulture.NumberFormat);
                    interval = float.Parse(intervalString[1], CultureInfo.InvariantCulture.NumberFormat);
                    interval = interval / init_time;
                    //Debug.Log("Transmitter.RequestInterval.init_time = " + init_time);
                    //Debug.Log("Transmitter.RequestInterval.interval = " + interval);
                    Resume();
                    Pause();
                    Resume();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
            finally
            {
                client.Dispose();
            }
            client.Dispose();
        }

        async public void GetLabel()
        {
            if (!serverStatus) return;
            string action = "request";
            string result = "";
            HttpClient client = new HttpClient();
            try { 
                    if (previousLabel.Equals(""))
                    {
                        result = await client.GetStringAsync(protocol + host + "/repeater/retrieve_label.php?group_id=" + group + "&basemap_id=" + basemap + "&action=" + action + "&user_id=" + user + "&json=");
                        previousLabel = result;
                    }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
            client.Dispose();
        }


        async public void GetAnnotationLabel()
        {
            if (!serverStatus) return;
            string action = "request";
            string result = "";
            HttpClient client = new HttpClient();
            TextMesh t = new TextMesh();
            GameObject g = staticGameObject;
            label = "";
            try
            {
                //do
                //{
                    result = await client.GetStringAsync(protocol + host + "/repeater/retrieve_label.php?group_id=" + group + "&basemap_id=" + basemap + "&action=" + action + "&user_id=" + user + "&json=");
                    //Debug.Log("Transmitter.GetAnnotationLabel.result = " + previousLabel + " : " + result);

                    if ((staticGameObject != null && result != null && (!result.Equals(previousLabel))))
                    {
                    
                        previousLabel = result;
                        if (staticGameObject.transform.childCount > 0) g = staticGameObject.transform.GetChild(0).gameObject;
                        if (g != null) t = g.transform.GetComponent<TextMesh>();
                        if (t != null) t.text = result;
                        staticGameObject.tag = "annotation";
                        label = result;
                    Debug.Log("Finalize from Transmitter.GetAnnotationLabel");
                        Annotation.Finalize();
                    label = "";
                    //Debug.Log("Transmitter.GetAnna.label = " + staticGameObject.tag + ": " + t.text);
                    //Debug.Log("Transmitter.GetAnna.staticGameObject = " + staticGameObject);
                    //lock (this)
                    //{
                    //Debug.Log("Monitor.Pulse(this); Label : " + result);
                    //Monitor.Pulse(this);
                    //}
                    label2 = "";
                    }
                
                    //await Task.Delay(2000);
               // } while (t == null);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
            client.Dispose();
           // return result;
        }

        public GameObject SetGameObject(GameObject clonedGameObject)
        {
            if (!serverStatus) return staticGameObject;
            //Resume();
            //Pause();
            if (previousLabel.Equals("")) GetLabel();
            staticGameObject = clonedGameObject;
            return staticGameObject;
        }

        async public void Check()
        {
            if (!serverStatus) return;
            try
            { 
                if (!label.Equals(""))
                {
                   // Annotation.Finalize();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }

        }

        public void SetLabel()
        {
            try {
                if (!serverStatus) return;

                //Debug.Log("Transmitter.SetLabel.staticGameObject = " + staticGameObject);
                if (staticGameObject != null && staticGameObject.name.Contains("spot")) {
                    //TextMesh t = new TextMesh();
                    //GameObject g = staticGameObject;
                    //if (staticGameObject.transform.childCount > 0) g = staticGameObject.transform.GetChild(0).gameObject;
                    //if (g != null) t = g.transform.GetComponent<TextMesh>();
                    //if (t != null) t.text = " ";
                    staticGameObject.tag = "annotation";
                    label = " ";
                    //Annotation.Finalize();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }

        }



        async public void Recall(string action)
        {
            if (!serverStatus) return;
            HttpClient client = new HttpClient();
            string result = "";
            try
            {
                result = await client.GetStringAsync(protocol + host + "/repeater/recall.php?group_id=" + group + "&basemap_id=" + basemap + "&action=" + action + "&user_id=" + user);
                if (int.Parse(result) > 0)
                {

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
            finally
            {
                client.Dispose();
            }
            client.Dispose();
        }


        public void Pause()
        {
            if (!serverStatus) return;
            //try
            //{
            Debug.Log("Transmitter.Pause()");
            //InvokeRepeating("Loop", 9999f, 999999f);
            //if (Transmitter.Instance == null) Debug.Log("Transmitter is NULl");
            if (transmitter.IsInvoking("Loop")) {
                //Debug.Log("Transmitter.IsInvoking(Loop) = true");
                transmitter.CancelInvoke();
            }
            
            //CancelInvoke("Request");
            //CancelInvoke("Loop");
            //}
            //catch (Exception ex)
            //{
            //    ExceptionHandler.Instance.GetException(ex);
            //}
            //finally
            //{
            //}
        }


        public void Resume()
        {
            if (!serverStatus) return;
            try
            {
                //InvokeRepeating("Request", init_time, interval);
                Debug.Log("Transmitter.Resume()");
                if (transmitter !=  null) { 
                    transmitter.InvokeRepeating("Loop", init_time, interval);
                }
                //CancelInvoke();
                //InvokeRepeating("Request", init_time, interval);
                //StartCoroutine("Request", interval);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
            finally
            {
            }
        }


        public void Loop()
        {
            if (!serverStatus) return;
            try
            {
                Request();
                GetAnnotationLabel();
                Check();
//                GetAnnotationLabel(staticGameObject);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
            finally
            {
            }
        }

        public void StartRequest(GameObject go)
        {
            if (!serverStatus) return;
            try
            {
                staticGameObject = go;
//                InvokeRepeating("GetAnnotationLabel", init_time, interval);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
            }
            finally
            {
            }
        }


        public void Clear()
        {
            if (!serverStatus) return;
            GameObject[] allAnnotations = UnityEngine.GameObject.FindGameObjectsWithTag("annotation");
            foreach (GameObject annotation in allAnnotations)
            {
                //Debug.Log("Destroy = "  + annotation.name);
                DestroyImmediate(annotation);
            }
            previousjson = "";
        }





        async public void Request()
        {
            if (!serverStatus) return;
            HttpClient client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            string result = "";
            try
            {
                string parameters = "group_id=mrgeo&basemap_id=map001&action=request&user_id=james&json=";
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var stringContent = new StringContent(parameters, Encoding.UTF8, "application/x-www-form-urlencoded");

                int retry = 3;
                do
                {
                    response = await client.PostAsync(protocol + host + php, stringContent);
                    result = await response.Content.ReadAsStringAsync();
                    //Debug.Log(this.name + " : Request() - " + result);
                    if (result != null && result != "" && !result.Equals(previousjson))
                    {
                        previousjson = result;
                        Clear();
                        JSONParser.Instance.FromJSON(result);
                    }
                } while (!response.IsSuccessStatusCode && retry-- > 0);
                if (!response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
                client.Dispose();
            }
            finally
            {
                response.Dispose();
                client.Dispose();
            }

        }



        async public void Transmit(string json)
        {
            GetAddress();
            if (!serverStatus) return;
            previousjson = json;
            HttpClient client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            string result = "";
            //Debug.Log("Transmit(string json) = " + json);
            try
            {
                string parameters = "group_id=mrgeo&basemap_id=map001&action=update&user_id=james&json=";
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var stringContent = new StringContent(parameters + json, Encoding.UTF8, "application/x-www-form-urlencoded");

                int retry = 3;
                do
                {
                    response = await client.PostAsync(protocol + host + php , stringContent);
                    result = await response.Content.ReadAsStringAsync();
                    //Debug.Log(" : Transmit().result = " + result);
                } while (!response.IsSuccessStatusCode && retry-- > 0);
                if (!response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Instance.GetException(ex);
                client.Dispose();
            }
            response.Dispose();
            client.Dispose();

        }

    }
}
