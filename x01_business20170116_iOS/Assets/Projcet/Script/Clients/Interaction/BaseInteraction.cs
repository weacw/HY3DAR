/****
创建人：NSWell
用途：用户交互
******/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum OperatType
{
    The3D,
    The2D
};
public class BaseInteraction : MonoBehaviour
{
    public Action<BaseInteraction> m_CreatedAction;
    public OperatType operatType;
    public virtual void Init()
    {

    }
    public virtual void Awake()
    {

    }
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }


    public virtual void DoInteraction(GameObject target,int id)
    {

    }

    public virtual void CreatePrefab(CreatePrefabConfig createPrefabConfig, out GameObject prefab)
    {
        Transform root = GameObject.Find(createPrefabConfig.cloneObjParent).transform;
        prefab = Instantiate(createPrefabConfig.cloneObjPrefab, root) as GameObject;

        switch (createPrefabConfig.cloneObjectAxisType)
        {
            case CreatePrefabConfig.CloneObjectAxisType.Local:
                prefab.transform.localPosition = createPrefabConfig.cloneObjPos;
                prefab.transform.localRotation = Quaternion.Euler(createPrefabConfig.cloneObjRotation);
                break;
            case CreatePrefabConfig.CloneObjectAxisType.World:
                prefab.transform.position = createPrefabConfig.cloneObjPos;
                prefab.transform.rotation = Quaternion.Euler(createPrefabConfig.cloneObjRotation);
                break;
        }

        prefab.transform.localScale = createPrefabConfig.cloneObjScale;
    }
}

[System.Serializable]
public class CreatePrefabConfig
{
    public bool isAutoShowing;
    public bool isAutoSetting;
    public Vector3 cloneObjPos = Vector3.zero;
    public Vector3 cloneObjRotation = Vector3.zero;
    public Vector3 cloneObjScale = Vector3.one;

    public GameObject cloneObjPrefab;
    public String cloneObjParent = "Root Name";
    public CloneObjectAxisType cloneObjectAxisType = CloneObjectAxisType.Local;
    public UIType uiType = UIType.Image;
    public enum CloneObjectAxisType
    {
        Local,
        World
    };
    public enum UIType
    {
        Button,
        Image,
        Text,
        Other
    }
}