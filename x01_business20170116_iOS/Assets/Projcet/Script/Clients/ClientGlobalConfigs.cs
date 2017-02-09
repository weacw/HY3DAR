/******
创建人：NSWell
用途：客户端全局配置
******/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientGlobalConfigs : ScriptableObject
{
    public DBQuery dbQuery;
    public ServerConfigs serverCofing;
    public ClienConfigs clientConfigs;
    public PHPEnvironmentObject phpEnvironment;
}


[System.Serializable]
public class DBQuery
{
    public string dbName;
    public List<string> key_POST_Form = new List<string>();
    public List<string> value_POST_Form = new List<string>();
    private Dictionary<string, string> dbQueryKey = new Dictionary<string, string>();
    public void Init()
    {
        for (int i = 0; i < key_POST_Form.Count; i++)
        {
            if (!dbQueryKey.ContainsKey(key_POST_Form[i]))
                dbQueryKey.Add(key_POST_Form[i], value_POST_Form[i]);
        }
    }

    public string GetValueForQuery(string key)
    {
        Init();
        if (!dbQueryKey.ContainsKey(key)) return null;
        return dbQueryKey[key];
    }
}
[System.Serializable]
public class ServerConfigs
{
    public string serverIPs;
    public string mysqlUserName;
    public string mysqlPassword;
    public string serverbundlesurl;
    public string serverTrackersurl;
}
[System.Serializable]
public class ClienConfigs
{
    public string projectName;
    public bool canShare;
    public bool canRecordingVideo;
    public bool canGestureCtrl;
}

[System.Serializable]
public class PHPEnvironmentObject
{
    public string path;
    public string CONNECTSQLPHP = "ConnectMySQL.php";
    public string DISCONNECTSQLPHP = "CloseMySQL.php";
    public string CREATEDBPHP = "CreateDatabases.php";
    public string CREATTABLEPHP = "CreateTable.php";
    public string INSERTPHP = "InsertData.php";
    public string SELECTPHP = "SelectData.php";
    public string UPLOADASSETBUNDLE = "UploadAssetbundle.php";
    public string UPLOADVIDEOS = "UploadVideo.php";
    public string QUERYDNTABLE = "QueryAssettable.php";
}