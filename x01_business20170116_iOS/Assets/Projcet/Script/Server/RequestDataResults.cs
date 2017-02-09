

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * 数据类型
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RequestDataResults
{
    [System.Serializable]
    public class Appversion
    {
        public string appName;
        public string appVersion;
        public int beUse;
        public string appUrl;
        public  void CheckAppversion()
        {
            Manager.Instance.GetUIManager.CheckVersion();
        }
    }

    [System.Serializable]
    public class Thumbnails
    {
        public int id;
        public string tag;
        public string thumbnail;
        public string name;
        public string assetversion;
        public string assetpath;
    }

    [System.Serializable]
    public class Assets
    {
        public int id;
        public string alias;
        public string assetname;
        public string bundletype;
        public string videourl;
        public string androidUrl;
        public string iosUrl;
        public string thumbnails;
        public float version;

        public BundleType GetBundleType()
        {
            return (BundleType) Enum.Parse(typeof(BundleType), bundletype);
        }
    }

    public enum BundleType
    {
        Model,
        Video,
        Mixed
    };

    }