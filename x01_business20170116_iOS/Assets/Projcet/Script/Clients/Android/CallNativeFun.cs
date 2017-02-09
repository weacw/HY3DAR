/****
创建人：NSWell
用途：调用源生函数
******/


using UnityEngine;
using System.Collections;

public class CallNativeFun : MonoBehaviour
{
    //显示提示
    public void ShowNativeToast(string mTips)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                CallAndroidFun("ShowToastTip", mTips);
                break;
            case RuntimePlatform.IPhonePlayer:
                //TODO:IOSBridge call
                IOSBridger.ShowIOSToast(mTips);
                break;
        }
        Debug.Log(mTips);
    }

    //显示对话提示
    public void ShowAlertDialog(string title, string contents, string btnA, string btnB, string funA, string funB)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                string[] p = new[] { "ShowAlertDialog", title, contents, btnA, btnB, funA, funB };
                CallAndroidFun(p);
                break;
            case RuntimePlatform.IPhonePlayer:
                //TODO:IOSBridge call
                IOSBridger.ShowIOSAlertDialog(title,contents,btnA);
                break;
        }

    }

    //存储图片
    public void SaveImageNativeFun(string path, string title, string tips)
    {

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                CallAndroidFun("SaveImages", path, title, tips);
                break;
            case RuntimePlatform.IPhonePlayer:
                //TODO:IOSBridge call
                IOSBridger.SaveImageToAblum(path);
                break;
        }
    }

    //解压zip
    public void Decompressionzip(string path, string targetDir)
    {

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                CallAndroidFun("Decompression", path, targetDir);
                break;
            case RuntimePlatform.IPhonePlayer:
                //TODO:IOSBridge call
                IOSBridger.UnzipForUnity(path,targetDir);
                break;
        }
    }

    //获取SDCard路径
    public string GetSDcardPath()
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return jo.Call<string>("GetSDcardPath");
            }
        }
    }

    //扫描文件
    public void ScanFile(string path)
    {

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        jo.Call("scanFile",path);
                    }
                }
                break;
            case RuntimePlatform.IPhonePlayer:
                //TODO:IOSBridge call
                IOSBridger.SaveVideoToAblum(path);
                break;
        }
    }
    /// <summary>
        /// 调用安卓源生函数
        /// </summary>
        /// <param name="mObj">调用的函数以及参数，[0]:调用函数名,[1]:形参1,[2]:形参2...</param>
        private
        void CallAndroidFun(params object[] mObj)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                switch (mObj.Length)
                {
                    case 2:
                        jo.Call(mObj[0].ToString(), mObj[1].ToString());
                        break;
                    case 3:
                        jo.Call(mObj[0].ToString(), mObj[1].ToString(), mObj[2].ToString());
                        break;
                    case 4:
                        jo.Call(mObj[0].ToString(), mObj[1].ToString(), mObj[2].ToString(), mObj[3].ToString());
                        break;
                    case 7:
                        jo.Call(mObj[0].ToString(), mObj[1].ToString(), mObj[2].ToString(), mObj[3].ToString(),
                         mObj[4].ToString(), mObj[5].ToString(), mObj[6].ToString());
                        break;
                }
            }
        }
    }
}
