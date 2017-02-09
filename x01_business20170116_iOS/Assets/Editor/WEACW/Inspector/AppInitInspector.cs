using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(AppSettings))]
public class AppInitInspector : Editor
{
    private ClientGlobalConfigs cc;
    private string mPath = "/Projcet/Artwork/Configs/ClientGlobalConfigs.asset";

    private AppSettings _appSettings;
    public void OnEnable()
    {
        _appSettings = (AppSettings)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!File.Exists(Application.dataPath + mPath))
        {
            if (_appSettings.clientGlobalConfigs == null)
                _appSettings.clientGlobalConfigs =
                    AssetDatabase.LoadAssetAtPath<ClientGlobalConfigs>("Assets" + mPath);

            if (GUILayout.Button("Creat Cliecnt Configs"))
            {
                cc = CreateInstance<ClientGlobalConfigs>();
                AssetDatabase.CreateAsset(cc, "Assets" + mPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
        }
        else
        {
            if (GUILayout.Button("Set Cliecnt Configs"))
            {
                cc = AssetDatabase.LoadAssetAtPath<ClientGlobalConfigs>("Assets" + mPath);
                EditorGUIUtility.PingObject(cc);
                if (_appSettings.clientGlobalConfigs == null)
                    _appSettings.clientGlobalConfigs = cc;
            }
        }
    }
}
//[CustomEditor(typeof(ClientGlobalConfigs))]
public class CliecntGlobalConfigsInspector : Editor
{
    private ClientGlobalConfigs c;    
    private AnimBool mShowServer;
    private AnimBool mShowClient;
    private AnimBool mShowDBQuery;
    public void OnEnable()
    {
        c = (ClientGlobalConfigs)target;
        mShowServer = new AnimBool(true);
        mShowServer.valueChanged.AddListener(Repaint);

        mShowClient= new AnimBool(true);
        mShowClient.valueChanged.AddListener(Repaint);

        mShowDBQuery = new AnimBool(true);
        mShowDBQuery.valueChanged.AddListener(Repaint);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical("Box");
        DBQueryInspector();
        ServerConfigsInspector();
        ClientConfigsInspector();
        EditorGUILayout.EndVertical();
    }

    private void ClientConfigsInspector()
    {
        mShowClient.target = EditorGUILayout.Toggle("Client Configs", mShowClient.target, EditorStyles.radioButton);
        if (EditorGUILayout.BeginFadeGroup(mShowClient.faded))
        {
            EditorGUILayout.BeginVertical("Box");
            c.clientConfigs.projectName = EditorGUILayout.TextField("Project Name ", c.clientConfigs.projectName);
            c.clientConfigs.canGestureCtrl = EditorGUILayout.Toggle("Can Gesture Ctrl ", c.clientConfigs.canGestureCtrl);
            c.clientConfigs.canRecordingVideo = EditorGUILayout.Toggle("Can Recording Video ", c.clientConfigs.canRecordingVideo);
            c.clientConfigs.canShare = EditorGUILayout.Toggle("Can Share ", c.clientConfigs.canShare);
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.EndFadeGroup();

    }
    private void ServerConfigsInspector()
    {
        mShowServer.target = EditorGUILayout.Toggle("Server Configs", mShowServer.target,EditorStyles.radioButton);
        if (EditorGUILayout.BeginFadeGroup(mShowServer.faded))
        {
            EditorGUILayout.BeginVertical("Box");
            c.serverCofing.serverIPs = EditorGUILayout.TextField("Server IPs ", c.serverCofing.serverIPs);
            c.serverCofing.mysqlUserName = EditorGUILayout.TextField("MySql UserName ", c.serverCofing.mysqlUserName);
            c.serverCofing.mysqlPassword = EditorGUILayout.TextField("MySql Password ", c.serverCofing.mysqlPassword);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFadeGroup();
    }
    private void DBQueryInspector()
    {

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Key & Value", "buttonleft"))
        {
            c.dbQuery.key_POST_Form.Add("key");
            c.dbQuery.value_POST_Form.Add("value");
        }
        if (GUILayout.Button("Clear All Data", "buttonright"))
        {
            c.dbQuery.key_POST_Form.Clear();
            c.dbQuery.value_POST_Form.Clear();
        }
        EditorGUILayout.EndHorizontal();
        mShowDBQuery.target = EditorGUILayout.Toggle("DB Query Key & Value", mShowDBQuery.target, EditorStyles.radioButton);
        if (EditorGUILayout.BeginFadeGroup(mShowDBQuery.faded))
        {
            for (int i = 0; i < c.dbQuery.key_POST_Form.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                c.dbQuery.key_POST_Form[i] = EditorGUILayout.TextField(c.dbQuery.key_POST_Form[i],
                    GUILayout.Width(EditorGUIUtility.currentViewWidth*0.4f));
                c.dbQuery.value_POST_Form[i] = EditorGUILayout.TextField(c.dbQuery.value_POST_Form[i],
                    GUILayout.Width(EditorGUIUtility.currentViewWidth*0.4f));
                if (GUILayout.Button("-"))
                {
                    c.dbQuery.key_POST_Form.RemoveAt(i);
                    c.dbQuery.value_POST_Form.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();
    }
}