

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadAssetsCore
{
    private const string USERID="USERID";
    private const string USERPSD = "USERPSD";
    private WWW www;
    private ServerConfig serverConfig;

    public UploadAssetsCore(ServerConfig serverConfig)
    {
        this.serverConfig = serverConfig;
    }



    internal bool ConnectTest()
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField(USERID,serverConfig.userID);
        wwwForm.AddField(USERPSD, serverConfig.userPsd);
        wwwForm.AddField("AUTO", 0);
        www = new WWW(serverConfig.serverIP + "/" + serverConfig.CONNECTSQLPHP, wwwForm);
        EditorCoroutineRunner.StartEditorCoroutine(WWWPOST());       
        return true;
    }



    private IEnumerator WWWPOST()
    {
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
            yield return null;
        }
        Debug.Log(www.text);
    }
}