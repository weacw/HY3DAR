/******
创建人：NSWell
用途：所有内容管理器
******/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RequestDataResults;
using UnityEngine.UI;

public class AllContentsManager : MonoBehaviour
{
    public GameObject favoritePrefab;
    public Transform contents;

    public List<Assets> contentAssets;
    public List<GameObject> allContents;
    public List<ContentsItemElement> contentsItemElements = new List<ContentsItemElement>();
    private Dictionary<string, Assets> contentAssetsDic = new Dictionary<string, Assets>();
    public DBQuery_Assets_AllContents GetDbQueryAssetsOnAllContents { get; private set; }
    public int Index { get; private set; }
    public ContentsItemElement mClickElement;
    private WWW www;
    public float GetContentDownloadProgress { get; private set; }
    private int guid = 0;
    public bool isDownloading;
    private void Update()
    {
        if (!isDownloading && null != mClickElement
            && mClickElement.downloadProgressBar.fillAmount < 1
            && mClickElement.downloadProgressBar.fillAmount >= 0.9f
            && GetContentDownloadProgress>0.95f
            || www!=null&& www.isDone )
            mClickElement.downloadProgressBar.fillAmount = 1;

        if (www == null)
            return;
        if (www.isDone && isDownloading)
            return;
        if (mClickElement.downloadProgressBar.fillAmount >= 0.99f)
            return;
        GetContentDownloadProgress = www.progress;
        mClickElement.downloadProgressBar.fillAmount = GetContentDownloadProgress / 1.0f;
        mClickElement.downloadProgressText.text = string.Format("下载：{0}%", Mathf.RoundToInt(GetContentDownloadProgress * 100));
    }
    public void Init()
    {
        GetDbQueryAssetsOnAllContents = new DBQuery_Assets_AllContents();
    }
    public void CreateFavoriteItem()
    {
        if (contentAssetsDic.ContainsKey(contentAssets[contentAssets.Count - 1].assetname))
        {
            FindObjectOfType<CallNativeFun>().ShowNativeToast("没有更多内容了");
        }
        foreach (Assets asset in contentAssets)
        {
            if (contentAssetsDic.ContainsKey(asset.assetname))
            {
                continue;
            }
            contentAssetsDic.Add(asset.assetname, asset);

            GameObject item = Instantiate(favoritePrefab, contents) as GameObject;

            //init item trasnform
            item.transform.localScale = Vector3.one;

            //init item data
            ContentsItemElement fie = item.GetComponent<ContentsItemElement>();

            fie.mAssetNameText.text = asset.alias;
            fie.guid = guid;

            string name = AppSettings.Instance.AppAssetbundlePaths + asset.assetname + ".assetbundle";
            if (asset.GetBundleType() == BundleType.Video)
                name = name.Replace("assetbundle", "mp4");
            bool fileStatus = FileOpeartion.GetFileOpeartion().CheckingFile(name);



            switch (asset.GetBundleType())
            {
                case BundleType.Model:
                    switch (Application.platform)
                    {
                        case RuntimePlatform.Android:
                            AddDownloadEvent(fie.mDownloadBtn, asset.androidUrl, asset.assetname);
                            break;
                        case RuntimePlatform.IPhonePlayer:
                            AddDownloadEvent(fie.mDownloadBtn, asset.iosUrl, asset.assetname);
                            break;
                    }
#if UNITY_EDITOR
                    AddDownloadEvent(fie.mDownloadBtn, asset.androidUrl, asset.assetname);
                    if (fileStatus)
                    {
                        fie.dontDownloadMask.gameObject.SetActive(false);
                        fie.downloadProgressText.gameObject.SetActive(false);
                    }
#endif
                    break;
                case BundleType.Video:
                    AddDownloadEvent(fie.mDownloadBtn, asset.videourl, asset.assetname);
                    break;
                case BundleType.Mixed:
                    break;
            }

            if (fileStatus)
            {
                fie.downloadProgressBar.fillAmount = 1;
                fie.dontDownloadMask.gameObject.SetActive(false);
                fie.downloadProgressText.gameObject.SetActive(false);
            }
            allContents.Add(item);

            WWW www = new WWW(asset.thumbnails);
            StartCoroutine(LoadTexture(www, fie));
            guid++;
            contentsItemElements.Add(fie);
        }
        Index += 16;
        Manager.Instance.GetUIManager.loadMoreBtn.SetActive(true);
    }


