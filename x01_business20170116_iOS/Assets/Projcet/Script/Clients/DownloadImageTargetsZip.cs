/******
创建人：NSWell
用途：下载识别图
******/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Vuforia;

public delegate void OnDownloading(float progress);
public class DownloadImageTargetsZip : MonoBehaviour
{
    public OnDownloading onDownloading;
    public string serverPath;
    //public string imageTargetZipVersion;
    public string imageTargetZipName;
    private IEnumerator downloadITEnumerator;
    public IEnumerator checking;
    private void OnEnable()
    {
        GeneralWWW.Instance.GetTrackerDownloader.OnDownloadOver += OnDownloadTargetEndEvent;
    }

    public void Download()
    {
        string path = AppSettings.Instance.clientGlobalConfigs.serverCofing.serverTrackersurl;

        if (downloadITEnumerator != null)
        {
            GeneralWWW.Instance.StopWWW(downloadITEnumerator);
            downloadITEnumerator = null;
        }
        downloadITEnumerator = GeneralWWW.Instance.GetTrackerDownloader.WWWDownloader(Manager.Instance.OnNetworkError, path);
        if (checking != null)
        {
            GeneralWWW.Instance.StopWWW(checking);
            checking = null;
        }
        checking = GeneralWWW.Instance.GetTrackerDownloader.CheckingTimeOut(Manager.Instance.OnNetworkError);
        GeneralWWW.Instance.OpenCoroutine(downloadITEnumerator);
        GeneralWWW.Instance.OpenCheckingCoroutine(checking);
    }

    private bool OnDownloadTargetEndEvent(byte[] bytes)
    {

        GeneralWWW.Instance.StopWWW(checking);
        if (bytes.Length <= 0) return false;
        if (!Directory.Exists(AppSettings.Instance.AppTrackerPaths))
            Directory.CreateDirectory(AppSettings.Instance.AppTrackerPaths);

        if (File.Exists(AppSettings.Instance.AppTrackerPaths + imageTargetZipName))
            File.Delete(AppSettings.Instance.AppTrackerPaths + imageTargetZipName);
        Stream stream = null;
        stream = File.Create(AppSettings.Instance.AppTrackerPaths + imageTargetZipName);
        Debug.Log("Creating file!!!");
        stream.Write(bytes, 0, bytes.Length);
        stream.Close();
        stream.Dispose();


        GetComponent<CallNativeFun>().Decompressionzip(AppSettings.Instance.AppTrackerPaths + imageTargetZipName, AppSettings.Instance.AppTrackerPaths);

        StartCoroutine(WaitToDeleteFile());
        return true;
    }

    private IEnumerator WaitToDeleteFile()
    {
        yield return new WaitForSeconds(1.25f);
        if (File.Exists(AppSettings.Instance.AppTrackerPaths + imageTargetZipName))
            File.Delete(AppSettings.Instance.AppTrackerPaths + imageTargetZipName);
        if (null == Manager.Instance.GetAtRuntimeLoadDBs.onLoadSuccess) yield break;
        List<string> filesName = AppSettings.Instance.GetTrackerFilesName(AppSettings.Instance.AppTrackerPaths);

        for (int i = 0; i < filesName.Count; i++)
        {
            Manager.Instance.GetAtRuntimeLoadDBs.onLoadSuccess.Invoke(
                AppSettings.Instance.AppTrackerPaths + filesName[i],
                VuforiaUnity.StorageType.STORAGE_ABSOLUTE);
        }
        //加载全部识别图，对识别图信息进行重新更新
        Manager.Instance.GetAtRuntimeLoadDBs.UpdateImageTarget();
        yield return new WaitForSeconds(1);
        Manager.Instance.GetUIManager.HideTrackersProgress();
    }
}
