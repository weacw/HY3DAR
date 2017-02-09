/****
创建人：NSWell
用途：下载识别图
******/
using UnityEngine;
using System.Collections;

public class TrackersDownloader : IDownloader
{
    public WWW TrackerWww { get; private set; }
    public OnWWWUIHandler OnDonloadTrackersBegin { get; set; }
    public OnWWWEndDelegate OnDownloadOver { get; set; }
    private float timeout = 30;
    //开始下载
    public IEnumerator WWWDownloader(OnWWWErrorHandler error_callback,string path)
    {
        if (null != OnDonloadTrackersBegin) OnDonloadTrackersBegin.Invoke();
        TrackerWww = new WWW(path);
        yield return TrackerWww;
        if (null == TrackerWww) yield break;
        if (!string.IsNullOrEmpty(TrackerWww.error))
        {
            Debug.LogError(TrackerWww.error);
            if (null != error_callback) error_callback.Invoke(TrackerWww.error, SenderType.FLOAT_WINDOW);
            DisposeCurrentWWW();
            yield break;
        }
        if (null != OnDownloadOver) OnDownloadOver.Invoke(TrackerWww.bytes);
    }
    //关闭下载
    public bool DisposeCurrentWWW()
    {
        if (null == TrackerWww) return false;
        TrackerWww.Dispose();
        TrackerWww = null;
        return true;
    }
    //超时检查
    public IEnumerator CheckingTimeOut(OnWWWErrorHandler error_callback)
    {
        yield return new WaitForSeconds(timeout);
        if (null != error_callback &&null!=TrackerWww) error_callback.Invoke(TrackerWww.error, SenderType.FLOAT_WINDOW);
        DisposeCurrentWWW();
    }
}
