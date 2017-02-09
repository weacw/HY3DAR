/****
创建人：NSWell
用途：用户交互
******/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionOf3DN2DBlending : BaseInteraction
{
    public string scriptTarget;
    private BaseInteraction baseInteraction;
    public List<BlendingTarget> blendingTarget;
    private GameObject tmpTarget;
    private GameObject self;
    public override void Start()
    {
        self = this.gameObject;                
        switch (operatType)
        {
            case OperatType.The2D:
                baseInteraction = GameObject.Find(scriptTarget).GetComponent<BaseInteraction>();
                if (baseInteraction != null)
                    baseInteraction.m_CreatedAction = BindingMethodToTarget;
                break;
            case OperatType.The3D:
                break;
        }
    }

    public override void DoInteraction(GameObject target, int id)
    {
        base.DoInteraction(target, id);
        Interaction(id);
    }

    private void BindingMethodToTarget(BaseInteraction bi)
    {
        if (bi.GetType() != typeof(CreateIntorductionUISingle)) return;
        CreateIntorductionUISingle ciu = bi as CreateIntorductionUISingle;

        for (int i = 0; i < ciu.BtnList.Count; i++)
        {
            var id = i;
            ciu.AddEventToBtn(ciu.BtnList[i], () =>
            {
                Interaction(id);
            });
        }
    }

    public void Interaction(int id)
    {
        switch (blendingTarget[id].invokeMethod)
        {
            case BlendingTarget.Method.SetGameObjectActive_True:
                if (tmpTarget == null)
                    tmpTarget = GameObject.Find(blendingTarget[id].ctrlTargetName);
                tmpTarget.SetActive(true);
                break;
            case BlendingTarget.Method.SetGameObjectActive_False:
                if (tmpTarget == null)
                    tmpTarget = GameObject.Find(blendingTarget[id].ctrlTargetName);
                tmpTarget.SetActive(false);
                break;
            case BlendingTarget.Method.SetGameobjectActive_True_OffOther:
                if (tmpTarget == null)
                    tmpTarget = GameObject.Find(blendingTarget[id].ctrlTargetName);
                self = GameObject.Find(blendingTarget[id].closeTarget);
                self.SetActive(false);
                tmpTarget.SetActive(true);
                break;
            case BlendingTarget.Method.SetGameobjectActive_False_OnOther:
                if (tmpTarget == null)
                    tmpTarget = GameObject.Find(blendingTarget[id].ctrlTargetName);
                self = GameObject.Find(blendingTarget[id].closeTarget);
                self.SetActive(true);
                tmpTarget.SetActive(false);
                break;
            case BlendingTarget.Method.OpenWebSet:
                Application.OpenURL(blendingTarget[id].ctrlTargetName);
                break;
            case BlendingTarget.Method.DoAnimate_Trigger:
                if (tmpTarget == null)
                    tmpTarget = GameObject.Find(blendingTarget[id].ctrlTargetName);
                Animator ani = tmpTarget.GetComponentInChildren<Animator>();
                ani.SetTrigger(blendingTarget[id].condition);
                break;
            case BlendingTarget.Method.PlaySounds:
                if (tmpTarget == null)
                    tmpTarget = GameObject.Find(blendingTarget[id].ctrlTargetName);
                AudioSource audio = tmpTarget.GetComponentInChildren<AudioSource>();
                audio.clip = blendingTarget[id].ac;
                audio.Play();
                break;
            case BlendingTarget.Method.StopSounds:
                if (tmpTarget == null)
                    tmpTarget = GameObject.Find(blendingTarget[id].ctrlTargetName);
                tmpTarget.GetComponentInChildren<AudioSource>().Stop();
                break;
        }
    }
}
[System.Serializable]
public class BlendingTarget
{
    public string ids;
    public GameObject bingdingTarget;
    public Method invokeMethod;
    public string ctrlTargetName;
    public string condition;
    public string closeTarget;

    public AudioClip ac;
    public enum Method
    {
        SetGameObjectActive_True,
        SetGameObjectActive_False,
        SetGameobjectActive_True_OffOther,
        SetGameobjectActive_False_OnOther,
        OpenWebSet,
        DoAnimate_Trigger,
        PlaySounds,
        StopSounds
    }
}
