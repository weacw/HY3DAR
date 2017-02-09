

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 *Description:用户界面交互逻辑类
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using cn.sharesdk.unity3d;
using DG.Tweening;
using RequestDataResults;
using UnityEngine;
using UnityEngine.UI;

public enum ShareType
{
    Image = 2,
    Recording = 6
};
public class UserInterfaceManager : MonoBehaviour
{
    private ShareType curShareType;
    [Header("Download Abs UI")]
    public CanvasGroup progressGroup_Abs;
    public Image progressPercentImage_Abs;
    public Text progressPercentText_Abs;
    public bool GetProgressAbsStatus { get; private set; }

    [Header("Download Target UI")]
    public CanvasGroup progressGroup_Targets;
    public Image progressPercentImage_Targets;
    public Text progressPercentText_Targets;



    [Header("Float window - Trackers downloader error")]
    public GameObject floatWindowOfError;
    public Button canel;
    public Button tryAgain;
    public bool GetProgressTargetsStatus { get; private set; }


    [Header("Scanning UI")]
    public GameObject scanningTipUI;
    public GameObject closeCurTargetUI;
    public GameObject restCtrlUI;
    public GameObject bottomUI;
    public GameObject swapCameraUI;
    public Button addToFavoriteBtn;
    public Button removeAtFavoriteBtn;
    public Button menuBtn;

    [Header("Favorite UI")]
    public GameObject favoritePrefab;
    public Transform favoriteRoot;

    [Header("Screen shot UI")]
    public GameObject uiRoot;
    public bool isShare = true;
    [Header("Recording UI")]
    public Button startRecording;
    public Button stopRecording;
    public Image recordingProgress;
    public Text recordingText;
    private DateTime dt;
    [Header("Share UI")]
    public GameObject shareView;
    public RawImage screenImage;
    public RawImage videoImage;
    public Button shareQQ;
    public Button shareSina;
    public Button shareWechatFriends;
    public Button shareWechatMoments;
    public Button saveLocal;
    public MediaPlayerCtrl mediaPlayer;


    public GameObject leftMenu;
    public GameObject leftMenuMask;
    public GameObject favoriteTips;

    public GameObject newVersioinMark;
    public GameObject newVersionText;
    public Text curCacheSizeText;

    public Button updateVersion;
    public Button clearupCache;
    public List<GameObject> panels = new List<GameObject>();

    private bool isOpenView;
    [SerializeField]
    private bool isNewVersion;
    public GameObject loadMoreBtn;
    public bool IsAddedToFavorite { get; set; }

    public void Awake()
    {
        shareQQ.onClick.AddListener(() => Share(PlatformType.QQ));
        shareSina.onClick.AddListener(() => Share(PlatformType.SinaWeibo));
        shareWechatMoments.onClick.AddListener(() => Share(PlatformType.WeChatMoments));
        shareWechatFriends.onClick.AddListener(() => Share(PlatformType.WeChat));
        saveLocal.onClick.AddListener(SaveLocal);
        clearupCache.onClick.AddListener(() => ClearCache(new DirectoryInfo(AppSettings.Instance.AppFavoriterPaths)));
        clearupCache.onClick.AddListener(() => ClearCache(new DirectoryInfo(AppSettings.Instance.AppAssetbundlePaths)));

        updateVersion.onClick.AddListener(UpdateVersion);
        CalculaterCacheSize();
    }
    //当收藏完毕后调用此方法
    internal void AddedToFavorite()
    {
        addToFavoriteBtn.gameObject.SetActive(false);
        removeAtFavoriteBtn.gameObject.SetActive(true);
        IsAddedToFavorite = false;
    }
    internal void CalculaterCacheSize()
    {
        long sizeOfFavoriter =
            AppSettings.Instance.GetFavoriteCacheSize(new DirectoryInfo(AppSettings.Instance.AppFavoriterPaths));
        long sizeOfAbs =
            AppSettings.Instance.GetFavoriteCacheSize(new DirectoryInfo(AppSettings.Instance.AppAssetbundlePaths));
        //计算当前收藏缓存的大小
        curCacheSizeText.text = ((sizeOfAbs + sizeOfFavoriter) / Mathf.Pow(1024, 2)).ToString("0.00") + "M";
    }

