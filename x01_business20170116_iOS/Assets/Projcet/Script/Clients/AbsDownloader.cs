/****
创建人：NSWell
用途：用于下载服务器资源
******/
using UnityEngine;
using System.Collections;

public class AbsDownloader : IDownloader
{
    public WWW AbsWww { get; private set; }
    public OnWWWUIHandler onAbsDownloadBegin;
    public OnWWWUIHandler onAbsDownloadEnd;
    public OnWWWEndDelegate onAbsDownloadOver;    
    private float timeout = 90;
    /// <summary>
    /// 下载Assetbundle
    /// </summary>
    /// <param name="error_callback">出错回调</param>
    public IEnumerator WWWDownloader(OnWWWErrorHandler error_callback,string path)
    {

        AbsWww = new WWW(path);
        //开始进行资源下载的回调
        if (null != onAbsDownloadBegin)
            onAbsDownloadBegin.Invoke();
        yield return AbsWww;

        //if(null==AbsWww)
        //    AbsWww = new WWW(AppSettings.Instance.clientGlobalConfigs.serverCofing.serverbundlesurl);
        if (!string.IsNullOrEmpty(AbsWww.error))
        {
            Debug.LogError(AbsWww.error);
            if (null != error_callback)
                error_callback.Invoke(AbsWww.error, SenderType.Text_Tips);
            DisposeCurrentWWW();    
            yield break;
        }
      
        //资源下载 、加载完毕后执行

        if (null != onAbsDownloadOver)
            onAbsDownloadOver.Invoke(AbsWww.bytes);
        if (null != onAbsDownloadEnd)
            onAbsDownloadEnd.Invoke();        
    }

    /// <summary>
    /// 关闭www
    /// </summary>
    /// <returns></returns>
    public bool DisposeCurrentWWW()
    {
        if (null == AbsWww) return false;
        AbsWww.Dispose();
        AbsWww = null;
        return true;
    }

    public IEnumerator CheckingTimeOut(OnWWWErrorHandler error_callback)
    {
        yield return new WaitForSeconds(timeout);
        if (!string.IsNullOrEmpty(AbsWww.error))
        {
            DisposeCurrentWWW();
            yield break;
        }
        if (null != error_callback)
            error_callback.Invoke(AbsWww.error+ " timed out", SenderType.Text_Tips);
        DisposeCurrentWWW();
    }
}
