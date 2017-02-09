/****
创建人：NSWell
用途：打开网页
******/
using UnityEngine;
using System.Collections;

public class OpenWebViewForInteraction : BaseInteraction
{
    public string url;
    public override void DoInteraction(GameObject target,int id)
    {
        base.DoInteraction(target,id);
        Application.OpenURL(url);
    }
}
