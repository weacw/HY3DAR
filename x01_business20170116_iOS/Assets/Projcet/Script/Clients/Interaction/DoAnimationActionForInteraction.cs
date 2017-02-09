/****
创建人：NSWell
用途：对UI按钮进行事件添加
******/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class DoAnimationActionForInteraction : MonoBehaviour
{
    public Animator selfAnimator;
    public CreateIntorductionUISingle cius;
    public List<string> animateNameList = new List<string>();
    public bool isAuto;
    private void Start()
    {
        if (!isAuto || cius == null) return;
        if (cius.config.uiType != CreatePrefabConfig.UIType.Button) return;
        if (selfAnimator == null)
            selfAnimator = GetComponentInChildren<Animator>();
        for (int i = 0; i < cius.BtnList.Count; i++)
        {
            var i1 = i;
            cius.AddEventToBtn(cius.BtnList[i], () =>
            {
                selfAnimator.SetTrigger(animateNameList[i1]);
                Debug.Log(animateNameList[i1]);
            });
        }
    }



}
