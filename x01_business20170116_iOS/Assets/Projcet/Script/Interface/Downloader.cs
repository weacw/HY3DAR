/******
创建人：NSWell
用途：下载器接口
******/

using UnityEngine;
using System.Collections;

public interface IDownloader
{
    IEnumerator WWWDownloader(OnWWWErrorHandler error_callback,string path);
    bool DisposeCurrentWWW();
    IEnumerator CheckingTimeOut(OnWWWErrorHandler error_callback);
}

public enum SENDERTYPER
{
    TIPS,
    FLOATWINDOW,
}