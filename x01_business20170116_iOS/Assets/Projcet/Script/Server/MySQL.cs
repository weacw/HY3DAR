

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * json到list操作
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MySQL
{
    public abstract void SendMysqlQuery(string query);
    public virtual bool GetJsonClass<T>(ref List<T> t, List<string> jsonList)
    {
        for (int i = 0; i < jsonList.Count - 1; i++)
            t.Add(DataConversion.LoadJsonFormString<T>(jsonList[i]));
        if (t.Count > 0)
            return true;
        return false;
    }

}