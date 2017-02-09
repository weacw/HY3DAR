using UnityEngine;

public class ServerConfig : ScriptableObject
{
    [Header("SERVER INFO")]
    public string serverIP = "127.0.0.1";
    public string userID = "root";
    public string userPsd = "weacw";

    [Header("PHP HELPER")]
    public string CONNECTSQLPHP = "ConnectMySQL.php";
    public string DISCONNECTSQLPHP = "CloseMySQL.php";
    public string CREATEDBPHP = "CreateDatabases.php";
    public string CREATTABLEPHP = "CreateTable.php";
    public string INSERTPHP = "InsertData.php";
    public string SELECTPHP = "SelectData.php";
    public string UPLOADASSETBUNDLE = "UploadAssetbundle.php";
}