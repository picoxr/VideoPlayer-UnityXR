using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

public class PXR_SQPLoader : MonoBehaviour
{
    public const string RESOURCE_BUNDLE_NAME = "asset_resources";
    public const string EXTERNAL_STORAGE_PATH = "/sdcard/Android/data";
    public const string SCENE_LOAD_DATA_NAME = "SceneLoadData.txt";
    private const string SQP_INDEX_NAME = "PxrSQPIndex";
    private const string CACHE_SCENES_PATH = "cache/scenes";

    private AsyncOperation loadSceneOperation;

    private struct SceneInfo
    {
        public List<string> scenes; 
        public long version;

        public SceneInfo(List<string> sceneList, long currentSceneEpochVersion)
        {
            scenes = sceneList;
            version = currentSceneEpochVersion;
        }
    }

    private string scenePath = "";
    private string sceneLoadDataPath = "";
    private List<AssetBundle> loadedAssetBundles = new List<AssetBundle>();
    private SceneInfo currentSceneInfo;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        string applicationPath = Path.Combine(EXTERNAL_STORAGE_PATH, Application.identifier);
        scenePath = Path.Combine(applicationPath, CACHE_SCENES_PATH);
        sceneLoadDataPath = Path.Combine(scenePath, SCENE_LOAD_DATA_NAME);

        currentSceneInfo = GetSceneInfo();
        if (currentSceneInfo.version != 0 && !string.IsNullOrEmpty(currentSceneInfo.scenes[0]))
        {
            LoadScene(currentSceneInfo);
        }
    }

    public void Update()
    {
    }

    private SceneInfo GetSceneInfo()
    {
        SceneInfo sceneInfo = new SceneInfo();
        try
        {
            StreamReader reader = new StreamReader(sceneLoadDataPath);
            sceneInfo.version = Convert.ToInt64(reader.ReadLine());
            List<string> sceneList = new List<string>();
            while (!reader.EndOfStream)
            {
                sceneList.Add(reader.ReadLine());
            }
            sceneInfo.scenes = sceneList;
        }
        catch
        {
        }

        return sceneInfo;
    }

    private void LoadScene(SceneInfo sceneInfo)
    {
        AssetBundle mainSceneBundle = null;
        // Fetch all files under scene cache path, excluding unnecessary files such as scene metadata file
        string[] bundles = Directory.GetFiles(scenePath, "*_*");
        string mainSceneBundleFileName = "scene_" + sceneInfo.scenes[0].ToLower();
        try
        {
            foreach (string b in bundles)
            {
                var assetBundle = AssetBundle.LoadFromFile(b);

                if (assetBundle != null)
                {
                    Debug.Log("Loading file bundle: " + assetBundle.name == null ? "null" : assetBundle.name);
                    loadedAssetBundles.Add(assetBundle);
                }
                else
                {
                    Debug.LogError("Loading file bundle failed");
                }

                if (assetBundle.name == mainSceneBundleFileName)
                {
                    mainSceneBundle = assetBundle;
                }

                if (assetBundle.name == RESOURCE_BUNDLE_NAME)
                {
                }
            }
        }
        catch (Exception e)
        {
            return;
        }

        if (mainSceneBundle != null)
        {
            string[] scenePaths = mainSceneBundle.GetAllScenePaths();
            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);

            loadSceneOperation = SceneManager.LoadSceneAsync(sceneName);
            loadSceneOperation.completed += LoadSceneOperation_completed;
        }
    }

    private void LoadSceneOperation_completed(AsyncOperation obj)
    {
        StartCoroutine(onCheckNewSceneCoroutine());
    }

    IEnumerator onCheckNewSceneCoroutine()
    {
        SceneInfo newSceneInfo;
        while (true)
        {
            newSceneInfo = GetSceneInfo();
            if (newSceneInfo.version != currentSceneInfo.version)
            {
                Debug.Log("Scene change detected.");

                // Unload all asset bundles
                foreach (var b in loadedAssetBundles)
                {
                    if (b != null)
                    {
                        b.Unload(true);
                    }
                }
                loadedAssetBundles.Clear();

                // Unload all scenes in the hierarchy including main scene and 
                // its dependent additive scenes.
                int activeScenes = SceneManager.sceneCount;
                for (int i = 0; i < activeScenes; i++)
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                }
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    Destroy(go);
                }
                SceneManager.LoadSceneAsync(SQP_INDEX_NAME);
                break;
            }
            yield return new WaitForSeconds(0);
        }
    }

}
