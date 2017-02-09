/******
创建人：NSWell
用途：App版本检查
******/
using UnityEngine;
using System.Collections;
using RequestDataResults;
public class AppVersionContrl
{
    public Appversion GetAppversion { get; private set; }

    internal void Start()
    {
        #if UNITY_ANDROID
        GlobalCoroutine.Instance.AtNowStartCoroutine(CheckingAppVersion());
        #endif
    }

    private IEnumerator CheckingAppVersion()
    {
        WWWForm wf = new WWWForm();
        wf.AddField("DBNAME",AppSettings.Instance.clientGlobalConfigs.dbQuery.dbName);
        wf.AddField("TABLE","appversion");
        WWW www = new WWW(AppSettings.Instance.clientGlobalConfigs.serverCofing.serverIPs+"/QueryVersion.php",wf);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
            www.Dispose();
            yield break;
        }        
        string mJson = www.text.Replace("[", "").Replace("]","");
        Debug.Log(mJson);
        GetAppversion = JsonUtility.FromJson<Appversion>(mJson); 
        GetAppversion.CheckAppversion();
    }
}
