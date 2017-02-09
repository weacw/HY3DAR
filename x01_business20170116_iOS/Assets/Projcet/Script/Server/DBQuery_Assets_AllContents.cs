/******
创建人：NSWell
用途：数据库查询
******/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DBQuery_Assets_AllContents : MySQL
{
    private IEnumerator mySQL;

    public override void SendMysqlQuery(string query)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return;

        string php = AppSettings.Instance.clientGlobalConfigs.serverCofing.serverIPs;
        php += AppSettings.Instance.clientGlobalConfigs.phpEnvironment.QUERYDNTABLE;

        SetupRequestAssets();
        GeneralWWW.Instance.GetAssetsRequest.RequestOver = null;
        GeneralWWW.Instance.GetAssetsRequest.RequestOver = TextToJson;
        mySQL = GeneralWWW.Instance.GetAssetsRequest.WWWDownloader(Manager.Instance.OnNetworkError, php);
        GeneralWWW.Instance.OpenCoroutine(mySQL);
    }

    public override bool GetJsonClass<T>(ref List<T> t, List<string> jsonList)
    {
        if (!base.GetJsonClass(ref t, jsonList)) return false;
        Manager.Instance.GetAllContentsManager.CreateFavoriteItem();
        return true;
    }

    private void TextToJson(string data)
    {
        //Manager.Instance.GetScaneManager.assets.Clear();
        List<string> jsonList = new List<string>();
        if (data.Contains("404") || data.Contains("no found"))
        {
            Manager.Instance.OnNetworkError("404", SenderType.Text_Tips);
            return;
        }
        jsonList.AddRange(data.Split('}'));
        GetJsonClass(ref Manager.Instance.GetAllContentsManager.contentAssets, jsonList);
    }


    public void SetupRequestAssets()
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("DBNAME", AppSettings.Instance.clientGlobalConfigs.dbQuery.dbName);
        wwwForm.AddField("TABLE", AppSettings.Instance.clientGlobalConfigs.dbQuery.GetValueForQuery("assets"));
        wwwForm.AddField("PAGE", Object.FindObjectOfType<AllContentsManager>().Index);
        GeneralWWW.Instance.GetAssetsRequest.RequestParams = null;
        GeneralWWW.Instance.GetAssetsRequest.RequestParams = wwwForm;
    }
}
