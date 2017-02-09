

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * 放大模型控制
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchControl : ModelGestureControl
{
    //最小至最大尺寸
    private float minSize, maxSize;
    //比例因子
    public float perspectiveZoomSpeed = 0.05f; 

    public PinchControl(float minSize, float maxSize)
    {
        this.maxSize = maxSize;
        this.minSize = minSize;
    }

    public override void ControlUpdate()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // 在每一个触摸的前一帧中找到位置。
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // 查找在每个帧中的接触之间的向量的大小（距离）.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // 找出每一帧之间的距离的差异。
        float deltaMagnitudeDiff =  touchDeltaMag- prevTouchDeltaMag;
        float localSizeX = GetTarget().scale.localScale.x;
        //否则改变之间的距离的变化的基础上.
        localSizeX += deltaMagnitudeDiff*perspectiveZoomSpeed;
        GetTarget().scale.localScale = new Vector3(localSizeX, localSizeX, localSizeX);
        // 限制范围
        float clamp = Mathf.Clamp(GetTarget().scale.localScale.x, minSize, maxSize);
        GetTarget().scale.localScale = new Vector3(clamp, clamp, clamp);
    }
}