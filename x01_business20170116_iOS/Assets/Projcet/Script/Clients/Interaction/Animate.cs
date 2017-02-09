/****
创建人：NSWell
用途：用于播放模型动画
******/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimateType
{
    Trigger,
    Bool,
    Float,
    Int
}

public class Animate : BaseInteraction
{
    private Animator animate;
    public AnimateType animateType;
    public List<string> condition = new List<string>();
    private GameObject target;
    public bool CanInvoke { get; private set; }

    public override void DoInteraction(GameObject target, int id)
    {
        base.DoInteraction(target, id);
        this.target = target;

        switch (operatType)
        {
            case OperatType.The3D:
                InvokeOn3D(id);
                break;
            case OperatType.The2D:
                InvokeOn2D(id);
                break;
        }
    }

    private void InvokeOn2D(int id)
    {
        switch (animateType)
        {
            case AnimateType.Trigger:
                animate.SetTrigger(condition[id]);
                break;
            case AnimateType.Bool:
                animate.SetBool(condition[id],!animate.GetBool(condition[id]));
                break;
            case AnimateType.Float:
                break;
            case AnimateType.Int:
                break;
        }
    }

    private void InvokeOn3D(int id)
    {
        switch (animateType)
        {
            case AnimateType.Trigger:
                break;
            case AnimateType.Bool:
                break;
            case AnimateType.Float:
                break;
            case AnimateType.Int:
                break;
        }
    }
    
    public override void Init()
    {
        base.Init();
        if (animate != null) return;
        CanInvoke = false;
        enabled = false;
        animate = target.GetComponentInChildren<Animator>();
    }

    public override void Start()
    {
        base.Start();
        Init();
    }
}