    //判断当前呈现内容是否加入收藏夹
    internal bool CurContentIsAddToFavorite()
    {
        FavoriteManager fm = FindObjectOfType<FavoriteManager>();
        BundleType bt = BundleType.Mixed;
        if (fm.favoriteAssets.Count > 0)
        {
            string name = Manager.Instance.GetScaneManager.GetCurContentName;
            if (fm.favoriteAssets.ContainsKey(name))
                bt = fm.favoriteAssets[name].GetBundleType();
        }
        else if (Manager.Instance.GetScaneManager.assets.Count > 0)
        {
            bt = Manager.Instance.GetScaneManager.assets[0].GetBundleType();
        }
        else if (Manager.Instance.GetAllContentsManager.mClickElement != null)
        {
            int id = Manager.Instance.GetAllContentsManager.mClickElement.guid;
            bt = Manager.Instance.GetAllContentsManager.contentAssets[id].GetBundleType();
        }
        string path = null;
        switch (bt)
        {
            case BundleType.Model:
                path = AppSettings.Instance.GetCurContentOnLocal();
                break;
            case BundleType.Video:
                path =
                      AppSettings.Instance.GetCurContentOnLocal().Replace("assetbundle", "mp4");
                break;
            case BundleType.Mixed:
                break;
        }
        bool exists = FileOpeartion.GetFileOpeartion().CheckingFile(path);
        if (exists)
        {
            removeAtFavoriteBtn.gameObject.SetActive(true);
            addToFavoriteBtn.gameObject.SetActive(false);
        }
        return exists;
    }

    //打开侧边菜单
    public void OpenLeftMenu()
    {
        menuBtn.gameObject.SetActive(false);
        swapCameraUI.gameObject.SetActive(false);

        CanvasGroup cg = leftMenu.GetComponent<CanvasGroup>();
        cg.DOFade(1, 0.55f);
        leftMenu.gameObject.SetActive(true);
        Tween tw = leftMenu.transform.DOMoveX(0, 0.4f);
        tw.SetEase(Ease.OutCirc);
        Image tempMask = leftMenuMask.GetComponent<Image>();
        tw.OnStart(() =>
        {
            leftMenuMask.SetActive(true);
            tempMask.DOFade(1, 0.4f);
            scanningTipUI.SetActive(false);
            MakeTopUIStatus(false);
            Manager.Instance.GetScaneManager.RemoveTrackable();
        });
        Manager.Instance.GetScaneManager.RemoveTrackable();
        Manager.Instance.GetScaneManager.CtrlOtherTracker(null, TRACKSENABLESTATUS.STOPALL);
    }

    //关闭侧边菜单 -showScanTip->显示扫描UI
    public void CloseLeftMenu(bool showScanTips)
    {
        menuBtn.gameObject.SetActive(true);
        CanvasGroup cg = leftMenu.GetComponent<CanvasGroup>();
        cg.DOFade(0, 0.7f);
        Tween tw = leftMenu.transform.DOMoveX(-Screen.width, 0.55f);
        tw.SetEase(Ease.InCirc);
        Image tempMask = leftMenuMask.GetComponent<Image>();
        tw.OnStart(() =>
        {
            tempMask.DOFade(0, 0.7f);
        });
        tw.OnComplete(() =>
        {
            leftMenuMask.SetActive(false);
            if (isOpenView) return;
            scanningTipUI.SetActive(showScanTips);
            swapCameraUI.gameObject.SetActive(true);
            Manager.Instance.GetScaneManager.CtrlOtherTracker(null, TRACKSENABLESTATUS.PLAYALL);
        });
    }

    //返回Home
    public void GoToHomePage()
    {
        CloseExceptForurself(null);
        CloseLeftMenu(true);
        swapCameraUI.SetActive(true);
        isOpenView = false;
    }

    public void OpenAllContentsPanel()
    {        
        OpenUIPanel(panels[0]);
    }

    //通过侧边菜单开启收藏夹的操作
    public void OpenFavorite()
    {
        OpenUIPanel(panels[1]);
        //显示提示，无收藏内容
        favoriteTips.SetActive(favoriteRoot.childCount <= 0);
    }


    //打开设置面板
    public void OpenSettingPanel()
    {
        CalculaterCacheSize();
        OpenUIPanel(panels[2]);
    }

