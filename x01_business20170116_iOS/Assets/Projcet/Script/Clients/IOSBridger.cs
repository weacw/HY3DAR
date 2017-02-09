/****
创建人：NSWell
用途：ios平台桥接器
******/
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
public class IOSBridger 
{
	[DllImport("__Internal")]
	private static extern void _UnzipForUnity(string filePath,string outPath);	
	public static void UnzipForUnity(string filePath,string outPath)
	{
		_UnzipForUnity(filePath,outPath);
	}

	[DllImport("__Internal")]
	private static extern void _ShowIOSToast(string msg);
	public static void ShowIOSToast(string msg)
	{
		_ShowIOSToast(msg);
	}

    [DllImport("__Internal")]
    private static extern void _ShowIOSAlertDialog(string title,string contents,string btnName);
    public static void ShowIOSAlertDialog(string title,string contents,string btnName)
    {
        _ShowIOSAlertDialog(title,contents,btnName);
    }
    [DllImport("__Internal")]
    private static extern void _SaveVideoToAblum(string url);
    public static void SaveVideoToAblum(string url)
    {
        _SaveVideoToAblum(url);
    }

    [DllImport("__Internal")]
    private static extern void _SavePhotoToAblum(string url);
    public static void SaveImageToAblum(string url)
    {
        _SavePhotoToAblum(url);
    }

    [DllImport("__Internal")]
    private static extern void _ReEncoding(string input,string output);
    public static void ReEncoding(string input,string output)
    {
        _ReEncoding(input,output);
    }
}
