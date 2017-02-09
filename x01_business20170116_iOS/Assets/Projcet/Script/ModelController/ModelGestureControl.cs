

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * 手势操作控制
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModelGestureControl : BaseControl
{
    public enum TouchType
    {
        NONE,
        MOVEMENT,
        ROTATE,
        PINCHZOOM
    };

    public TouchType tounchType;
    private GestureParater mTarget;
    //设置控制目标
    public virtual void SetTarget(GestureParater mTraget)
    {
        this.mTarget = mTraget;
    }

    public GestureParater GetTarget()
    {
        return mTarget;
    }

    //输入反馈
    public virtual Vector2 InputFeedback()
    {
        return Application.platform == RuntimePlatform.Android
               || Application.platform == RuntimePlatform.IPhonePlayer
            ? TouchInputFeedback()
            : MouseInputFeedback();
    }

    //触摸输入
    private Vector2 TouchInputFeedback()
    {
        return AxisInput();
    }

    //鼠标输入
    private Vector2 MouseInputFeedback()
    {
        return !Input.GetMouseButton(0) ? Vector2.zero : AxisInput();
    }

    //轴输入
    private Vector2 AxisInput()
    {
        float zAngle = Manager.Instance.GetScaneManager.Camera2DPos.localEulerAngles.z;
        float x, y;

        x = Input.GetAxis("Mouse X");
        y = Input.GetAxis("Mouse Y");

        return new Vector2(x, y);
    }
}