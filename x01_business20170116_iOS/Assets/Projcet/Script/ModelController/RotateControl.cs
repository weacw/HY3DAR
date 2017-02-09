

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * 模型旋转
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateControl : ModelGestureControl
{
    public float rotateRate;

    public RotateControl(float rotateRate)
    {
        this.rotateRate = rotateRate;
        tounchType = TouchType.ROTATE;
    }
    public override void ControlUpdate()
    {
        if (null == this.GetTarget().rotation) return;
        float zAngle = Manager.Instance.GetScaneManager.Camera2DPos.localEulerAngles.z;
        float x = 0, y = 0;
        if (zAngle.Equals(0))
        {
            y = InputFeedback().y;
            x = InputFeedback().x * -1;
        }
        else
        {
            x = InputFeedback().x;
            y = InputFeedback().y;
        }

        GetTarget().rotation.Rotate(new Vector3(y, x, 0) * Time.deltaTime * rotateRate, Space.Self);
    }

}