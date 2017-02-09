

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * 移动模型控制
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControl : ModelGestureControl
{

    public float movementRate;

    public MovementControl(float movementRate)
    {
        this.movementRate = movementRate;
        tounchType = TouchType.MOVEMENT;
    }

    public override void ControlUpdate()
    {
        if (null == this.GetTarget().posistion) return;
        float zAngle = Manager.Instance.GetScaneManager.Camera2DPos.localEulerAngles.z;
        float x=0, y=0;
        if (zAngle.Equals(0))
        {
            x = InputFeedback().x*-1;
            y = InputFeedback().y;
        }
        else
        {
            x = InputFeedback().x;
            y = InputFeedback().y;
        }
        switch (Manager.Instance.GetScaneManager.ContentStatus)
        {
            case TrackerContentStatus.OnTracker:
                GetTarget().posistion.Translate(new Vector3(-x,0, y) * Time.deltaTime * movementRate);

                break;
            case TrackerContentStatus.LoseTracker:
                GetTarget().posistion.Translate(new Vector3(-x, y, 0) * Time.deltaTime * movementRate);

                break;
        }
    }
}