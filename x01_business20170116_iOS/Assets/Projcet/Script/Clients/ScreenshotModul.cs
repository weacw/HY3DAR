/****
创建人：NSWell
用途：拍照模块
******/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenshotModul : BaseModuls
{

    public event OnBaseModulsOperatedHandler screenshotModulsOperatedEvent;//拍照结束-回调
    public event OnBaseModulsOperationHandler screenshotModulsOpeartionEvent;//拍照开始-回调
    public string TempFileNameFull { get; private set; }
    private int imageWidth;//照片的宽度
    private int imageHeight;//照片的高度
    public Texture2D GetImage { get; private set; }
    public Texture2D waterMark;
    private IEnumerator screenshoCoroutine;//拍照动作协程

    //开始对这个模块进行操作
    public override void ModulsOperat()
    {
        ModulsInit();
        if (screenshoCoroutine != null)
            GlobalCoroutine.Instance.AtNowStartCoroutine(screenshoCoroutine);
    }

    //在操作此模块之前的初始化
    public override void ModulsInit()
    {
        waterMark = Resources.Load<Texture>("WaterMark/compyWaterMark") as Texture2D;
        screenshoCoroutine = WaitForScreenshot();
        TempFileNameFull = null;
        imageWidth = Screen.width;
        imageHeight = Screen.height;
        if (string.IsNullOrEmpty(AppSettings.Instance.AppScreenShotPaths)) return;
        TempFileNameFull = AppSettings.Instance.AppScreenShotPaths + GetImageName();
        if (screenshotModulsOpeartionEvent != null)
            screenshotModulsOpeartionEvent.Invoke();
    }

    //拍照操作的具体逻辑执行
    private IEnumerator WaitForScreenshot()
    {
        yield return new WaitForEndOfFrame();
        if (GetImage != null)
        {
            UnityEngine.Object.Destroy(GetImage);
            GetImage = null;
        }
        GetImage = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);
        GetImage.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0, false);
        int startX = GetImage.width - waterMark.width;
        int startY = GetImage.height - waterMark.height;
        Debug.Log(startX + "-" + startY);
        for (int i = 0; i < waterMark.width; i++)
        {
            for (int j = 0; j < waterMark.height; j++)
            {
                Color color = waterMark.GetPixel(i, j);
                if (color.a<= 0.25f || color.r<=0.25f||color.g<= 0.25f || color.b<= 0.25f) continue;
                GetImage.SetPixel(startX + i, j, color);

            }
        }

        GetImage.Apply(false);

        byte[] bytes = GetImage.EncodeToPNG();
        FileOpeartion.GetFileOpeartion().FileCreater(TempFileNameFull, bytes, null);
        yield return new WaitForSeconds(0.125f);
        if (screenshotModulsOperatedEvent != null) screenshotModulsOperatedEvent.Invoke();
        GlobalCoroutine.Instance.AtNowStopCoroutine(screenshoCoroutine);
    }

    //获取当前存储的照片名字
    private string GetImageName()
    {
        string fileName = null;
        string[] nameArray = DateTime.Now.ToString("F").Split(',', ' ', ':');
        foreach (string name in nameArray)
        {
            fileName += name;
        }
        Debug.LogError(fileName);
        return fileName + ".png";
    }
}
