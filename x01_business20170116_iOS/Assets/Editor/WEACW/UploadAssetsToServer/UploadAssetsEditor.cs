

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RequestDataResults;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;



public class UploadAssetsEditor : EditorWindow
{
    public static UploadAssetsEditor MWindow { get; private set; }
    private string path = "Assets/Editor/WEACW/Resources/CONFIGS/";
    public ServerConfig Config { get; private set; }
    private bool showSettingBtn = true;
    private bool isConnected = false;
    private UploadAssetsCore uac;
    private int index = 0;
    private string zipFilePath = "Empty";
    private string[] toolbarNames = new[] { "Image Target", "Assets Upload", "Setting" };
    private List<AssetUploadVar> assetUploadVars = new List<AssetUploadVar>();
    private Vector2 scroolVector2;
    [MenuItem("Window/WEACW Tools/Upload Editor")]
    private static void Init()
    {
        if (null == MWindow)
            MWindow = GetWindow<UploadAssetsEditor>();
        MWindow.titleContent = new GUIContent("Upload Assets");
        MWindow.Config = Resources.Load<ServerConfig>("CONFIGS/SERVERCONFIG");
        MWindow.showSettingBtn =
            !System.IO.File.Exists(Application.dataPath + "/Editor/WEACW/Resources/CONFIGS/SERVERCONFIG.asset");
    }

    private void OnGUI()
    {
        if (!MWindow) Init();
        index = GUILayout.Toolbar(index, toolbarNames);
        switch (index)
        {
            case 0:
                ImageTargetUpload();
                break;
            case 1:
                AssetsUpload();
                break;
            case 2:
                ServerConfigsCreate();
                MySQLConnectTest();
                break;
        }
    }

