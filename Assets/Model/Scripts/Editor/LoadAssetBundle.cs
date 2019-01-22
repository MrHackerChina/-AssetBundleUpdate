using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LoadAssetBundle {

    #region Package AssetBundle 
    /************************************************************
     * 
     * 
     *     一次性打本地的资源包和服务器要更新的
     *     其实应该是分别打包的  
     *     这样只是做个测试 大家在做时候可以尝试一下别的方式
     * 
     * 
     * ************************************************************/

    private static string assetPath = Application.dataPath + "/Model/Prefab/";
    private static string urlPath = Application.dataPath + "/Model/UrlPrefab/";
    [MenuItem("Tool/LoadAB/Window", false,1)]
    public static void LoadWindow()
    {
        LoadAsset(assetPath,Application.streamingAssetsPath, BuildTarget.StandaloneWindows64);
        LoadAsset(urlPath, Application.dataPath+ "/../StreamingAssets/", BuildTarget.StandaloneWindows64);
    }
    [MenuItem("Tool/LoadAB/IOS", false, 2)]
    public static void LoadIOS()
    {
        LoadAsset(assetPath, Application.streamingAssetsPath, BuildTarget.iOS);
        LoadAsset(urlPath, Application.dataPath + "/../StreamingAssets/", BuildTarget.iOS);
    }
    [MenuItem("Tool/LoadAB/Android", false, 3)]
    public static void LoadAndroid()
    {
        LoadAsset(assetPath, Application.streamingAssetsPath, BuildTarget.Android);
        LoadAsset(urlPath, Application.dataPath + "/../StreamingAssets/", BuildTarget.Android);
    }
    private static void  LoadAsset(string path,string target, BuildTarget buildTarget)
    {
        string[] filePaths = Directory.GetFiles(path);
        string file = string.Empty;
        string dep = string.Empty;
        Dictionary<string, List<string>> assDepens = new Dictionary<string, List<string>>();
        Dictionary<string, uint> assetRef = new Dictionary<string, uint>();
        List<AssetBundleBuild> mult = new List<AssetBundleBuild>();
        for (int i = 0; i < filePaths.Length; i++)
        {
            file = filePaths[i];
            if (file.EndsWith(".meta") || file.Contains(".DS_Store") || file.EndsWith(".manifest")) continue;
            file = "Assets" + file.Replace(Application.dataPath, "");
            string[] depens = AssetDatabase.GetDependencies(file);
            Debug.LogError(depens.Length);
            for (int k = 0; k < depens.Length; k++)
            {
                dep = depens[k];
                if (!assetRef.ContainsKey(dep))
                    assetRef.Add(dep, 0);
                else
                    assetRef[dep]++;
                if (!assDepens.ContainsKey(dep))
                    assDepens[dep] = new List<string>();
                assDepens[dep].Add(file);
            }
        }
        var enu = assDepens.GetEnumerator();
        while(enu.MoveNext())
        {
            if(enu.Current.Key.EndsWith(".cs"))
                continue;
            if(enu.Current.Value.Count>1)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetNames = new string[] { enu.Current.Key };
                build.assetBundleName = Path.GetFileNameWithoutExtension(enu.Current.Key);
                mult.Add(build);
            }
        }
        for (int i = 0; i < filePaths.Length; i++)
        {
            file = "Assets" + filePaths[i].Replace(Application.dataPath, "");
            string name = "prefab/" + Path.GetFileNameWithoutExtension(file);
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetNames = new string[] { file };
            build.assetBundleName = name;
            mult.Add(build);
        }
        if(Directory.Exists(target))
        {
            Directory.Delete(target, true);
        }
        Directory.CreateDirectory(target);
        BuildPipeline.BuildAssetBundles(target, mult.ToArray(), BuildAssetBundleOptions.None, buildTarget);
        AssetDatabase.Refresh();
    }
    #endregion

}
