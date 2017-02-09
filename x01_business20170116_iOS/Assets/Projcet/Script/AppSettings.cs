/******
创建人：NSWell
用途：App 全局设置配置
******/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RequestDataResults;

public class AppSettings : SingletonMono<AppSettings>
{
    public string AppDataPaths { get; private set; }
    public string AppTrackerPaths { get; private set; }
    public string AppScreenShotPaths { get; private set; }
    public string AppAssetbundlePaths { get; private set; }
    public string AppFavoriterPaths { get; private set; }
    public string AppFavoriterThumbnailPaths { get; private set; }
    public string AppVideoPaths { get; private set; }
    public string DBPath { get; private set; }
    public ClientGlobalConfigs clientGlobalConfigs = null;
    private string AndroidStreamAssetPath;
    private string IOSStreamAssetPath;
    private List<string> AppPaths;
    public float CurAppVersion;

    internal List<string> GetTrackerFilesName(string path)
    {
        List<string> trackerFiles = new List<string>();
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileInfo[] filesInfo = directoryInfo.GetFiles();
        foreach (FileInfo info in filesInfo)
        {
            if (info.Extension.EndsWith(".xml"))
            {
                trackerFiles.Add(info.Name);
            }
        }
        return trackerFiles;
    }

    internal List<string> GetDirectoryAllfilesName(string path, string endWith)
    {
        List<string> files = new List<string>();
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileInfo[] filesInfo = directoryInfo.GetFiles();
        foreach (FileInfo info in filesInfo)
        {
            if (info.Extension.EndsWith(endWith))
            {
                files.Add(info.Name);
            }
        }
        return files;
    }


    public string GetCurContentOnLocal()
    {
        FavoriteManager fm = FindObjectOfType<FavoriteManager>();
        BundleType bt = BundleType.Mixed;
        if (fm.favoriteAssets.Count > 0)
        {
            string name = Manager.Instance.GetScaneManager.GetCurContentName;
            if (fm.favoriteAssets.ContainsKey(name))
                bt = fm.favoriteAssets[name].GetBundleType();
        }
        else if (Manager.Instance.GetScaneManager.assets.Count > 0)
        {
            bt = Manager.Instance.GetScaneManager.assets[0].GetBundleType();
        }
        else if (Manager.Instance.GetAllContentsManager.mClickElement != null)
        {
            int id = Manager.Instance.GetAllContentsManager.mClickElement.guid;
            bt = Manager.Instance.GetAllContentsManager.contentAssets[id].GetBundleType();
        }
        string mPath = AppFavoriterPaths + Manager.Instance.GetScaneManager.GetCurContentName;

        switch (bt)
        {
            case BundleType.Model:
                mPath += ".assetbundle";
                break;
            case BundleType.Video:
                mPath += ".mp4";
                break;
            case BundleType.Mixed:
                mPath += ".assetbundle";
                break;
        }

        return mPath;
    }
    public string GetCurContentThumbilOnLocal()
    {
        string mPath = AppSettings.Instance.AppFavoriterThumbnailPaths +
               Manager.Instance.GetScaneManager.GetCurContentName + ".jpg";
        return mPath;
    }

    public long GetFavoriteCacheSize(DirectoryInfo directoryInfo)
    {
        long size = 0;
        FileInfo[] fileInfo = directoryInfo.GetFiles();

        foreach (FileInfo info in fileInfo)
        {
            size += info.Length;
        }
        DirectoryInfo[] dis = directoryInfo.GetDirectories();
        if (dis.Length <= 0) return size;
        foreach (DirectoryInfo info in dis)
        {
            foreach (FileInfo file in info.GetFiles())
            {
                size += file.Length;
            }
        }
        return size;
    }
    private void SetPlatformPaths()
    {
        AppPaths = new List<string>();
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                DBPath = "jar:file://" + Application.dataPath + "!/assets/";
                string tempPath = Application.temporaryCachePath.Replace("cache", "");
                AppDataPaths = tempPath + clientGlobalConfigs.clientConfigs.projectName;
                break;
            case RuntimePlatform.IPhonePlayer:
                AppDataPaths = Application.persistentDataPath + "/";
                DBPath = "file://"+Application.dataPath + "/Raw/";
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
                AppDataPaths = Application.dataPath.Replace("Assets", "") + clientGlobalConfigs.clientConfigs.projectName;
                DBPath = "file://" + Application.streamingAssetsPath + "/";
                break;
        }

        AppTrackerPaths = AppDataPaths + "/Trackers/";
        AppScreenShotPaths = AppDataPaths + "/ScreenShots/";
        AppAssetbundlePaths = AppDataPaths + "/Assetsbundle/";
        AppFavoriterPaths = AppDataPaths + "/Favoriters/";
        AppFavoriterThumbnailPaths = AppFavoriterPaths + "Thumbnails/";
        AppVideoPaths = AppDataPaths + "/RecordingVideos/";

        AppPaths.Add(AppDataPaths);
        AppPaths.Add(AppTrackerPaths);
        AppPaths.Add(AppScreenShotPaths);
        AppPaths.Add(AppAssetbundlePaths);
        AppPaths.Add(AppFavoriterPaths);
        AppPaths.Add(AppFavoriterThumbnailPaths);
        AppPaths.Add(AppVideoPaths);
        CreateDirectories();

    }
    private void CreateDirectories()
    {
        foreach (string path in AppPaths)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        AppPaths.Clear();
        AppPaths = null;
    }
    public void Awake()
    {
        PlayerPrefs.SetFloat("CurAppVersion", CurAppVersion);
        SetPlatformPaths();
        Debug.Log(AppDataPaths);
      
        Debug.Log(FileOpeartion.GetFileOpeartion().CheckingFile(DBPath.Replace("file://","") + "weacwDB.db"));
        Debug.Log(FileOpeartion.GetFileOpeartion().CheckingFile(AppDataPaths + "/weacwDB.db"));
        if (!FileOpeartion.GetFileOpeartion().CheckingFile(AppDataPaths + "/weacwDB.db"))
            StartCoroutine(CopySqlitedbToSDcard(DBPath + "weacwDB.db", AppDataPaths + "/weacwDB.db"));
        if (!FileOpeartion.GetFileOpeartion().CheckingFile(AppDataPaths + "/Icon.jpg"))
            StartCoroutine(CopySqlitedbToSDcard(DBPath + "Icon.jpg", AppDataPaths + "/Icon.jpg"));

        GeneralWWW.Instance.GeneralInit();
        Manager.Instance.ManagerInit(); 
    }

    private IEnumerator CopySqlitedbToSDcard(string path, string targetPath)
    {
        WWW www = new WWW(path);
        yield return www;
        while (!www.isDone) { }
        File.WriteAllBytes(targetPath, www.bytes);
        while (!FileOpeartion.GetFileOpeartion().CheckingFile(AppDataPaths + "/weacwDB.db"))
        {
            yield return null;
        }
        Debug.Log(FileOpeartion.GetFileOpeartion().CheckingFile(AppDataPaths + "/weacwDB.db"));
        StartCoroutine(WaitScendsToLoadFavorite());
    }

    private IEnumerator WaitScendsToLoadFavorite()
    {
        yield return new WaitForSeconds(1);
        FindObjectOfType<FavoriteManager>().OnFavoriteDisplay();

    }
}
