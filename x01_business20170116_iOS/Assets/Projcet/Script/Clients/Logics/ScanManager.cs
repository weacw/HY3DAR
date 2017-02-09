

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 *Description:扫描逻辑管理器
*/

using System;
using System.Collections.Generic;
using RequestDataResults;
using UnityEngine;
using Vuforia;

public enum TrackerContentStatus
{
    OnTracker,
    LoseTracker
};

public enum TRACKSENABLESTATUS
{
    STOPOTHER,
    STOPALL,
    PLAYOTHER,
    PLAYALL
};
public class ScanManager : MonoBehaviour
{
    //获取当前已经识别到的对象容器
    public TrackableEvent GetTrackableTarget;// { get; private set; }

    //用于获取当前已经呈现出来的内容
    public GameObject GetContent { get; private set; }

    //用于获取当前已经识别到对象的名字
    public string GetCurContentName { get; private set; }

    //用于查询数据库
    public DBQuery_Assets_OnScanning GetDbQueryAssetsOnScanning { get; private set; }

    //用于跟踪ARCamera
    public Transform Camera2DPos { get; private set; }

    //当前场景中存在的所有识别对象
    private List<TrackableEvent> trackableEvents = new List<TrackableEvent>();

    public GameObject videoPrefab;
    //用于存储服务器返回数据映射类
    [SerializeField]
    internal List<Assets> assets = new List<Assets>();



    public TrackerContentStatus ContentStatus { get; private set; }
    //初始化必备对象
    public void Initi()
    {
        //获取相机位置
        Camera2DPos = GameObject.FindGameObjectWithTag("ARCamera2DPOS").transform;
    }

    //加载全部的tracker，以便管理
    public void FindAllTracker()
    {
        trackableEvents.AddRange(FindObjectsOfType<TrackableEvent>());
    }

    //消除所有对象的识别，除当前已经识别到对象外
    public void CtrlOtherTracker(TrackableEvent trackable, TRACKSENABLESTATUS tracksenablestatus)
    {
        for (int i = 0; i < trackableEvents.Count; i++)
        {
            switch (tracksenablestatus)
            {
                case TRACKSENABLESTATUS.STOPOTHER:
                    if (trackableEvents[i].Equals(trackable))
                    {
                        trackable.GetTrackableBehaviour.RegisterTrackableEventHandler(trackableEvents[i]);
                        trackable.enabled = true;
                        trackable.GetTrackableBehaviour.enabled = true;
                        continue;
                    }
                    trackableEvents[i].GetTrackableBehaviour.UnregisterTrackableEventHandler(trackableEvents[i]);
                    trackableEvents[i].enabled = false;
                    trackableEvents[i].GetTrackableBehaviour.enabled = false;
                    break;
                case TRACKSENABLESTATUS.STOPALL:
                    trackableEvents[i].GetTrackableBehaviour.UnregisterTrackableEventHandler(trackableEvents[i]);
                    trackableEvents[i].enabled = false;
                    trackableEvents[i].GetTrackableBehaviour.enabled = false;
                    break;
                case TRACKSENABLESTATUS.PLAYOTHER:
                    if (trackableEvents[i].Equals(trackable) || trackable == null) continue;
                    trackableEvents[i].GetTrackableBehaviour.RegisterTrackableEventHandler(trackableEvents[i]);
                    trackableEvents[i].enabled = true;
                    trackableEvents[i].GetTrackableBehaviour.enabled = true;
                    break;
                case TRACKSENABLESTATUS.PLAYALL:
                    trackableEvents[i].GetTrackableBehaviour.RegisterTrackableEventHandler(trackableEvents[i]);
                    trackableEvents[i].enabled = true;
                    trackableEvents[i].GetTrackableBehaviour.enabled = true;
                    break;
            }

        }
    }

