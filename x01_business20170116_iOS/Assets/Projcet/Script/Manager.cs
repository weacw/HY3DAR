

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * APP全局管理器
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vuforia;

public class Manager : SingletonMono<Manager>
{
    #region gesture ctrl class
    private RotateControl rotateControl;
    private MovementControl movementControl;
    private PinchControl pinchControl;
    private RestControl restControl;
    #endregion


    public const string UNABLERESOLVEHOST = "No address associated with hostname";
    public const string NOFOUND = "not found";
    public const string TIMEOUT = "timed out";

    public bool HappenError { get; private set; }
    public bool IsOffLineWorking { get; private set; }
    public ShareModuls GetShareModuls { get; private set; }

    public ScreenshotModul GetScreenshotModul { get; private set; }
    public RecordingModul GetRecordingModul { get; private set; }

    public ScanManager GetScaneManager { get; private set; }
    public UserInterfaceManager GetUIManager { get; private set; }
    public DownloadImageTargetsZip GetTargetsZip { get; private set; }
    public AtRuntimeLoadDBs GetAtRuntimeLoadDBs { get; private set; }
    public AllContentsManager GetAllContentsManager { get; private set; }
    public AppVersionContrl GetAppVersionCtrl { get; private set; }
    private float pixelratio;
    public GestureParater mTarget;
    private Vector2 firstPoint, secondPoint;
    private int quitAmount = 0;
    private float mTimeSinceLastTap;
    private bool isInited;
    public void ManagerInit()
    {
        InitSubsystemsModuls();
        InitGestureModuls();
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
        isInited = true;
    }


