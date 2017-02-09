/******
创建人：NSWell
用途：动态加载识别图
******/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;
public delegate bool OnLoadDatasetsSuccess(string path, VuforiaUnity.StorageType storageType);

public class AtRuntimeLoadDBs : MonoBehaviour
{
    private ObjectTracker objectTracker;
    public OnLoadDatasetsSuccess onLoadSuccess;
    public void Init()
    {
        onLoadSuccess = LoadDataset;
    }

    public bool LoadDataset(string dataSetPath, VuforiaUnity.StorageType storageType)
    {
        if (!VuforiaRuntimeUtilities.IsVuforiaEnabled()) return false;
        if (!DataSet.Exists(dataSetPath, storageType)) return false;
        objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        DataSet dataSet = objectTracker.CreateDataSet();
        if (!dataSet.Load(dataSetPath, storageType)) return false;
        objectTracker.ActivateDataSet(dataSet);
        return true;
    }

    public void UpdateImageTarget()
    {
        List<ImageTargetBehaviour> trackers = new List<ImageTargetBehaviour>();
        trackers.AddRange(FindObjectsOfType<ImageTargetBehaviour>());

        for (int i = 0; i < trackers.Count; i++)
        {
            trackers[i].gameObject.name = trackers[i].TrackableName;
            trackers[i].gameObject.AddComponent<TrackableEvent>();
            trackers[i].gameObject.AddComponent<TurnOffBehaviour>();

            //设置手势操作对象
            GameObject mPos = new GameObject("Position");
            GameObject mRot = new GameObject("Rotation");
            GameObject mScale = new GameObject("Scale");

            mRot.transform.SetParent(mPos.transform);
            mScale.transform.SetParent(mRot.transform);
            mPos.transform.SetParent(trackers[i].transform);
            mPos.transform.position = mRot.transform.position = mScale.transform.position = Vector3.zero;
            mPos.transform.rotation = mRot.transform.rotation = mScale.transform.rotation = Quaternion.identity;
            mPos.transform.localScale = mRot.transform.localScale = mScale.transform.localScale = Vector3.one;
        }
        Manager.Instance.GetScaneManager.FindAllTracker();
    }

}