    //等待数据接收完毕后
    internal void BeginToDownloadAbs()
    {
        switch (assets[0].GetBundleType())
        {
            case BundleType.Model:

                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                        AppSettings.Instance.clientGlobalConfigs.serverCofing.serverbundlesurl = assets[0].androidUrl;
                        break;
                    case RuntimePlatform.Android:
                        AppSettings.Instance.clientGlobalConfigs.serverCofing.serverbundlesurl = assets[0].androidUrl;
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        AppSettings.Instance.clientGlobalConfigs.serverCofing.serverbundlesurl = assets[0].iosUrl;
                        break;
                }

                break;
            case BundleType.Video:
                AppSettings.Instance.clientGlobalConfigs.serverCofing.serverbundlesurl = assets[0].videourl;
                break;
            case BundleType.Mixed:
                break;
        }
        DownloadAbsNCreate.Instance.DownLoad(AppSettings.Instance.clientGlobalConfigs.serverCofing.serverbundlesurl);

    }

    //清除当前已经呈现出来的内容
    public void RemoveTrackable()
    {
        if (!GetTrackableTarget) return;
        if (FindObjectOfType<ShowerSetting>() != null)
            if (FindObjectOfType<ShowerSetting>().isVideoBundle)
                FileOpeartion.GetFileOpeartion().FileDelect(AppSettings.Instance.AppAssetbundlePaths + "temp.mp4");

        assets.Clear();
        FindObjectOfType<FavoriteManager>().mAsset = null;
        FindObjectOfType<FavoriteManager>().thumbnail = null;
        Manager.Instance.GetUIManager.removeAtFavoriteBtn.gameObject.SetActive(false);
        Manager.Instance.GetUIManager.addToFavoriteBtn.gameObject.SetActive(true);
        CtrlOtherTracker(GetTrackableTarget, TRACKSENABLESTATUS.PLAYOTHER);
        GetTrackableTarget.transform.SetParent(null);
        GetTrackableTarget.transform.rotation = Quaternion.identity;
        GetTrackableTarget = null;
        if (!GetContent) return;
        Destroy(GetContent);
        GetCurContentName = null;
        Resources.UnloadUnusedAssets();
        Manager.Instance.GetAllContentsManager.mClickElement = null;
    }

    public void SwapCameraDiction()
    {
        CameraDevice.Instance.Stop();
        CameraDevice.Instance.Deinit();
        var config = VuforiaRenderer.Instance.GetVideoBackgroundConfig();
        switch (CameraDevice.Instance.GetCameraDirection())
        {
            case CameraDevice.CameraDirection.CAMERA_DEFAULT:
            case CameraDevice.CameraDirection.CAMERA_BACK:
                CameraDevice.Instance.Init(CameraDevice.CameraDirection.CAMERA_FRONT);
                config.reflection = VuforiaRenderer.VideoBackgroundReflection.ON;
                VuforiaRenderer.Instance.SetVideoBackgroundConfig(config);
                CameraDevice.Instance.Start();
                break;
            case CameraDevice.CameraDirection.CAMERA_FRONT:
                CameraDevice.Instance.Init(CameraDevice.CameraDirection.CAMERA_DEFAULT);
                config.reflection = VuforiaRenderer.VideoBackgroundReflection.OFF;
                VuforiaRenderer.Instance.SetVideoBackgroundConfig(config);
                CameraDevice.Instance.Start();
                break;
        }

        //Camera2DPos.localRotation = Quaternion.Euler(CameraDevice.Instance.GetCameraDirection() ==
        //                               CameraDevice.CameraDirection.CAMERA_FRONT
        //    ? new Vector3(0, 0, 180)
        //    : new Vector3(0, 0, 0));
        //Camera2DPos.localScale = CameraDevice.Instance.GetCameraDirection() ==
        //                               CameraDevice.CameraDirection.CAMERA_FRONT
        //    ? new Vector3(-1, 1, 1)
        //    : new Vector3(1, 1, 1);
    }

    //当前呈现出来的内容
    public void SetContent(GameObject go)
    {
        GetContent = go;
    }

    //识别识别图后
    internal void OnFoundEventMethod(string name, bool status, TrackableEvent trackable)
    {
        bool exists = false;

        //隐藏扫描中的UI界面        
        Manager.Instance.GetUIManager.scanningTipUI.SetActive(false);
        ContentStatus = TrackerContentStatus.OnTracker;
        if (GetCurContentName == null)
        {
            //获取当前识别到的对象名字        
            GetCurContentName = name;
            //本地查询，是否存在sdcard中
            exists = Manager.Instance.GetUIManager.CurContentIsAddToFavorite();
        }

        if (GetTrackableTarget == null)
        {
            //当前呈现的追踪对象
            GetTrackableTarget = trackable;

            if (!exists)
            {
                //TODO:优化建议->可用另外一个操作类封装此操作
                //发送请求至服务器
                if (GetDbQueryAssetsOnScanning == null)
                    GetDbQueryAssetsOnScanning = new DBQuery_Assets_OnScanning();
                GetDbQueryAssetsOnScanning.SendMysqlQuery(name);
            }
            else
            {
                Debug.Log(AppSettings.Instance.GetCurContentOnLocal());
                //从本地加载
                DownloadAbsNCreate.Instance.DownLoad("file://" + AppSettings.Instance.GetCurContentOnLocal());
            }
            //制空父物体            
            GetTrackableTarget.transform.SetParent(null);
            //消除对其他识别图的识别
            CtrlOtherTracker(GetTrackableTarget, TRACKSENABLESTATUS.STOPOTHER);
        }
        else
        {
            //TODO:内容已经显示，直接呈现。
            //当前扫描的对象与识别到的对象是否同一个
            string curTrackerName =
                GetTrackableTarget.GetComponent<TrackableEvent>().GetTrackableBehaviour.TrackableName;
            if (String.Compare(trackable.GetTrackableBehaviour.TrackableName, curTrackerName, StringComparison.Ordinal) != 0) return;
            if (FindObjectOfType<ShowerSetting>() != null)
                FindObjectOfType<ShowerSetting>().FixedAngleByFinding();

            GetTrackableTarget.transform.SetParent(null);
            GetTrackableTarget.transform.localPosition = Vector3.zero;
        }
    }

    //丢失识别图后
    internal void OnLostEventMethod(string name, bool status)
    {
        if (!Camera2DPos || !GetTrackableTarget) return;
        if (FindObjectOfType<ShowerSetting>() != null)
            FindObjectOfType<ShowerSetting>().FixedAngleByLosting();

        if (GetCurContentName == null)
        {
            //获取当前识别到的对象名字        
            GetCurContentName = name;
            Manager.Instance.GetUIManager.CurContentIsAddToFavorite();
        }

        ContentStatus = TrackerContentStatus.LoseTracker;
        GetTrackableTarget.transform.SetParent(Camera2DPos);

        if (GetContent != null && GetContent.GetComponent<ShowerSetting>() != null)
            GetTrackableTarget.transform.localPosition = GetContent.GetComponent<ShowerSetting>().on2DDist;
        else
            //TODO:未来将从内容上动态读取设置ß
            GetTrackableTarget.transform.localPosition = new Vector3(0, 0, 1);

        GetTrackableTarget.transform.LookAt(Camera2DPos);
        Vector3 local = GetTrackableTarget.transform.localEulerAngles;
        local.z = 0;
        local.y = 0;
        GetTrackableTarget.transform.localRotation = Quaternion.Euler(local);


    }

    internal void FromFavoriteCreated(string name, bool status)
    {
        if (Manager.Instance.IsOffLineWorking)
            GetCurContentName = null;
        GetTrackableTarget = GameObject.Find(name).GetComponent<TrackableEvent>();
        CtrlOtherTracker(GetTrackableTarget, TRACKSENABLESTATUS.STOPOTHER);
        OnLostEventMethod(name, status);
    }
}