    public void OnApplicationPause()
    {
        StopAllCoroutines();
        GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    public void OnRecordStarted()
    {
        Texture2D t2d = new Texture2D(240, 320, TextureFormat.RGBA32, false);
        t2d.wrapMode = TextureWrapMode.Clamp;
        Everyplay.SetThumbnailTargetTexture(t2d);
    }
    public void UploadVideoToServer()
    {
        if (GetRecordingModul == null)
            return;
        StartCoroutine(GetRecordingModul.UploadToServer());
    }

    private void InitSubsystemsDelegat()
    {
        GeneralWWW.Instance.GetAbsDownloader.onAbsDownloadBegin = GetUIManager.ShowAbsProgress;
        GeneralWWW.Instance.GetAbsDownloader.onAbsDownloadEnd = GetUIManager.HideAbsProgress;
        GeneralWWW.Instance.GetTrackerDownloader.OnDonloadTrackersBegin = GetUIManager.ShowTrackersProgress;
        GetTargetsZip.Download();

        GetScreenshotModul.screenshotModulsOpeartionEvent += GetUIManager.OnScreenshotting;
        GetScreenshotModul.screenshotModulsOperatedEvent += GetUIManager.OnScreenshoted;

        //GetRecordingModul.recordModulsOpeartionEvent += GetUIManager.StartRecording;
        GetRecordingModul.recordModulsOpeartedEvent += GetUIManager.HideRecording;
    }

    private void InitSubsystemsModuls()
    {
        if (GetScaneManager == null) GetScreenshotModul = new ScreenshotModul();
        if (GetRecordingModul == null) GetRecordingModul = new RecordingModul();
        if (GetShareModuls == null) GetShareModuls = new ShareModuls();
        GetAppVersionCtrl = new AppVersionContrl();
        GetAppVersionCtrl.Start();

        //所有内容模块管理器
        GetAllContentsManager = FindObjectOfType<AllContentsManager>() ?? gameObject.AddComponent<AllContentsManager>();

        //扫描模块管理器
        GetScaneManager = FindObjectOfType<ScanManager>() ?? gameObject.AddComponent<ScanManager>();
        GetScaneManager.Initi();

        //用户交互图形界面管理器
        GetUIManager = FindObjectOfType<UserInterfaceManager>() ?? gameObject.AddComponent<UserInterfaceManager>();

        //识别对象下载管理
        GetTargetsZip = FindObjectOfType<DownloadImageTargetsZip>() ??
                        gameObject.AddComponent<DownloadImageTargetsZip>();
        InitSubsystemsDelegat();


        //运行时加载识别对象数据管理器
        GetAtRuntimeLoadDBs = FindObjectOfType<AtRuntimeLoadDBs>() ?? gameObject.AddComponent<AtRuntimeLoadDBs>();
        GetAtRuntimeLoadDBs.Init();

        FindObjectOfType<AllContentsManager>().Init();
        FindObjectOfType<AllContentsManager>().GetDbQueryAssetsOnAllContents.SendMysqlQuery(null);

    }


    public void Update()
    {
        if (!CheckGUIRaycastObjects() && Input.GetMouseButtonDown(0))
        {
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }     
        if (!CheckGUIRaycastObjects())
            ModelGestureCtrlModuls();
        
        AppQuitCheck();
        DrawProgress();

        if (GetRecordingModul != null && GetRecordingModul.IsStartRecord)
        {
            GetUIManager.Recording(GetRecordingModul.CurTime);
        }
        if (GetUIManager.IsAddedToFavorite)
        {
            GetUIManager.menuBtn.interactable = false;
            GetUIManager.menuBtn.interactable = true;
            GetUIManager.closeCurTargetUI.GetComponent<Button>().interactable = true;
            GetUIManager.AddedToFavorite();
        }
    }


    private bool CheckGUIRaycastObjects()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.pressPosition = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    private void AppQuitCheck()
    {
        if (Application.platform != RuntimePlatform.Android) return;
        switch (quitAmount)
        {
            case 1:
                mTimeSinceLastTap += Time.deltaTime;
                if (mTimeSinceLastTap > 0.5f)
                {
                    mTimeSinceLastTap = 0;
                    quitAmount = 0;
                }
                break;
            case 2:
                mTimeSinceLastTap = 0;
                quitAmount = 0;
                Application.Quit();
                break;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            quitAmount++;
            GetComponent<CallNativeFun>().ShowNativeToast("再按一次返回键退出应用");
        }
    }
    public void AppQuit() { Application.Quit(); }

    public void ReDownload()
    {
        GetTargetsZip.Download();
    }
    private void DrawProgress()
    {
        if (!isInited) return;
        if (GetUIManager.GetProgressAbsStatus && !HappenError && null != GeneralWWW.Instance.GetAbsDownloader.AbsWww)
            GetUIManager.UpdateAbsProgressData(GeneralWWW.Instance.GetAbsDownloader.AbsWww.progress);

        if (GetUIManager.GetProgressTargetsStatus && !HappenError && null != GeneralWWW.Instance.GetTrackerDownloader.TrackerWww)
            GetUIManager.UpdateTrackersProgressData(GeneralWWW.Instance.GetTrackerDownloader.TrackerWww.progress);
    }


    //网络错误回调
    public void OnNetworkError(string error_Msg, SenderType sender)
    {
        Debug.Log(sender.ToString()+"\n\n\n\n\n\n\n\n");
        StopAllCoroutines();
        HappenError = true;
        switch (sender)
        {
            case SenderType.Text_Tips:
                Manager.Instance.GetScaneManager.RemoveTrackable();
                if (!string.IsNullOrEmpty(error_Msg))
                {
                    string mError = ErrorConvert(error_Msg);
                    GetComponent<CallNativeFun>().ShowNativeToast(mError);
                }
                break;
            case SenderType.Img_Tips:
                break;
            case SenderType.FLOAT_WINDOW:
#if UNITY_EDITOR
                GetUIManager.FloatWindowTips();
#endif
                #if UNITY_ANDROID
                FindObjectOfType<CallNativeFun>().ShowAlertDialog("连接错误", "网络不给力，请检查网络设置。", "忽略", "重试", "OffLineWorking", "ReDownload");
                #endif
                #if UNITY_IOS || UNITY_IPHONE
                FindObjectOfType<CallNativeFun>().ShowAlertDialog("连接错误", "网络不给力，请检查网络设置。", "忽略", "", "", "");
                #endif
                break;
        }
        if (!HappenError) return;
        GetUIManager.scanningTipUI.SetActive(true);
        GetUIManager.menuBtn.interactable = true;
        GetUIManager.HideAbsProgress();
        HappenError = false;
    }
    //网络错误消息解析至中文
    private string ErrorConvert(string error_Msg)
    {
        string mError = null;
        if (error_Msg.Contains(UNABLERESOLVEHOST.ToLower()))
            mError = "无法连接服务器，请确保网络畅通";
        else if (error_Msg.Contains(TIMEOUT.ToLower()))
            mError = "连接超时，";
        else if (error_Msg.Contains("404") || error_Msg.Contains(NOFOUND.ToLower()))
            mError = "找不到此资源";
        else
            mError = "连接超时";
        return mError;
    }

    private void OffLineWorking()
    {
        IsOffLineWorking = true;
        List<string> filesName = AppSettings.Instance.GetTrackerFilesName(AppSettings.Instance.AppTrackerPaths);
        for (int i = 0; i < filesName.Count; i++)
        {
            GetAtRuntimeLoadDBs.onLoadSuccess.Invoke(
                AppSettings.Instance.AppTrackerPaths + filesName[i],
                VuforiaUnity.StorageType.STORAGE_ABSOLUTE);
        }
        //加载全部识别图，对识别图信息进行重新更新
        GetAtRuntimeLoadDBs.UpdateImageTarget();
        GetUIManager.HideTrackersProgress();
    }
    #region Gesture Moduls

    public void RestCtrl()
    {
        restControl.ControlUpdate();
    }



    //初始化控制模块
    private void InitGestureModuls()
    {
        float height = Screen.height;
        float width = Screen.width;
        float proportion = width / height;
        height *= proportion;
        width *= proportion;
        float portionH = height / 2;
        float portionW = width / 2;
        pixelratio = (portionH - portionW) + 40;
        rotateControl = new RotateControl(60);

        movementControl = new MovementControl(0.5f);
        pinchControl = new PinchControl(1, 5);
        restControl = new RestControl();
        //tapHandlerCtrl = new TapHandlerCtrl();
    }
    //设置控制对象
    public void SetGestureCtrlTargets()
    {
        if (movementControl != null)
            movementControl.SetTarget(mTarget);
        if (rotateControl != null)
            rotateControl.SetTarget(mTarget);
        if (pinchControl != null)
            pinchControl.SetTarget(mTarget);
        if (restControl != null)
            restControl.SetTarget(mTarget);
    }
    //设置对象拥有控制操作，例如只要让模型A拥有旋转，缩放功能禁用移动功能等
    public void SetGestureCtrlModuls()
    {

    }


    //update执行，判断用户的手势输入
    private void ModelGestureCtrlModuls()
    {
        if (null == mTarget) return;
        if (!mTarget.posistion || !mTarget.rotation || !mTarget.scale) return;

        //tapHandlerCtrl.ControlUpdate();
        //if (tapHandlerCtrl.GetDouble)
        //restControl.ControlUpdate();

        if (Input.touchCount >= 2)
        {
            firstPoint = Input.GetTouch(0).position;
            secondPoint = Input.GetTouch(1).position;
            //switch (Input.GetTouch(0).phase)
            //{
            //    case TouchPhase.Began:
            //        firstPoint = Input.GetTouch(0).position;
            //        break;
            //    case TouchPhase.Ended:
            //    case TouchPhase.Canceled:
            //        firstPoint = Vector2.zero;
            //        break;
            //}

            //switch (Input.GetTouch(1).phase)
            //{
            //    case TouchPhase.Began:
            //        secondPoint = Input.GetTouch(1).position;
            //        break;
            //    case TouchPhase.Ended:
            //    case TouchPhase.Canceled:
            //        secondPoint = Vector2.zero;
            //        break;
            //}

            if (Input.GetTouch(0).phase != TouchPhase.Moved || Input.GetTouch(1).phase != TouchPhase.Moved) return;
            if (Input.touchCount < 2 || Input.touchCount > 2) return;

            if (Vector3.Distance(firstPoint, secondPoint) < pixelratio)
                movementControl.ControlUpdate();
            else
                pinchControl.ControlUpdate();
        }
        else
        {
            if (Input.touchCount <= 0 || Input.touchCount > 1) return;
            if (Input.GetTouch(0).phase != TouchPhase.Moved) return;
            rotateControl.ControlUpdate();
        }
    }
    #endregion
}
[System.Serializable]
public class GestureParater
{
    public Transform posistion;
    public Transform rotation;
    public Transform scale;
}