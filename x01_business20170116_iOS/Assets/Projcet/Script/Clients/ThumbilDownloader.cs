

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 *Description:发送下载缩略图请求类
*/
using UnityEngine;
using System.Collections;

public class ThumbilDownloader : IDownloader
{
    public WWW ThumbilWww { get; private set; }
    public OnWWWEndWithPathDelegate DownloadOverSaveToLocal;
    private float timeout = 90;

    //开始下载
    public IEnumerator WWWDownloader(OnWWWErrorHandler error_callback, string path)
    {
        ThumbilWww = new WWW(path);
        yield return ThumbilWww;
        if (!string.IsNullOrEmpty(ThumbilWww.error))
        {
            Debug.LogError(ThumbilWww.error);
            if (null != error_callback)
                error_callback.Invoke(ThumbilWww.error, SenderType.Text_Tips);
            DisposeCurrentWWW();
            yield break;
        }
        if (null != DownloadOverSaveToLocal)
            DownloadOverSaveToLocal.Invoke(ThumbilWww.bytes);
        DisposeCurrentWWW();
    }

    //关闭下载
    public bool DisposeCurrentWWW()
    {
        if (null == ThumbilWww) return false;
        ThumbilWww.Dispose();
        ThumbilWww = null;
        return true;
    }

    //超时检查
    public IEnumerator CheckingTimeOut(OnWWWErrorHandler error_callback)
    {
        yield return new WaitForSeconds(timeout);
        if (!string.IsNullOrEmpty(ThumbilWww.error))
        {
            DisposeCurrentWWW();
            yield break;
        }
        if (null != error_callback)
            error_callback.Invoke(ThumbilWww.error + " timed out", SenderType.Text_Tips);
        DisposeCurrentWWW();
    }
}
