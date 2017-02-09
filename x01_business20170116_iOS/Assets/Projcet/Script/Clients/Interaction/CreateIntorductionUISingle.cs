/****
创建人：NSWell
用途：用户交互-动态创建UI按钮
******/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateIntorductionUISingle : BaseInteraction
{
    public CreatePrefabConfig config;
    private GameObject mGameObject;
    public List<Button> BtnList;//{ get; private set; }
    public override void Awake()
    {
        base.Awake();
        if (config != null && config.isAutoShowing)
            CreatePrefab(config, out mGameObject);

        Init();
    }

    internal void AddEventToBtn(Button btn, UnityAction id)
    {
        btn.onClick.AddListener(id);
    }

    private void OnDestroy()
    {
        if (mGameObject != null)
            Destroy(mGameObject);
    }


    public override void Init()
    {
        base.Init();
        if (mGameObject == null) return;
        if (config == null) return;
        if (config.uiType != CreatePrefabConfig.UIType.Button) return;
        BtnList = new List<Button>();
        BtnList.AddRange(mGameObject.GetComponentsInChildren<Button>());
    }

    public override void Start()
    {
        base.Start();

    }

    public override void DoInteraction(GameObject target, int id)
    {
        base.DoInteraction(target, id);
        CreatePrefab(config, out mGameObject);
    }

    public override void CreatePrefab(CreatePrefabConfig createPrefabConfig, out GameObject prefab)
    {
        if (mGameObject != null)
        {
            prefab = mGameObject;
            return;
        }
        base.CreatePrefab(createPrefabConfig, out prefab);

        RectTransform instaceRectTrans = prefab.GetComponent<RectTransform>();
        RectTransform orgineRectTrans = createPrefabConfig.cloneObjPrefab.GetComponent<RectTransform>();
        instaceRectTrans.anchorMax = orgineRectTrans.anchorMax;
        instaceRectTrans.anchorMin = orgineRectTrans.anchorMin;
        instaceRectTrans.sizeDelta = orgineRectTrans.sizeDelta;
        instaceRectTrans.offsetMax = orgineRectTrans.offsetMax;
        instaceRectTrans.offsetMin = orgineRectTrans.offsetMin;
        instaceRectTrans.pivot = orgineRectTrans.pivot;

        StartCoroutine(BindingEvent());
    }

    private IEnumerator BindingEvent()
    {
        yield return new WaitForSeconds(0.25f);
        Init();

        if (m_CreatedAction == null) yield break;
        m_CreatedAction.Invoke(this);
    }
}
