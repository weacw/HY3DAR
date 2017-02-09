/****
创建人：NSWell
用途：分享模块
******/
using System;
using UnityEngine;
using System.Collections;
using cn.sharesdk.unity3d;
using Object = UnityEngine.Object;

public class ShareModuls
{
    private ShareSDK ssdk;
    public class contentValue
    {
        public string title;
        public string text;
        public string imagePath;
        public string videoUrl;
    }
    //初始化
    public ShareModuls()
    {
        if (null == ssdk)
            ssdk = Object.FindObjectOfType<ShareSDK>();

        ssdk.authHandler = OnAuthResultHandler;
        ssdk.shareHandler = OnShareResultHandler;
        ssdk.showUserHandler = OnGetUserInfoResultHandler;
        ssdk.getFriendsHandler = OnGetFriendsResultHandler;
        ssdk.followFriendHandler = OnFollowFriendResultHandler;
    }

    //分享到各个平台
    public void ShareContents(contentValue cv, PlatformType platform, int contentType)
    {

        bool clientValid = ssdk.IsClientValid(platform);
        if (!clientValid)
            Object.FindObjectOfType<CallNativeFun>().ShowNativeToast("您还未安装客户端，无法分享。");

        ShareContent content = new ShareContent();

        switch (platform)
        {
            case PlatformType.SinaWeibo:
                if (contentType == 6)
                {
                    content.SetText(cv.text + "视频链接:" + cv.videoUrl);
                    content.SetShareType(1);
                }
                else
                {
                    content.SetImagePath(Manager.Instance.GetScreenshotModul.TempFileNameFull);
                    content.SetText("海源三维AR");
                    content.SetShareType(contentType);
                }
                #if UNITY_IOS || UNITY_IPHONE
                ssdk.ShowShareContentEditor(platform, content);
                #endif
                break;
            case PlatformType.QQ:
                if (contentType != 6)
                {
                    content.SetImagePath(cv.imagePath);
                    content.SetShareType(contentType);
                }
                else
                {
                    #if UNITY_IOS || UNITY_IPHONE
                    content.SetThumbImageUrl(cv.imagePath);
                    content.SetTitle(cv.title);
                    content.SetUrl(cv.videoUrl);
                    #endif
                    content.SetTitle(cv.title);
                    content.SetText(cv.text);
                    content.SetTitleUrl(cv.videoUrl);
                    content.SetImagePath(cv.imagePath);
                    content.SetShareType(ContentType.Video);
                }
                Debug.Log(contentType.ToString());
                break;
            case PlatformType.WeChat:
            case PlatformType.WeChatMoments:
                if (contentType == 6)
                {
                    content.SetTitle(cv.title);
                    content.SetText(cv.text);
                    content.SetUrl(cv.videoUrl);
                    content.SetImagePath(cv.imagePath);
                    content.SetShareType(6);
                }
                else
                {
                    content.SetImagePath(cv.imagePath);
                    content.SetShareType(contentType);
                }
                break;
        }
        #if UNITY_IOS || UNITY_IPHONE
        if (platform != PlatformType.SinaWeibo)
            ssdk.ShareContent(platform, content);
        #elif UNITY_ANDROID
            ssdk.ShareContent(platform, content);
        #endif
    }

    //分享回调
    void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            Debug.Log("authorize success !" + "Platform :" + type);
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            Debug.Log("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("cancel !");
        }
    }

    void OnGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            Debug.Log("get user info result :");
            Debug.Log(MiniJSON.jsonEncode(result));
            Debug.Log("Get userInfo success !Platform :" + type);
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            Debug.Log("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("cancel !");
        }
    }

    void OnShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            //Debug.Log("share successfully - share result :");
            //Debug.Log(MiniJSON.jsonEncode(result));
            Debug.Log("Success!!!!!!!!!!!");
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            Debug.Log("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("cancel !");
        }
    }

    void OnGetFriendsResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            Debug.Log("get friend list result :");
            Debug.Log(MiniJSON.jsonEncode(result));
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            Debug.Log("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("cancel !");
        }
    }

    void OnFollowFriendResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            Debug.Log("Follow friend successfully !");
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            Debug.Log("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            Debug.Log("cancel !");
        }
    }
}
