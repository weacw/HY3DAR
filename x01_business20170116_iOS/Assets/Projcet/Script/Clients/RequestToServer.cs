/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 *Description: 查询数据库表操作
*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RequestToServer : IDownloader
{
    public WWW Request { get; private set; }
    public WWWForm RequestParams { get; set; }
    public OnRequestOver RequestOver { get; set; }
    private float timeout = 30;
    //用于查询数据库操作
    public IEnumerator WWWDownloader(OnWWWErrorHandler error_callback, string path)
    {
        //请求地址，表单
        Request = new WWW(path, RequestParams);
        yield return Request;

        if (!string.IsNullOrEmpty(Request.error))
        {
            Debug.LogError(Request.error);
            if (null != error_callback) error_callback.Invoke(Request.error, SenderType.Text_Tips);
            DisposeCurrentWWW();
            yield break;
        }        
        
        if (Request.text.Contains(RequestErrorCode._404ErrorCode) || Request.text.Contains(RequestErrorCode._NoFoundErrorCode))
        {
            if (null != error_callback) error_callback.Invoke(RequestErrorCode._404ErrorCode, SenderType.Text_Tips);
            yield break;
        }
        if (null == RequestOver) yield break;
        RequestOver.Invoke(Request.text);
        RequestOver = null;
        DisposeCurrentWWW();
    }

    //销毁 www 流
    public bool DisposeCurrentWWW()
    {
        if (null == Request) return false;
        Request.Dispose();
        Request = null;
        RequestParams = null;
        return true;
    }

    //检测超时
    public IEnumerator CheckingTimeOut(OnWWWErrorHandler error_callback)
    {
        yield return new WaitForSeconds(timeout);
        Debug.LogError(Request.error);
        if (null != error_callback) error_callback.Invoke(Request.error, SenderType.Text_Tips);
        DisposeCurrentWWW();
    }
}
