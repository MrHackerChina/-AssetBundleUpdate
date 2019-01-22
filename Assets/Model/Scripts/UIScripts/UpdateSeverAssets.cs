using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public class UpdateSeverAssets : SingletonMono<UpdateSeverAssets> {

    /// <summary>
    /// sever url
    /// </summary>
    private string urlPath = string.Empty;

    private void Awake()
    {
        urlPath = Application.dataPath + "/../StreamingAssets";
    }
    public void Init()
    {
        ComparisonAssets();
    }
    private void ComparisonAssets()
    {
        List<string> urlAssets = new List<string>();
        List<string> natAssets = new List<string>();
        Tools.RecursiveDir(urlPath, ref urlAssets);
        Tools.RecursiveDir(Application.streamingAssetsPath, ref natAssets);
        List<string> updateAssets = new List<string>();
        string url = string.Empty;
        string mdNat = string.Empty;
        string mdUrl = string.Empty;
        for (int i = 0; i < urlAssets.Count; i++)
        {
            url = Application.streamingAssetsPath +  urlAssets[i].Replace(urlPath, "");
            if(natAssets.Contains(url))
            {
                mdNat = Tools.MD5File(url);
                mdUrl = Tools.MD5File(urlAssets[i]);
                if(string.Equals(mdNat,mdUrl))
                {
                    natAssets.Remove(url);
                    continue;
                }
                updateAssets.Add(urlAssets[i]);
            }
            else
            {
                updateAssets.Add(urlAssets[i]);
            }

        }
        // 删除本地不需要的文件
        for (int i = 0; i < natAssets.Count; i++)
        {
            if (File.Exists(natAssets[i]))
            {
                File.Delete(natAssets[i]);
            }
        }
        // 更新
        for (int i = 0; i < updateAssets.Count; i++)
        {
            url = Application.streamingAssetsPath + urlAssets[i].Replace(urlPath, "");
            // 这里需要去服务器下载资源（本地的话直接负就行）
            // 这里只提供一个思路
            File.Copy(updateAssets[i], url, true);
        }
       
    }
	
}



public class Tools
{
    public static void RecursiveDir(string path, ref List<string> allFilePath, bool isFirstRun = true)
    {
        if (isFirstRun && allFilePath.Count > 0)
        {
            allFilePath.TrimExcess();
            allFilePath.Clear();
        }
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);

        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta") || ext.Equals(".manifest")) continue;

            allFilePath.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            RecursiveDir(dir, ref allFilePath, false);
        }
    }
    public static string MD5File(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }
}

