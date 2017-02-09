

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * 用途：双击、单击操作
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool OnDoubleTapHandler();
public delegate bool OnSingleTapHandler();
public class TapHandlerCtrl : BaseControl
{


    public OnDoubleTapHandler onDoubleTapHandler;
    public OnSingleTapHandler onSingleTapHandler;
    public bool GetDouble { get { return getDoubleTap; } }
    private bool getDoubleTap;
    //秒数
    private const float DOUBLE_TAP_MAX_DELAY = 0.25f;
    private int mTapCount = 0;
    private float mTimeSinceLastTap;
    public override void ControlUpdate()
    {
        HandleTap(out getDoubleTap);
    }

    private void HandleTap(out bool status)
    {
        status = false;
        switch (mTapCount)
        {
            case 1:
                mTimeSinceLastTap += Time.deltaTime;
                //超过双击时间间隔即为单击
                if (mTimeSinceLastTap > DOUBLE_TAP_MAX_DELAY)
                {
                    mTapCount = 0;
                    mTimeSinceLastTap = 0;
                    if (null != onSingleTapHandler)
                        onSingleTapHandler();
                    status = false;
                }
                break;
            case 2:
                mTimeSinceLastTap = 0;
                mTapCount = 0;
                if (null != onDoubleTapHandler)
                    onDoubleTapHandler();
                status = true;
                break;
        }
        if (Input.GetMouseButtonUp(0))
        {
            mTapCount++;
            if (mTapCount == 1)
                OnSingleTap();
        }
    }

    protected virtual void OnSingleTap() { }

}