    //打开帮助面板
    public void OpenHelperPanel()
    {
        OpenUIPanel(panels[3]);
    }
    public void OpenHelper_A()
    {
        OpenUIPanel(panels[5]);
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
    public void OpenHelper_B()
    {
        OpenUIPanel(panels[6]);
    }
    public void OpenHelper_C()
    {
        OpenUIPanel(panels[7]);
    }
    //打开关于面板
    public void OpenAboutPanel()
    {
        OpenUIPanel(panels[4]);
    }



    //开启用户面板
    private void OpenUIPanel(GameObject panelNeedToBeOpened)
    {
        //关闭左侧菜单面板
        CloseLeftMenu(false);
        CloseExceptForurself(panelNeedToBeOpened);
        //开启panelNeedToBeOpened面板
        panelNeedToBeOpened.transform.DOLocalMoveX(0, 0.2f);
        panelNeedToBeOpened.GetComponent<CanvasGroup>().DOFade(1, 0.1f);
        //面板是否打开
        isOpenView = true;
    }



    //检查版本
    public void CheckVersion()
    {
        float version_New = float.Parse(Manager.Instance.GetAppVersionCtrl.GetAppversion.appVersion);
        if (PlayerPrefs.GetFloat("CurAppVersion") < version_New)
        {
            isNewVersion = true;
            newVersionText.GetComponent<Text>().text = "有新版本可用";
            newVersioinMark.SetActive(true);
            newVersionText.SetActive(true);
        }
        else
        {
            isNewVersion = false;
            newVersionText.SetActive(true);
            newVersionText.GetComponent<Text>().text = "当前已是最新版本";
        }
    }
    private void UpdateVersion()
    {
        //Manager.Instance.GetAppVersionCtrl.Start();
        if (isNewVersion)
            Application.OpenURL(Manager.Instance.GetAppVersionCtrl.GetAppversion.appUrl);
        else
        {
            FindObjectOfType<CallNativeFun>().ShowNativeToast("当前已是最新版本");
        }
    }


    //关闭除了当前要开启的界面以外的所有界面
    internal void CloseExceptForurself(GameObject curPanelTarget)
    {
        foreach (GameObject panel in panels)
        {
            if (!panel.activeInHierarchy || panel.Equals(curPanelTarget)) continue;
            panel.transform.DOLocalMoveX(Screen.width, 0.65f);
            panel.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        }
    }

    public void DrogToLoad()
    { 
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            FindObjectOfType<CallNativeFun>().ShowNativeToast("网络不给力，请检查网络设置。");
            loadMoreBtn.gameObject.SetActive(true);
            return;
        }


        Manager.Instance.GetAllContentsManager.GetDbQueryAssetsOnAllContents.SendMysqlQuery(null);
    }


