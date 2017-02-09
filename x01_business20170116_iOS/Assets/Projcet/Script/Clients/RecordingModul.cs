/****
创建人：NSWell
用途：录像模块
******/
using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using Object = UnityEngine.Object;

public class RecordingModul : BaseModuls
{
    //public event OnBaseModulsOperationHandler recordModulsOpeartionEvent;//录像中-回调
    public event OnBaseModulsOperatedHandler recordModulsOpeartedEvent;
    public string CurVideoName { get; private set; }
    public string TempFileNameFull { get; private set; }
    public bool IsStartRecord { get; private set; }
    private bool isStopRecord;
    public float CurTime { get; private set; }
    private int totalTime = 15;
    private IEnumerator recordingCoroutine;
    private byte[] bytes;
    public bool IsUploaded { get; private set; }
    public string tempFullName;
    public override void ModulsOperat()
    {
        ModulsInit();
        if (!IsStartRecord && !isStopRecord)
        {
            IsStartRecord = true;
            isStopRecord = false;
            StartRecording();
        }
        else
        {
            isStopRecord = true;
            IsStartRecord = false;
            Everyplay.StopRecording();
        }

    }

    public override void ModulsInit()
    {
        
        if (!Everyplay.IsSupported())
        {
            Object.FindObjectOfType<CallNativeFun>().ShowNativeToast("设备不支持该功能");
        }
        else if (!isStopRecord)
        {
            if (recordingCoroutine == null)
                recordingCoroutine = Recording();
           
            //Everyplay.SetMaxRecordingSecondsLength(totalTime);
            Everyplay.SetLowMemoryDevice(true);
            Everyplay.SetTargetFPS(30);
            Everyplay.SetDisableSingleCoreDevices(true);
            Everyplay.SetMotionFactor(2);
            Everyplay.RecordingStopped += StopRecording;
            Everyplay.RecordingStarted += Manager.Instance.OnRecordStarted;
        }
    }

    private void StartRecording()
    {
        Everyplay.StartRecording();
        GlobalCoroutine.Instance.AtNowStartCoroutine(recordingCoroutine);
    }

    private void StopRecording()
    {
        MediaPlayerCtrl mpc = MonoBehaviour.FindObjectOfType<MediaPlayerCtrl>();
        if (mpc != null)
            mpc.Pause();
        AudioListener.volume = 0;

        CurTime = 0;
        if (recordingCoroutine != null)
        {
            GlobalCoroutine.Instance.AtNowStopCoroutine(recordingCoroutine);
            recordingCoroutine = null;
        }
        isStopRecord = false;
        IsStartRecord = false;
        string tempFile = Application.temporaryCachePath;
#if UNITY_IPHONE || UNITY_IOS
        tempFile = tempFile.Replace("Library/Caches", "tmp");
#endif
        DirectoryInfo dir = new DirectoryInfo(tempFile);
        var files = dir.GetFiles("*.mp4", SearchOption.AllDirectories);
        var file = files.OrderByDescending(f => f.CreationTime).FirstOrDefault();
        if (file != null && string.IsNullOrEmpty(file.FullName)) { return; }
        TempFileNameFull = file.FullName;
       
       
        #if UNITY_IOS|| UNITY_IPHONE
        string fileName = file.Name;
        tempFullName = TempFileNameFull;
        TempFileNameFull = TempFileNameFull.Replace(fileName, fileName + "01.mp4");
        IOSBridger.ReEncoding(tempFullName, TempFileNameFull);
        #endif

        //GlobalCoroutine.Instance.AtNowStartCoroutine(UploadToServer());
        //bytes = FileOpeartion.GetFileOpeartion().ReadFileFromLocal(TempFileNameFull);
        if (recordModulsOpeartedEvent != null)
            recordModulsOpeartedEvent.Invoke();
        CurVideoName = GetVideoName();
    }

    public IEnumerator UploadToServer()
    {
        bytes = FileOpeartion.GetFileOpeartion().ReadFileFromLocal(TempFileNameFull);
        IsUploaded = false;
        Object.FindObjectOfType<CallNativeFun>().ShowNativeToast("开始上传");
        yield return new WaitForSeconds(.25f);
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("name", CurVideoName);
        wwwForm.AddBinaryData("file", bytes);
        WWW www = new WWW(AppSettings.Instance.clientGlobalConfigs.serverCofing.serverIPs + AppSettings.Instance.clientGlobalConfigs.phpEnvironment.UPLOADVIDEOS, wwwForm);
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) Debug.Log(www.error);
        Debug.Log(www.text);
        Object.FindObjectOfType<CallNativeFun>().ShowNativeToast("上传完毕");
        IsUploaded = true;
    }

    //获取当前存储的照片名字
    public string GetVideoName()
    {
        string fileName = null;
        string[] nameArray = DateTime.Now.ToString("F").Split(',', ' ', ':');
        foreach (string name in nameArray)
        {
            fileName += name;
        }
        Debug.LogError(fileName);
        return fileName + "_.mp4";
    }

    private IEnumerator Recording()
    {
        while (!isStopRecord)
        {
            yield return null;
            if (CurTime >= totalTime)
            {
                CurTime = 0;
                isStopRecord = true;
                Everyplay.StopRecording();
                GlobalCoroutine.Instance.AtNowStopCoroutine(recordingCoroutine);
                recordingCoroutine = null;
                yield break;
            }
            CurTime += Time.deltaTime;
        }
    }
}
