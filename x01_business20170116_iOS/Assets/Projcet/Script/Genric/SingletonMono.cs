/******
创建人：NSWell
用途：Mono 单例
******/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static object mLock = new object();
    private static bool applicationIsQuitting = false;
    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                 "' already destroyed on application quit." +
                                 " Won't create again - returning null.");
                return null;
            }
            lock (mLock)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                }
            }
            return instance;
        }
    }
}