    //清空缓存
    private void ClearCache(DirectoryInfo info)
    {
        FileInfo[] files = info.GetFiles();
        foreach (FileInfo file in files)
        {
            FileOpeartion.GetFileOpeartion().FileDelect(file.FullName);
        }
        foreach (DirectoryInfo directory in info.GetDirectories())
        {
            files = directory.GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                FileOpeartion.GetFileOpeartion().FileDelect(fileInfo.FullName);
            }
        }
        FindObjectOfType<FavoriteManager>().OnClearUp();
        Manager.Instance.GetAllContentsManager.OnClearUp();
        //计算当前收藏缓存的大小
        curCacheSizeText.text = (AppSettings.Instance.GetFavoriteCacheSize(new DirectoryInfo(AppSettings.Instance.AppFavoriterPaths)) / Mathf.Pow(1024, 2)).ToString("0.00") + "M";
    }



    private void MakeTopUIStatus(bool status)
    {
        closeCurTargetUI.SetActive(status);
        bottomUI.SetActive(status);
        restCtrlUI.SetActive(status);
        if (!panels[1].activeInHierarchy)
            swapCameraUI.SetActive(!status);
    }

    #region User interaction
    //右上角 关闭内容按钮的操作
    public void OnDestroyBtn(bool status)
    {
        if (Manager.Instance.GetScaneManager.GetContent == null) return;
        if (status)
        {
            Manager.Instance.GetScaneManager.CtrlOtherTracker(null, TRACKSENABLESTATUS.PLAYALL);
            OnTrackerUIEnvet(false);
        }
        else
        {
            Manager.Instance.GetScaneManager.CtrlOtherTracker(null, TRACKSENABLESTATUS.STOPALL);
            closeCurTargetUI.SetActive(false);
            bottomUI.SetActive(false);
            restCtrlUI.SetActive(false);
            swapCameraUI.SetActive(true);
        }
        removeAtFavoriteBtn.gameObject.SetActive(false);
        addToFavoriteBtn.gameObject.SetActive(true);

        if (Manager.Instance.GetRecordingModul.IsStartRecord)
            Everyplay.StopRecording();

        Manager.Instance.GetScaneManager.RemoveTrackable();
        Manager.Instance.RestCtrl();
        Manager.Instance.mTarget = null;
        GC.Collect();
    }

    //收藏已经呈现内容的操作
    public void AddToFavorite()
    {
        //TODO:添加入收藏夹
        FindObjectOfType<FavoriteManager>().AddToFavorite();
    }
    //取消收藏已经呈现内容的操作
    public void RemoveAtFavorite()
    {
        //TODO:从收藏夹移除
        FindObjectOfType<FavoriteManager>().RemoveAtFavorite();
    }


    public void Screenshot()
    {
        Manager.Instance.GetScreenshotModul.ModulsOperat();
    }
    #endregion

    #region Download Assetbundle ui

    //显示下载Assetbundle进度条
    internal void ShowAbsProgress()
    {
        Manager.Instance.GetUIManager.menuBtn.interactable = false;
        GetProgressAbsStatus = true;
        progressGroup_Abs.gameObject.SetActive(true);
        Tween tween = progressGroup_Abs.DOFade(1, 0.25f);
        tween.SetEase(Ease.InQuad);
        tween.SetAutoKill(true);
    }

    //隐藏下载Assetbundle进度条
    internal void HideAbsProgress()
    {
        GetProgressAbsStatus = false;
        Tween tween = progressGroup_Abs.DOFade(0, 0.25f);
        tween.SetEase(Ease.InQuad);
        tween.SetAutoKill(true);
        tween.OnComplete(() =>
        {
            progressPercentImage_Abs.fillAmount = 0;
            progressPercentText_Abs.text = null;
            progressGroup_Abs.gameObject.SetActive(false);
        });
        if (!Manager.Instance.HappenError)
            OnTrackerUIEnvet(true);

        StopAllCoroutines();
    }

    #endregion

    #region Download trackers ui

    internal void ShowTrackersProgress()
    {
        GetProgressTargetsStatus = true;
        progressGroup_Targets.gameObject.SetActive(true);
        Tween tween = progressGroup_Targets.DOFade(1, 0.05f);
        tween.SetEase(Ease.OutCirc);
        tween.SetAutoKill(true);
    }

    internal void HideTrackersProgress()
    {
        Tween tween = progressGroup_Targets.DOFade(0, 0.05f);
        tween.SetEase(Ease.OutCirc);
        tween.SetAutoKill(true);
        tween.OnComplete(() =>
        {
            GetProgressTargetsStatus = false;
            progressPercentImage_Targets.fillAmount = 0;
            progressPercentText_Targets.text = null;
            progressGroup_Targets.gameObject.SetActive(false);
        });
        if (Application.isPlaying)
            Manager.Instance.GetUIManager.scanningTipUI.SetActive(true);
    }

    #endregion

    #region Screen shot ui
    internal void OnScreenshoted()
    {
        curShareType = ShareType.Image;
        uiRoot.SetActive(true);
        if (!isShare) return;
        saveLocal.gameObject.SetActive(false);
        shareView.SetActive(true);
        Texture2D image = Manager.Instance.GetScreenshotModul.GetImage;
        if (image == null) return;
        screenImage.gameObject.SetActive(true);
        screenImage.texture = Manager.Instance.GetScreenshotModul.GetImage;
        FindObjectOfType<CallNativeFun>().SaveImageNativeFun(Manager.Instance.GetScreenshotModul.TempFileNameFull, "Image", "照片已存至相册");
    }

    internal void OnScreenshotting()
    {
        uiRoot.SetActive(false);
    }
    #endregion

    #region Recording ui

    public void StartRecording()
    {
        curShareType = ShareType.Recording;
        Manager.Instance.GetRecordingModul.ModulsOperat();
        Record(true);
    }
    public void StopRecording()
    {
        Manager.Instance.GetRecordingModul.ModulsOperat();
        Record(false);
    }

    internal void HideRecording()
    {
        Record(false);

        if (!isShare) return;
        shareView.SetActive(true);
        saveLocal.gameObject.SetActive(true);
        if (mediaPlayer != null)
            mediaPlayer.Load("file://" + Manager.Instance.GetRecordingModul.tempFullName);
        videoImage.transform.parent.gameObject.SetActive(true);
        Tween tw = videoImage.DOColor(Color.white, 1);
        tw.SetDelay(1.2f);
        tw.OnComplete(() =>
        {
            mediaPlayer.Pause();
            mediaPlayer.Stop();
            mediaPlayer.SetVolume(1);
            GameObject.Find("Play Mark").gameObject.SetActive(true);
        });
        //FindObjectOfType<CallNativeFun>().ShowNativeToast("录像已存储");
    }
    private void Record(bool status)
    {
        recordingProgress.fillAmount = 0;
        recordingText.text = null;
        startRecording.gameObject.SetActive(!status);
        stopRecording.gameObject.SetActive(status);
        recordingText.gameObject.SetActive(status);
    }

    public void Recording(float progress)
    {
        recordingProgress.fillAmount = progress / 15;
        recordingText.text = string.Format("00:{0:00}/00:15", Mathf.RoundToInt(progress));
    }
    #endregion

    #region Share ui
    public void OnCloseShareView()
    {
        AudioListener.volume = 1;

        shareView.SetActive(false);
        saveLocal.gameObject.SetActive(true);
        videoImage.transform.parent.gameObject.SetActive(false);
        screenImage.gameObject.SetActive(false);
        if (screenImage.texture != null)
        {
            Destroy(screenImage.texture);
            screenImage.texture = null;
        }
        if (videoImage.texture != null)
        {
            Destroy(videoImage.texture);
            videoImage.texture = null;
            videoImage.DOColor(Color.clear, 0.1f);
        }
    }

    public void Share(PlatformType platform)
    {
        ShareModuls.contentValue cv = new ShareModuls.contentValue();

        switch (curShareType)
        {
            case ShareType.Image:
                cv.imagePath = Manager.Instance.GetScreenshotModul.TempFileNameFull;
                break;
            case ShareType.Recording:
                if (!Manager.Instance.GetRecordingModul.IsUploaded)
                {
                    FindObjectOfType<CallNativeFun>().ShowNativeToast("正在上传，请稍后再分享。");
                    return;
                }
                cv.text =
                    "海源三维AR视频-海源三维AR是一款可以展示最新互动AR的APP，在这里你可以通过扫描识别图任意查看海源三维的系列产品，包含了中小学创客、中高职教育、3D打印模型、3D扫描模型等，体验虚拟与现实的实时互动奇景，拥有更多模型、场景和新玩法。";
                cv.imagePath = AppSettings.Instance.AppDataPaths + "/Icon.jpg";
                cv.title = "海源三维AR分享";
                cv.videoUrl = AppSettings.Instance.clientGlobalConfigs.serverCofing.serverIPs + "unity/video.php?id=" +
                              Manager.Instance.GetRecordingModul.CurVideoName;
                break;
        }
        Manager.Instance.GetShareModuls.ShareContents(cv, platform, (int)curShareType);
    }

    public void SaveLocal()
    {
        byte[] bytes = FileOpeartion.GetFileOpeartion().ReadFileFromLocal(Manager.Instance.GetRecordingModul.TempFileNameFull);
        FileOpeartion.GetFileOpeartion()
            .FileCreater(AppSettings.Instance.AppVideoPaths + Manager.Instance.GetRecordingModul.CurVideoName, bytes,null);        
        StartCoroutine(SaveVideoSuccess());
    }

    private IEnumerator SaveVideoSuccess()
    {
        yield return new WaitForSeconds(0.5f);
        FindObjectOfType<CallNativeFun>().ShowNativeToast("视频已存至本地");
        FindObjectOfType<CallNativeFun>()
            .ScanFile(AppSettings.Instance.AppVideoPaths + Manager.Instance.GetRecordingModul.CurVideoName);
    }

    #endregion

    internal void UpdateAbsProgressData(float progress)
    {
        progressPercentImage_Abs.fillAmount = progress;
        progressPercentText_Abs.text = Mathf.RoundToInt(progress * 100) + "%";
    }

    internal void UpdateTrackersProgressData(float progress)
    {
        progressPercentImage_Targets.fillAmount = progress;
        progressPercentText_Targets.text = Mathf.RoundToInt(progress * 100).ToString();
    }

    internal void FloatWindowTips()
    {
        floatWindowOfError.SetActive(true);
        canel.onClick.AddListener(() => floatWindowOfError.SetActive(false));
#if ANDROID
                canel.onClick.AddListener(Application.Quit);
#endif
        tryAgain.onClick.AddListener(() => Manager.Instance.GetTargetsZip.Download());
        tryAgain.onClick.AddListener(() => floatWindowOfError.SetActive(false));
        HideTrackersProgress();
    }

    internal void OnTrackerUIEnvet(bool status)
    {
        scanningTipUI.SetActive(!status);
        swapCameraUI.SetActive(!status);

        closeCurTargetUI.SetActive(status);
        bottomUI.SetActive(status);
        restCtrlUI.SetActive(status);
    }

    internal void ClickFavoriteEvent()
    {
        panels[1].transform.DOLocalMoveX(Screen.width, 0.25f);
        panels[1].GetComponent<CanvasGroup>().DOFade(0, 0.25f);
        //panels[1].transform.localPosition = new Vector3(Screen.width, panels[1].transform.localPosition.y, 0);
    }
}