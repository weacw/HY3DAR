/******
创建人：NSWell
用途：全局协程管理器
******/

using UnityEngine;
using System.Collections;

public class GlobalCoroutine : SingletonMono<GlobalCoroutine>
{
    public void AtNowStartCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void AtNowStopCoroutine(IEnumerator coroutine)
    {
        StopCoroutine(coroutine);
    }
}
