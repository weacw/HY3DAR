

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
  *Description: 下载Assetbundle，并创建
*/

using System;
using System.Collections;
using RequestDataResults;
using UnityEngine;


public class DownloadAbsNCreate : SingletonMono<DownloadAbsNCreate>
{
    internal Transform scale, rotation, posistion;
    private IEnumerator checkingCoroutine;
    public byte[] GetCurContentBytes { get; private set; }
    private AbsDownloader absDownloader;
    private string fileName;
    private GameObject targetImage;
    public AssetBundleRequest Request { get; private set; }
    public AssetBundleCreateRequest AbsRequest { get; private set; }
    private bool isReady;
    //发起资源下载请求操作
    public void DownLoad(string path)
    {
        int lastIndex = path.LastIndexOf("/") + 1;
        fileName = path.Substring(lastIndex, path.Length - lastIndex);
        if (fileName.Contains("assetbundle"))
            fileName = fileName.Replace(".assetbundle", "");
        else
            fileName = fileName.Replace(".mp4", "");

        InitCtrlTarget();
        absDownloader = GeneralWWW.Instance.GetAbsDownloader;

        //资源下载完毕后的回调
        if (null == absDownloader.onAbsDownloadOver)
            absDownloader.onAbsDownloadOver += OnDownloadAbsOver;

        //发起资源下载请求
        GeneralWWW.Instance.OpenCoroutine(absDownloader.WWWDownloader(Manager.Instance.OnNetworkError, path));

        //发起超时请求
        checkingCoroutine =
            absDownloader.CheckingTimeOut(Manager.Instance.OnNetworkError);
        GeneralWWW.Instance.OpenCheckingCoroutine(checkingCoroutine);
    }

    //寻找手势控制轴
    private void InitCtrlTarget()
    {
        if (null == Manager.Instance.GetScaneManager || null == Manager.Instance.GetScaneManager.GetTrackableTarget)
        {
            targetImage = GameObject.Find(fileName);
            posistion = targetImage.transform.FindChild("Position").transform;
            rotation = posistion.FindChild("Rotation").transform;
            scale = rotation.FindChild("Scale").transform;
            if (Manager.Instance.GetScaneManager != null)
                Manager.Instance.GetScaneManager.FromFavoriteCreated(fileName, true);
            return;
        }
        if (!Manager.Instance.GetScaneManager.GetTrackableTarget) return;
        posistion = Manager.Instance.GetScaneManager.GetTrackableTarget.transform.FindChild("Position").transform;
        rotation = posistion.FindChild("Rotation").transform;
        scale = rotation.FindChild("Scale").transform;
    }


    //www下载数据结束后加载数据并且实例化到场景
    public bool OnDownloadAbsOver(byte[] bytes)
    {
        if (bytes.Length <= 0) return false;
        GeneralWWW.Instance.StopWWW(checkingCoroutine);
        StartCoroutine(WaitToInstantiate(bytes));
        return true;
    }

    private IEnumerator WaitToInstantiate(byte[] bytes)
    {
        BundleType bundleType = BundleType.Model;
        if (Manager.Instance.GetScaneManager.assets.Count > 0)
            bundleType = Manager.Instance.GetScaneManager.assets[0].GetBundleType();
        else if (null != Manager.Instance.GetAllContentsManager.mClickElement)
        {
            int id = Manager.Instance.GetAllContentsManager.mClickElement.guid;
            bundleType = Manager.Instance.GetAllContentsManager.contentAssets[id].GetBundleType();
        }
        else if (FindObjectOfType<FavoriteManager>().favoriteAssets.Count > 0)
        {
            bundleType = FindObjectOfType<FavoriteManager>().favoriteAssets[fileName].GetBundleType();
        }
        switch (bundleType)
        {
            case BundleType.Model:
                AssetBundleCreateRequest absRequest = AssetBundle.LoadFromMemoryAsync(bytes);

                //异步加载需要一定的时间，故直到异步加载完毕后在进行下一步
                while (!absRequest.isDone)
                    yield return null;

                if (!absRequest.assetBundle) yield return null;
                Request = absRequest.assetBundle.LoadAllAssetsAsync<GameObject>();

                //如上异步加载
                while (!Request.isDone)
                    yield return null;


                //实例化模型
                GameObject myTempGameobject = Request.allAssets[0] as GameObject;
                GameObject go = Instantiate(myTempGameobject);

                ShowerSetting ss = go.GetComponent<ShowerSetting>();
                if (ss != null)
                {
                    rotation.localPosition = ss.pivotPosition;
                }

                go.transform.SetParent(scale);
                go.transform.localScale = myTempGameobject.transform.localScale;
                go.transform.localPosition = new Vector3(rotation.transform.localPosition.x, -rotation.localPosition.y, -rotation.transform.localPosition.z);
                go.transform.localRotation = Quaternion.identity;
                Debug.Log(go.transform.localRotation);

                if (ss != null)
                {
                    ss.OnEnable();
                }

                //消除assetbundle
                absRequest.assetBundle.Unload(false);

                //设置手势控制轴
                MakeGestureAxis(go);

                break;
            case BundleType.Video:
                FileOpeartion.GetFileOpeartion()
                    .FileCreater(AppSettings.Instance.AppAssetbundlePaths + "temp.mp4", bytes, () =>
                    {
                        isReady = true;
                    });

                yield return new WaitForSeconds(0.5f);
                Debug.Log(FileOpeartion.GetFileOpeartion().CheckingFile(AppSettings.Instance.AppAssetbundlePaths + "temp.mp4"));
                GameObject videoTemp = Instantiate(Manager.Instance.GetScaneManager.videoPrefab);
                videoTemp.transform.SetParent(scale);
                videoTemp.transform.localScale = videoTemp.transform.localScale;
                videoTemp.transform.localRotation = videoTemp.transform.localRotation;
                MediaPlayerCtrl media = videoTemp.GetComponent<MediaPlayerCtrl>();
                media.m_strFileName = "file://" + AppSettings.Instance.AppAssetbundlePaths + "temp.mp4";
                media.enabled = true;
                media.Play();

                Manager.Instance.GetScaneManager.SetContent(videoTemp);
                switch (Manager.Instance.GetScaneManager.ContentStatus)
                {
                    case TrackerContentStatus.OnTracker:
                        videoTemp.GetComponent<ShowerSetting>().FixedAngleByFinding();

                        break;
                    case TrackerContentStatus.LoseTracker:
                        videoTemp.GetComponent<ShowerSetting>().FixedAngleByLosting();

                        break;
                }

                break;
            case BundleType.Mixed:
                break;
        }

        yield return new WaitForSeconds(0.5f);

        //暂存当前生成的内容对象数据流，用于写入本地
        GetCurContentBytes = new byte[bytes.Length];
        bytes.CopyTo(GetCurContentBytes, 0);

        Manager.Instance.GetUIManager.menuBtn.interactable = true;
        yield return new WaitForSeconds(1.25f);
    }

    //设置手势控制轴
    private void MakeGestureAxis(GameObject go)
    {
        Manager.Instance.GetScaneManager.SetContent(go);
        //控制设置
        Manager.Instance.mTarget = new GestureParater()
        {
            posistion = this.posistion,
            rotation = this.rotation,
            scale = this.scale
        };
        Manager.Instance.SetGestureCtrlTargets();
    }
}

