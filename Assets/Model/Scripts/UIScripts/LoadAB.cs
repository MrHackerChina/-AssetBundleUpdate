using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


#region 单例
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;
    /// <summary>
    /// 线程锁
    /// </summary>
    static object locker = new object();

    public static T Instance
    {
        get
        {
            lock (locker)
            {
                if (_instance == null)
                {

                    GameObject single = null;
                    if (single == null)
                    {
                        single = new GameObject("SingletonMono");
                    }
                    DontDestroyOnLoad(single);
                    _instance = single.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}
#endregion


public class LoadAB : SingletonMono<LoadAB>
{

    private Transform uiParent;
    private Transform UIParent()
    {

        if (uiParent == null)
            uiParent = GameObject.Find("Canvas").transform;
        return uiParent;
    }
    private GameObject loadSaver;
    private AssetBundleManifest manifest;
    private AssetBundle mainfestAB;
    private Dictionary<string, AssetBundle> AlreadyAsset = new Dictionary<string, AssetBundle>();
    private Dictionary<string, GameObject> DicUIObj = new Dictionary<string, GameObject>();

    private IEnumerator LoadMainfest()
    {
        WWW www = new WWW("file://" + Application.streamingAssetsPath + "/StreamingAssets");
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            yield break;
        }
        mainfestAB = www.assetBundle;
        manifest = (AssetBundleManifest)mainfestAB.LoadAsset("AssetBundleManifest");
        www.Dispose();
        mainfestAB.Unload(false);
    }


    public IEnumerator LoadUIName(string name, Action<GameObject> callBack = null)
    {
        GameObject go = null;
        if (DicUIObj.ContainsKey(name))
        {
            go = DicUIObj[name];
            if(callBack!=null)
            {
                callBack(go);
            }
            yield break;
        }
        if (manifest == null)
        {
            yield return StartCoroutine(LoadMainfest());
        }
        string[] despensecies = manifest.GetAllDependencies("prefab/" + name.ToLower());
        WWW www = null;
        for (int i = 0; i < despensecies.Length; i++)
        {
            www = new WWW("file://" + Application.streamingAssetsPath + "/" + despensecies[i]);
            if (AlreadyAsset.ContainsKey(despensecies[i]))
            {
                continue;
            }
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                if (callBack != null)
                {
                    callBack(go);
                }
                yield break;
            }
            AssetBundle desAB = www.assetBundle;
            if (!AlreadyAsset.ContainsKey(despensecies[i]))
                AlreadyAsset.Add(despensecies[i], desAB);
        }
        www = new WWW("file://" + Application.streamingAssetsPath + "/prefab/" + name.ToLower());
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            if (callBack != null)
            {
                callBack(go);
            }
            yield break;
        }
        AssetBundle ab = www.assetBundle;
        if(!AlreadyAsset.ContainsKey(name))
        {
            AlreadyAsset.Add(name, ab);
        }
        else
        {
            AlreadyAsset[name] = ab;
        }
        AssetBundleRequest abs = ab.LoadAllAssetsAsync();
        yield return abs;
        var model = abs.allAssets;
        for (int i = 0; i < model.Length; i++)
        {
            go = Instantiate(model[i]) as GameObject;
            go.transform.SetParent(UIParent(), false);
            go.name = name;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.rotation = Quaternion.identity;
            if (!DicUIObj.ContainsKey(name))
                DicUIObj.Add(name, go);
            else
                DicUIObj[name] = go;
        }
        if (callBack != null)
        {
            callBack(go);
        }
    }

    
    public void LoadAsset()
    {
        if(loadSaver==null)
        {
            GameObject load = (GameObject)Resources.Load("loadsever");
            loadSaver = Instantiate(load,UIParent(),false);
            loadSaver.name = "loadseve";
            loadSaver.transform.localPosition = Vector3.zero;
            loadSaver.transform.localScale = Vector3.one;
            loadSaver.transform.rotation = Quaternion.identity;
        }
        loadSaver.SetActive(true);
        ClearObj();
        UnloadAsset();
    }

    private void ClearObj()
    {
        List<GameObject> obj = new List<GameObject>();
        obj.AddRange(DicUIObj.Values);
        for (int i = obj.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(obj[i]);
        }
        DicUIObj.Clear();
    }
    private void UnloadAsset()
    {
        List<AssetBundle> abs = new List<AssetBundle>();
        abs.AddRange(AlreadyAsset.Values);
        for (int i = abs.Count-1; i >= 0; i--)
        {
            abs[i].Unload(true);
        }
        AlreadyAsset.Clear();
        manifest = null;
        Resources.UnloadUnusedAssets();
    }


}