    private void AssetsUpload()
    {
        if (assetUploadVars.Count != 0)
        {
            scroolVector2 = EditorGUILayout.BeginScrollView(scroolVector2, false, false);
            for (int i = 0; i < assetUploadVars.Count; i++)
            {
                EditorGUILayout.BeginVertical("Box");
                assetUploadVars[i].bundleType = (BundleType)EditorGUILayout.EnumPopup("Bundle Type", assetUploadVars[i].bundleType);
                assetUploadVars[i].name = EditorGUILayout.TextField("Name ", assetUploadVars[i].name);
                assetUploadVars[i].alisa = EditorGUILayout.TextField("Alisa ", assetUploadVars[i].alisa);
                switch (assetUploadVars[i].bundleType)
                {
                    case BundleType.Model:
                        assetUploadVars[i].assetbundleAndroid =
         EditorGUILayout.ObjectField("Assetbundle Android", assetUploadVars[i].assetbundleAndroid, typeof(Object), false);
                        assetUploadVars[i].assetbundleIOS =
          EditorGUILayout.ObjectField("Assetbundle IOS", assetUploadVars[i].assetbundleIOS, typeof(Object), false);
                        break;
                    case BundleType.Video:
                        assetUploadVars[i].videoUrl = EditorGUILayout.TextField("Video Url ", assetUploadVars[i].videoUrl);
                        break;
                    case BundleType.Mixed:
                        assetUploadVars[i].assetbundleAndroid =
EditorGUILayout.ObjectField("Assetbundle Android", assetUploadVars[i].assetbundleAndroid, typeof(Object), false);
                        assetUploadVars[i].assetbundleIOS =
          EditorGUILayout.ObjectField("Assetbundle IOS", assetUploadVars[i].assetbundleIOS, typeof(Object), false);
                        assetUploadVars[i].videoUrl = EditorGUILayout.TextField("Video Url ", assetUploadVars[i].videoUrl);
                        break;
                }
                assetUploadVars[i].version = EditorGUILayout.FloatField("Version ", assetUploadVars[i].version);

                assetUploadVars[i].thumbnail =
                    EditorGUILayout.ObjectField("Thumbnail", assetUploadVars[i].thumbnail, typeof(Texture), false) as
                        Texture;
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Upload"))
                {

                    switch (assetUploadVars[i].bundleType)
                    {
                        case BundleType.Model:
                            #region 向服务器上传Android Assetbundle
                            string pathAndroid = AssetDatabase.GetAssetPath(assetUploadVars[i].assetbundleAndroid);
                            byte[] bytesAndroid = FileToBytes(pathAndroid);
                            WWWForm wwwFormAndroid = new WWWForm();
                            wwwFormAndroid.AddField("Name", assetUploadVars[i].name + ".assetbundle");
                            wwwFormAndroid.AddField("path", "Assetbundle/Android/");
                            wwwFormAndroid.AddBinaryData("post", bytesAndroid);
                            WWW wwwAndroid = new WWW(Config.serverIP + "/" + Config.UPLOADASSETBUNDLE, wwwFormAndroid);
                            IEnumerator uploadAndrdoid = Upload(wwwAndroid);
                            #endregion
                            #region 向服务器上传IOS Assetbundle
                            string pathIOS = AssetDatabase.GetAssetPath(assetUploadVars[i].assetbundleIOS);
                            byte[] bytesIOS = FileToBytes(pathIOS);
                            WWWForm wwwFormIOS = new WWWForm();
                            wwwFormIOS.AddField("Name", assetUploadVars[i].name + ".assetbundle");
                            wwwFormIOS.AddField("path", "Assetbundle/IOS/");
                            wwwFormIOS.AddBinaryData("post", bytesIOS, "multipart/form-data");
                            WWW wwwIOS = new WWW(Config.serverIP + "/" + Config.UPLOADASSETBUNDLE, wwwFormIOS);
                            IEnumerator upload = Upload(wwwIOS);
                            #endregion
                            EditorCoroutineRunner.StartEditorCoroutine(uploadAndrdoid);
                            break;
                        case BundleType.Video:
                            break;
                        case BundleType.Mixed:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }



                    //向服务器上传缩略图
                    string path2 = AssetDatabase.GetAssetPath(assetUploadVars[i].thumbnail);
                    byte[] bytes2 = FileToBytes(path2);
                    WWWForm wwwForm2 = new WWWForm();
                    wwwForm2.AddField("Name", assetUploadVars[i].name + ".jpg");
                    wwwForm2.AddField("path", "Thumbnails");
                    wwwForm2.AddBinaryData("post", bytes2, "multipart/form-data");
                    WWW www2 = new WWW(Config.serverIP + "/" + Config.UPLOADASSETBUNDLE, wwwForm2);
                    IEnumerator upload2 = Upload(www2);

                    #region insert data to asset table
                    WWWForm dataForm = new WWWForm();
                    string androidUrl = null, iosUrl = null;

                    androidUrl = Config.serverIP + "/Assetbundle/Android/" + assetUploadVars[i].name + ".assetbundle";
                    iosUrl = Config.serverIP + "/Assetbundle/IOS/" + assetUploadVars[i].name + ".assetbundle";


                    string sqlcmd_Insert =
                        string.Format(
                            "INSERT INTO `assets` (`id`, `alias`,`assetname`, `bundletype`,`videourl`,`androidUrl`, `iosUrl`, `thumbnails`,`version`) VALUES(NULL,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                            assetUploadVars[i].alisa,
                            assetUploadVars[i].name,
                            assetUploadVars[i].bundleType,
                            assetUploadVars[i].videoUrl,
                            androidUrl, iosUrl,
                            Config.serverIP + "/Thumbnails/" + assetUploadVars[i].name + ".jpg",
                            assetUploadVars[i].version);
                    string sqlcmd_Query = string.Format("SELECT * FROM `assets` WHERE `assetname` LIKE '{0}'",
                        assetUploadVars[i].name);
                    string sqlcmd_Update = string.Format("UPDATE `assets` SET `alias`={0},`assetname` = '{1}' ," +
                                                         "`bundletype`='{2}',`videourl`='{3}'" +
                                                         "`androidUrl`='{4}' ,`iosUrl`='{5}',`version`='{6}' WHERE `assets`.`assetname` = '{7}'",
                        assetUploadVars[i].alisa,
                        assetUploadVars[i].name,
                        assetUploadVars[i].bundleType,
                        assetUploadVars[i].videoUrl,
                        androidUrl, iosUrl,
                        assetUploadVars[i].version,
                        assetUploadVars[i].name);
                    dataForm.AddField("SQL_Insert", sqlcmd_Insert);
                    dataForm.AddField("SQL_Update", sqlcmd_Update);
                    dataForm.AddField("SQL_Query", sqlcmd_Query);
                    dataForm.AddField("DBNAME", "hy3d");
                    WWW dataWWW = new WWW(Config.serverIP + "/" + Config.INSERTPHP, dataForm);
                    IEnumerator insert = Upload(dataWWW);
                    EditorCoroutineRunner.StartEditorCoroutine(insert);
                    #endregion
                    EditorCoroutineRunner.StartEditorCoroutine(upload2);
                    //EditorCoroutineRunner.StartEditorCoroutine(upload);

                }
                if (GUILayout.Button("Remove"))
                {
                    assetUploadVars.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
        if (GUILayout.Button("Add new post"))
        {
            assetUploadVars.Add(new AssetUploadVar());
        }
    }

    //识别图上传
    private void ImageTargetUpload()
    {
        EditorGUILayout.LabelField("Image Target package", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.TextField("Save Path ", zipFilePath);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", "Buttonleft"))
        {
            zipFilePath = EditorUtility.SaveFilePanel("Save imagetarget to Zip", "Assets/", "WEACWImageTarget", "zip");
        }
        if (GUILayout.Button("Zip", "Buttonright"))
        {
            List<string> filePath = new List<string>();
            filePath.Add(Application.dataPath + "/StreamingAssets/QCAR");
            CompressFileZip.CompressFile(filePath, zipFilePath, CompressFileZip.CompressType.Directory);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();


        if (GUILayout.Button("Upload the file-Now"))
        {
            byte[] bytes = FileToBytes(zipFilePath);
            WWWForm wwwForm = new WWWForm();
            wwwForm.AddBinaryData("post", bytes, "multipart/form-data");
            wwwForm.AddField("Name", "WEACWImageTarget");
            WWW www = new WWW("127.0.0.1/WS/UploadFiles.php", wwwForm);
            EditorCoroutineRunner.StartEditorCoroutine(Upload(www));
        }
        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.Label(new Rect(rect.width / 2 - 45, MWindow.position.height - 25, 180, 25), "Powerd by Well Tsai");
    }
    //服务器设置
    private void ServerConfigsCreate()
    {
        if (showSettingBtn)
        {
            if (GUILayout.Button("Enable Configs"))
            {
                ServerConfig sc = ScriptableObject.CreateInstance<ServerConfig>();
                if (!System.IO.Directory.Exists("Assets/Editor/WEACW/Resources/CONFIGS"))
                {
                    AssetDatabase.CreateFolder("Assets/Editor/WEACW/Resources/", "CONFIGS");
                }
                AssetDatabase.CreateAsset(sc, path + "SERVERCONFIG.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorGUIUtility.PingObject(sc);
                Config = Resources.Load<ServerConfig>("CONFIGS/SERVERCONFIG");
                showSettingBtn =
                    !System.IO.File.Exists(Application.dataPath + "/Editor/WEACW/Resources/CONFIGS/SERVERCONFIG.asset");
            }
        }
    }

    //数据库连接测试
    private void MySQLConnectTest()
    {
        if (!Config) return;
        EditorGUILayout.BeginVertical("Box");
        Config.serverIP = EditorGUILayout.TextField("Server IP", Config.serverIP);
        Config.userID = EditorGUILayout.TextField("User ID", Config.userID);
        Config.userPsd = EditorGUILayout.TextField("User Password", Config.userPsd);

        if (GUILayout.Button("Connect", EditorStyles.miniButton))
        {
            //TODO:连接数据测试
            if (null == uac)
                uac = new UploadAssetsCore(Config);
            isConnected = uac.ConnectTest();
        }
        EditorGUILayout.EndVertical();
    }

    //上传
    private IEnumerator Upload(WWW www)
    {
        yield return www;
        while (!www.isDone)
        {
            yield return null;
        }
        if (!string.IsNullOrEmpty(www.error))
            Debug.Log(www.error);

        Debug.Log("Upload success" + www.text);
        if (www.text.Contains("success"))
        {
            Debug.Log("Upload success" + www.text);
            www.Dispose();
            www = null;
        }
    }


    //file to bytes
    private byte[] FileToBytes(string filePath)
    {
        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, bytes.Length);
        fileStream.Close();
        return bytes;
    }
}


public class AssetUploadVar
{
    public string name;
    public float version = 1.0f;
    public string alisa;
    public Texture thumbnail;
    public Object assetbundleAndroid;
    public Object assetbundleIOS;
    public BundleType bundleType;
    public string videoUrl;
    public enum Platform
    {
        Editor,
        Android,
        IOS,
        UWP
    };

    public AssetUploadVar()
    {
        videoUrl = UploadAssetsEditor.MWindow.Config.serverIP + "/Assetbundle/video/";
    }
}