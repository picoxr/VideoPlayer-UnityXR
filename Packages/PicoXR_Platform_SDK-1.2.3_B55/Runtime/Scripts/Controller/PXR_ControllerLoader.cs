/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System.Collections;
using System.IO;
using LitJson;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_ControllerLoader : MonoBehaviour
    {
        [SerializeField]
        private PXR_Input.Controller hand;

        public bool customModel = false;

        public GameObject g2;
        public GameObject neo2L;
        public GameObject neo2R;
        public GameObject neo3L;
        public GameObject neo3R;

        public Material mobileMaterial;
        public Material standardMaterial;

        private bool loadModelSuccess = false;

        private int controllerType = -1;

        private JsonData curControllerData = null;
        private int systemOrLocal = 0;

        private string modelName = "";
        private string texFormat = "";
        private string prePath = "";
        private string modelFilePath = "/system/media/pxrRes/controller/";
        private const string g2TexBasePath = "Controller/G2/controller3";
        private const string neo2TexBasePath = "Controller/Neo2/controller4";
        private const string neo3TexBasePath = "Controller/Neo3/controller5";

        private bool leftControllerState = false;
        private bool rightControllerState = false;

        private enum ControllerSimulationType
        {
            None,
            G2,
            Neo2,
            Neo3,
        }

        [SerializeField]
        private ControllerSimulationType controllerSimulation = ControllerSimulationType.None;
        public PXR_ControllerLoader(PXR_Input.Controller controller)
        {
            hand = controller;
        }

        void Awake()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            PXR_Plugin.System.UPxr_GetIntConfig((int)GlobalIntConfigs.CtrlModelLoadingPri, ref systemOrLocal);
#endif
#if UNITY_EDITOR
            switch (controllerSimulation)
            {
                case ControllerSimulationType.G2:
                    {
                        var g2Object = Instantiate(hand == PXR_Input.Controller.LeftController ? g2 : null, transform, false);
                        var g2Comp = g2Object.AddComponent<PXR_ControllerKeyEffects>();
                        g2Comp.hand = hand;
                        LoadTexture(g2Comp, g2TexBasePath, true);
                        break;
                    }
                case ControllerSimulationType.Neo2:
                    {
                        var neo2Object = Instantiate(hand == PXR_Input.Controller.LeftController ? neo2L : neo2R, transform, false);
                        var neo2Comp = neo2Object.AddComponent<PXR_ControllerKeyEffects>();
                        neo2Comp.hand = hand;
                        LoadTexture(neo2Comp, neo2TexBasePath, true);
                        break;
                    }
                case ControllerSimulationType.Neo3:
                    {
                        var neo3Object = Instantiate(hand == PXR_Input.Controller.LeftController ? neo3L : neo3R, transform, false);
                        var neo3Comp = neo3Object.AddComponent<PXR_ControllerKeyEffects>();
                        neo3Comp.hand = hand;
                        LoadTexture(neo3Comp, neo3TexBasePath, true);
                        break; ;
                    }
            }
#endif
        }

        void Start()
        {
            if (!customModel)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                LoadResFromJson();
#endif
                controllerType = PXR_Plugin.Controller.UPxr_GetControllerType();
                leftControllerState = PXR_Input.IsControllerConnected(PXR_Input.Controller.LeftController);
                rightControllerState = PXR_Input.IsControllerConnected(PXR_Input.Controller.RightController);
                if (hand == PXR_Input.Controller.LeftController)
                    RefreshController(PXR_Input.Controller.LeftController);
                if (hand == PXR_Input.Controller.RightController)
                    RefreshController(PXR_Input.Controller.RightController);
            }
        }

        void Update()
        {
            if (!customModel)
            {
                if (hand == PXR_Input.Controller.LeftController)
                {
                    if (PXR_Input.IsControllerConnected(PXR_Input.Controller.LeftController))
                    {
                        if (!leftControllerState)
                        {
                            controllerType = PXR_Plugin.Controller.UPxr_GetControllerType();
                            RefreshController(PXR_Input.Controller.LeftController);
                            leftControllerState = true;
                        }
                    }
                    else
                    {
                        if (leftControllerState)
                        {
                            DestroyLocalController();
                            leftControllerState = false;
                        }
                    }
                }

                if (hand == PXR_Input.Controller.RightController)
                {
                    if (PXR_Input.IsControllerConnected(PXR_Input.Controller.RightController))
                    {
                        if (!rightControllerState)
                        {
                            controllerType = PXR_Plugin.Controller.UPxr_GetControllerType();
                            RefreshController(PXR_Input.Controller.RightController);
                            rightControllerState = true;
                        }
                    }
                    else
                    {
                        if (rightControllerState)
                        {
                            DestroyLocalController();
                            rightControllerState = false;
                        }
                    }
                }
            }
        }

        private void RefreshController(PXR_Input.Controller hand)
        {
            if (PXR_Input.IsControllerConnected(hand))
            {
                if (systemOrLocal == 0)
                {
                    LoadControllerFromPrefab(hand);
                    if (!loadModelSuccess)
                    {
                        LoadControllerFromSystem((int)hand);
                    }
                }
                else
                {
                    var isControllerExist = false;
                    foreach (Transform t in transform)
                    {
                        if (t.name == modelName)
                        {
                            isControllerExist = true;
                        }
                    }
                    if (!isControllerExist)
                    {
                        LoadControllerFromSystem((int)hand);
                        if (!loadModelSuccess)
                        {
                            LoadControllerFromPrefab(hand);
                        }
                    }
                    else
                    {
                        var currentController = transform.Find(modelName);
                        currentController.gameObject.SetActive(true);
                    }
                }
                PXR_ControllerKeyTips.RefreshTips();
            }
        }

        private void LoadResFromJson()
        {
            string json = PXR_Plugin.System.UPxr_GetObjectOrArray("config.controller", (int)ResUtilsType.TypeObjectArray);
            if (json != null)
            {
                JsonData jdata = JsonMapper.ToObject(json);
                if (controllerType > 0)
                {
                    if (jdata.Count >= controllerType)
                    {
                        curControllerData = jdata[controllerType - 1];
                        if (curControllerData != null)
                        {
                            modelFilePath = (string)curControllerData["base_path"];
                            modelName = (string)curControllerData["model_name"] + "_sys";
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("LoadJsonFromSystem Error");
            }
        }

        private void DestroyLocalController()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
                loadModelSuccess = false;
            }
        }

        private void DestroySysController()
        {
            foreach (Transform t in transform)
            {
                if (t.name == modelName)
                {
                    Destroy(t.gameObject);
                    loadModelSuccess = false;
                }
            }
        }

        private void LoadControllerFromPrefab(PXR_Input.Controller hand)
        {
            switch (controllerType)
            {
                case 3:
                    var g2Go = Instantiate(g2, transform, false);
                    var g2Comp = g2Go.AddComponent<PXR_ControllerKeyEffects>();
                    LoadTexture(g2Comp, g2TexBasePath, true);
                    loadModelSuccess = true;
                    break;
                case 4:
                    var neo2Go = Instantiate(hand == PXR_Input.Controller.LeftController ? neo2L : neo2R, transform, false);
                    var neo2Comp = neo2Go.AddComponent<PXR_ControllerKeyEffects>();
                    neo2Comp.hand = hand;
                    LoadTexture(neo2Comp, neo2TexBasePath, true);
                    loadModelSuccess = true;
                    break;
                case 5:
                    var neo3Go = Instantiate(hand == PXR_Input.Controller.LeftController ? neo3L : neo3R, transform, false);
                    var neo3Comp = neo3Go.AddComponent<PXR_ControllerKeyEffects>();
                    neo3Comp.hand = hand;
                    LoadTexture(neo3Comp, neo3TexBasePath, true);
                    loadModelSuccess = true;
                    break;
                default:
                    loadModelSuccess = false;
                    break;
            }
        }

        private void LoadControllerFromSystem(int id)
        {
            var syscontrollername = controllerType.ToString() + id.ToString() + ".obj";
            var fullFilePath = modelFilePath + syscontrollername;

            if (!File.Exists(fullFilePath))
            {
                Debug.Log("Load Obj From Prefab");
            }
            else
            {
                GameObject go = new GameObject();
                go.name = modelName;
                MeshFilter meshFilter = go.AddComponent<MeshFilter>();
                meshFilter.mesh = PXR_ObjImporter.Instance.ImportFile(fullFilePath);
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.zero;

                MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
                int matID = (int)curControllerData["material_type"];
                meshRenderer.material = matID == 0 ? standardMaterial : mobileMaterial;

                loadModelSuccess = true;
                PXR_ControllerKeyEffects controllerVisual = go.AddComponent<PXR_ControllerKeyEffects>();
                
                controllerVisual.hand = hand;
                LoadTexture(controllerVisual, controllerType.ToString() + id.ToString(), false);
                go.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                go.transform.localScale = new Vector3(-0.01f, 0.01f, 0.01f);
            }
        }


        private void LoadTexture(PXR_ControllerKeyEffects visual, string controllerName, bool fromRes)
        {
            if (fromRes)
            {
                texFormat = "";
                prePath = controllerName;
            }
            else
            {
                texFormat = "." + (string)curControllerData["tex_format"];
                prePath = modelFilePath + controllerName;
            }

            var texturepath = prePath + "_idle" + texFormat;
            visual.textureIdle = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_app" + texFormat;
            visual.textureApp = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_home" + texFormat;
            visual.textureHome = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_touch" + texFormat;
            visual.textureTouchpad = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_volume_down" + texFormat;
            visual.textureVolDown = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_volume_up" + texFormat;
            visual.textureVolUp = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_trigger" + texFormat;
            visual.textureTrigger = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_a" + texFormat;
            visual.textureA = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_b" + texFormat;
            visual.textureB = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_x" + texFormat;
            visual.textureX = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_y" + texFormat;
            visual.textureY = LoadOneTexture(texturepath, fromRes);
            texturepath = prePath + "_grip" + texFormat;
            visual.textureGrip = LoadOneTexture(texturepath, fromRes);
        }

        private Texture2D LoadOneTexture(string filepath, bool fromRes)
        {
            if (fromRes)
            {
                return Resources.Load<Texture2D>(filepath);
            }
            else
            {
                int t_w = (int)curControllerData["tex_width"];
                int t_h = (int)curControllerData["tex_height"];
                var m_tex = new Texture2D(t_w, t_h);
                m_tex.LoadImage(ReadPNG(filepath));
                return m_tex;
            }
        }

        private byte[] ReadPNG(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] binary = new byte[fileStream.Length];
            fileStream.Read(binary, 0, (int)fileStream.Length);
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;
            return binary;
        }
    }
}

