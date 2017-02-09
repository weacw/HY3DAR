 
 
/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using UnityEditor;
using UnityEngine;

public class CompressFileZip
{
    public enum CompressType
    {
        Directory,
        File
    };
    public static void CompressFile(List<string> file,string savePath , CompressType type)
    {
        //压缩  
        using (ZipFile zip = new ZipFile(savePath))
        {
            for (int i = 0; i < file.Count; i++)
            {
                switch (type)
                {
                    case CompressType.Directory:
                        //压缩文件夹  
                        zip.AddDirectory(file[i], "");
                        break;
                    case CompressType.File:
                        //压缩文件  
                        zip.AddFile(file[i], "");
                        break;
                }
            }
            //保存  
            zip.Save();
        }
    }
}