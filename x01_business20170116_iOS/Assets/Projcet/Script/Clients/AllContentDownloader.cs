/******
创建人：NSWell
用途：下载所有内容中的单个内容
******/
using UnityEngine;
using System.Collections;

public class AllContentDownloader : IDownloader
{
    public WWW FavoriteWWW { get; private set; }
    public OnWWWUIHandler onFAVDownloadBegin;
    public OnWWWUIHandler onFAVDownloadEnd;
    public OnWWWEndDelegate onFAVDownloadOver;
    private float timeout = 90;
    public IEnumerator WWWDownloader(OnWWWErrorHandler error_callback,string path)
    {
        if(null== onFAVDownloadBegin)
            onFAVDownloadBegin.Invoke();

        string url = AppSettings.Instance.clientGlobalConfigs.serverCofing.serverIPs + "/" +
                     AppSettings.Instance.clientGlobalConfigs.phpEnvironment.QUERYDNTABLE;
        WWWForm favForm = new WWWForm();
        favForm.AddField("DBNAME",AppSettings.Instance.clientGlobalConfigs.dbQuery.dbName);
        favForm.AddField("TABLE", AppSettings.Instance.clientGlobalConfigs.dbQuery.GetValueForQuery("assets"));
        FavoriteWWW = new WWW(url,favForm);
        yield return FavoriteWWW;
        if (null == FavoriteWWW)
            FavoriteWWW = new WWW(AppSettings.Instance.clientGlobalConfigs.serverCofing.serverbundlesurl);
        if (!string.IsNullOrEmpty(FavoriteWWW.error))
        {
            Debug.LogError(FavoriteWWW.error);
            if (null != error_callback)
                error_callback.Invoke(FavoriteWWW.error, SenderType.Text_Tips);
            DisposeCurrentWWW();
            yield break;
        }


        if (null != onFAVDownloadOver)
            onFAVDownloadOver.Invoke(FavoriteWWW.bytes);
        if (null != onFAVDownloadEnd)
            onFAVDownloadEnd.Invoke();


        Debug.Log(FavoriteWWW.text);
    }

    public bool DisposeCurrentWWW()
    {
        if (null == FavoriteWWW) return false;
        FavoriteWWW.Dispose();
        FavoriteWWW = null;
        return true;
    }

    public IEnumerator CheckingTimeOut(OnWWWErrorHandler error_callback)
    {
        yield return new WaitForSeconds(timeout);
        if (!string.IsNullOrEmpty(FavoriteWWW.error))
        {
            DisposeCurrentWWW();
            yield break;
        }
        if (null != error_callback)
            error_callback.Invoke(FavoriteWWW.error + " timed out", SenderType.Text_Tips);
        DisposeCurrentWWW();
    }
}