    public void AddDownloadEvent(Button btn, string path, string fileName)
    {
        btn.onClick.AddListener(() =>
        {
            mClickElement = btn.GetComponentInParent<ContentsItemElement>();
            ButtonEvent(path, fileName);
        });
    }

    private void SetBtnInteraction(bool status)
    {
        foreach (ContentsItemElement element in contentsItemElements)
        {
            element.mDownloadBtn.interactable = status;
        }
    }
    private void ButtonEvent(string path, string fileName)
    {
        bool fileStatus = false;
        string name = AppSettings.Instance.AppAssetbundlePaths + fileName;
        switch (contentAssets[mClickElement.guid].GetBundleType())
        {
            case BundleType.Model:
                name += ".assetbundle";
                break;
            case BundleType.Video:
                name += ".mp4";
                break;
            case BundleType.Mixed:
                break;
        }

        fileStatus = FileOpeartion.GetFileOpeartion().CheckingFile(name);
        if (!fileStatus)
        {
            SetBtnInteraction(false);
            Download(path, fileName);
        }
        else
        {
            DownloadAbsNCreate.Instance.DownLoad("file://" + name);
            Manager.Instance.GetUIManager.CloseExceptForurself(null);
        }
    }
    public void OnClearUp()
    {
        foreach (ContentsItemElement element in contentsItemElements)
        {
            element.dontDownloadMask.gameObject.SetActive(true);
            element.dontDownloadMask.DOFade(1, 0.25f);
            element.downloadProgressBar.fillAmount = 0;
            element.downloadProgressText.text = null;
            element.downloadProgressText.DOFade(1, 0.25f);
            element.downloadProgressText.gameObject.SetActive(true);
        }
    }
    private void Download(string path, string fileName)
    {
        if (!isDownloading)
        {
            isDownloading = true;
            StartCoroutine(DownloadAbs(path, fileName));
            return;
        }
        FindObjectOfType<CallNativeFun>().ShowNativeToast("有个下载任务正在进行，请稍等。");

    }

    private IEnumerator DownloadAbs(string path, string fileName)
    {
        www = new WWW(path);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
            www.Dispose();
            yield break;
        }
        while (!www.isDone)
        {
            yield return null;
        }
        string mLocalPath = null;
        if (www != null && www.bytes.Length > 2)
        {
            switch (contentAssets[mClickElement.guid].GetBundleType())
            {
                case BundleType.Model:
                    mLocalPath = AppSettings.Instance.AppAssetbundlePaths + fileName + ".assetbundle";
                    break;
                case BundleType.Video:
                    mLocalPath = AppSettings.Instance.AppAssetbundlePaths + fileName + ".mp4";
                    break;
                case BundleType.Mixed:
                    break;
            }
        }
        FileOpeartion.GetFileOpeartion()
    .FileCreater(mLocalPath, www.bytes, null);


        yield return new WaitForSeconds(1);
        www.Dispose();
        www = null;
        if (mClickElement.dontDownloadMask.gameObject.activeInHierarchy)
        {
            mClickElement.dontDownloadMask.DOFade(0, 0.3f).OnComplete(() =>
            {
                mClickElement.dontDownloadMask.gameObject.SetActive(false);
            });
        }
        if (mClickElement.downloadProgressText.gameObject.activeInHierarchy)
            mClickElement.downloadProgressText.DOFade(0, 0.3f).OnComplete(() =>
            {
                mClickElement.downloadProgressText.gameObject.SetActive(false);
                SetBtnInteraction(true);
            });
        isDownloading = false;
    }

    private IEnumerator LoadTexture(WWW www, ContentsItemElement item)
    {
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
            www.Dispose();
            yield break;
        }
        while (!www.isDone)
        {
            yield return null;
        }
        //init thunmbil
        item.mThunmbilImage.texture = www.texture;
    }
}
