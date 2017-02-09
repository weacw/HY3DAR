

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 *Description:请求发送类
*/

using System.Collections;

public class GeneralWWW : SingletonMono<GeneralWWW>
{

    public void GeneralInit()
    {
        if (null == GetAssetsRequest)
            GetAssetsRequest = new RequestToServer();
        if (null == GetAbsDownloader)
            GetAbsDownloader = new AbsDownloader();
        if (null == GetTrackerDownloader)
            GetTrackerDownloader = new TrackersDownloader();
        if (null == GetAllContentDownloader)
            GetAllContentDownloader = new AllContentDownloader();
        if (null == GetThumbilDownloader)
            GetThumbilDownloader = new ThumbilDownloader();
        if(null == GetAbsDownloaderNoLoad)
            GetAbsDownloaderNoLoad = new AbsDownloader();
    }

    public void OpenCoroutine(IEnumerator coroutine)
    {
        StopWWW(coroutine);
        StartCoroutine(coroutine);
    }

    public void OpenCheckingCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
    //停止协程
    public void StopWWW(IEnumerator coroutine)
    {
            StopCoroutine(coroutine);
    }

    //数据库查询操作
    public AbsDownloader GetAbsDownloader { get; private set; }
    public RequestToServer GetAssetsRequest { get; private set; }
    public TrackersDownloader GetTrackerDownloader { get; private set; }
    public AllContentDownloader GetAllContentDownloader { get; private set; }
    public ThumbilDownloader GetThumbilDownloader { get; private set; }
    public AbsDownloader GetAbsDownloaderNoLoad { get; private set; }
}
