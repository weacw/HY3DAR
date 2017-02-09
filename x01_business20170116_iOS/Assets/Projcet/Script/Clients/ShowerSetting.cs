/****
创建人：NSWell
用途：模型数据配置
******/
using System;
using UnityEngine;
using System.Collections;

public class ShowerSetting : MonoBehaviour
{
    public Vector3 pivotPosition;

    public bool supportPinch;
    public Vector3 on2DDist = Vector3.forward;
    public Vector3 quaternion_onFound;
    public Vector3 quaternion_onLost;
    public Vector3 maxSize = Vector3.one, minSize = Vector3.one;

    public bool supportMovement;
    public bool supportRotateX, supportRotateY;
    public bool isVideoBundle;
    public float percent = 0.5f;
    private Vector3 size;

    //丢失调用-还原角度 位置等
    public void FixedAngleByLosting()
    {
        if (isVideoBundle)
        {
            Debug.Log(Quaternion.Euler(new Vector3(-90,0,0)));
            Debug.Log(transform.localRotation);

            transform.localRotation = Quaternion.Euler(quaternion_onLost);
            transform.localPosition = Vector3.zero;

        }
        Manager.Instance.GetScaneManager.GetTrackableTarget.transform.localPosition = on2DDist;
        Debug.Log(transform.localRotation);
    }

    //发现调用-还原角度 位置等
    public void FixedAngleByFinding()
    {
        if (isVideoBundle)
        {
            transform.localRotation = Quaternion.Euler(quaternion_onFound);
            transform.localPosition = pivotPosition;
        }
        Manager.Instance.GetScaneManager.GetTrackableTarget.transform.localPosition = Vector3.zero;
    }

    //脚本启动调用
    public void OnEnable()
    {
        Debug.Log(Manager.Instance.GetScaneManager.ContentStatus);
        switch (Manager.Instance.GetScaneManager.ContentStatus)
        {
            case TrackerContentStatus.OnTracker:
                FixedAngleByFinding();
                break;
            case TrackerContentStatus.LoseTracker:
                FixedAngleByLosting();
                break;
        }
    }
}
