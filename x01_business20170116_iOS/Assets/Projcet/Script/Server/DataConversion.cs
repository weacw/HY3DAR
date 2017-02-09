

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * 数据转化
*/

using UnityEngine;

public class DataConversion : SingletonMono<DataConversion>
{
    public static T LoadJsonFormString<T>(string json)
    {
        json = json.Replace("[", "");
        json = json.Replace("]", "");
        json = json.Replace(",{", "{");
        json += "}";
        return JsonUtility.FromJson<T>(json);
    }
}