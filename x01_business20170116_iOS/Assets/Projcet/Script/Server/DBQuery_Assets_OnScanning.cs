/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 *Description: 发送查询表Assets的操作请求
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBQuery_Assets_OnScanning : MySQL
{
    private IEnumerator mySQL;
    private RequestToServer requestToServer;

    //发送数据库查询操作
    public override void SendMysqlQuery(string query)
    {
        requestToServer = null;
        requestToServer = GeneralWWW.Instance.GetAssetsRequest;
        //查询数据库
        SetupRequestAssets(query);
    }

    //映射json到类型
    public override bool GetJsonClass<T>(ref List<T> t, List<string> jsonList)
    {
        if (!base.GetJsonClass(ref t, jsonList)) return false;
        Manager.Instance.GetScaneManager.BeginToDownloadAbs();
        return true;
    }

    //设置请求资源查询
    public void SetupRequestAssets(string targetName)
    {
        //设置查询地址
        string php = AppSettings.Instance.clientGlobalConfigs.serverCofing.serverIPs;
        php += AppSettings.Instance.clientGlobalConfigs.phpEnvironment.SELECTPHP;

        //对服务器请求表单
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("DBNAME", AppSettings.Instance.clientGlobalConfigs.dbQuery.dbName);
        wwwForm.AddField("TABLE", AppSettings.Instance.clientGlobalConfigs.dbQuery.GetValueForQuery("assets"));
        wwwForm.AddField("ASSETSNAME", targetName);
        GeneralWWW.Instance.GetAssetsRequest.RequestParams = null;
        GeneralWWW.Instance.GetAssetsRequest.RequestParams = wwwForm;
        //请求回调，请求成功后执行
        requestToServer.RequestOver = null;
        requestToServer.RequestOver = TextToJson;
        //请求协程
        mySQL = requestToServer.WWWDownloader(Manager.Instance.OnNetworkError, php);
        GeneralWWW.Instance.OpenCoroutine(mySQL);

    }

    //将json-string格式化后映射到类-发送数据库查询操作请求成功后的回调函数
    private void TextToJson(string data)
    {
        Debug.LogError(data);
        Manager.Instance.GetScaneManager.assets.Clear();
        List<string> jsonList = new List<string>();
        jsonList.AddRange(data.Split('}'));
        GetJsonClass(ref Manager.Instance.GetScaneManager.assets, jsonList);
    }
}