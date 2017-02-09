

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * 重置模型状态
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestControl : ModelGestureControl
{
    public override void ControlUpdate()
    {
        if (null == GetTarget()) return;
        if (!GetTarget().posistion || !GetTarget().rotation || !GetTarget().scale) return;
        GetTarget().scale.localScale = Vector3.one;
        GetTarget().posistion.localPosition = Vector3.zero;
        GetTarget().rotation.localRotation = Quaternion.identity;
    }
}