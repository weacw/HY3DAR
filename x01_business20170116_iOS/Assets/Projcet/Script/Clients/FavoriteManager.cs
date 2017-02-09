
/******
创建人：NSWell
用途：收藏夹管理器
******/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RequestDataResults;
using UnityEngine.Events;
using UnityEngine.UI;

public class FavoriteManager : MonoBehaviour
{
    //运行时生成的收藏对象ui
    public Dictionary<string, GameObject> favoriteGameObjects = new Dictionary<string, GameObject>();
    //本地收藏数据库读取到的数据
    public Dictionary<string, Assets> favoriteAssets = new Dictionary<string, Assets>();
    private QueryDatasFromLocal queryDatasFromLocal;
    internal Assets mAsset;
    private bool addingToFavorite;
    internal byte[] thumbnail;
    public void OnFavoriteDisplay()
    {
        ReadFavoriteDatas();
    }

    public void OnClearUp()
    {
        if (queryDatasFromLocal == null)
            queryDatasFromLocal = new QueryDatasFromLocal();
        queryDatasFromLocal.TruncateTable();
        mAsset = null;
        thumbnail = null;
        if (favoriteAssets == null || favoriteAssets.Count <= 0) return;
        favoriteAssets.Clear();

        foreach (KeyValuePair<string, GameObject> o in favoriteGameObjects)
        {
            Destroy(o.Value);
        }
        favoriteGameObjects.Clear();
        addingToFavorite = false;
    }
    internal void RemoveAtFavorite()
    {
        #region 取消当前收藏
        BundleType bt = BundleType.Mixed;
        if (favoriteAssets.Count > 0)
        {
            string name = Manager.Instance.GetScaneManager.GetCurContentName;
            if (favoriteAssets.ContainsKey(name))
                bt = favoriteAssets[name].GetBundleType();
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
        string curContentOnlocalPath = AppSettings.Instance.GetCurContentOnLocal();
        string curContentName = Manager.Instance.GetScaneManager.GetCurContentName;
        switch (bt)
        {
            case BundleType.Model:
                break;
            case BundleType.Video:
                curContentOnlocalPath = curContentOnlocalPath.Replace("assetbundle", "mp4");
                curContentName = curContentName.Replace("assetbundle", "mp4");
                break;
            case BundleType.Mixed:
                break;
        }

        string thunbil = AppSettings.Instance.GetCurContentThumbilOnLocal();
        thumbnail = FileOpeartion.GetFileOpeartion().ReadFileFromLocal(thunbil);
        if (!FileOpeartion.GetFileOpeartion().FileDelect(curContentOnlocalPath)) return;
        if (!FileOpeartion.GetFileOpeartion().FileDelect(thunbil)) return;
        ReadFavoriteItem(curContentName, ref mAsset);
        RemoveFavoriteDataOnLocal(curContentName);
        RemoveAtFavoriteAssets(curContentName);
        GameObject go = RemoveAtFavoriteGameobject(curContentName);
        if (null != go) Destroy(go);
        Manager.Instance.GetUIManager.removeAtFavoriteBtn.gameObject.SetActive(false);
        Manager.Instance.GetUIManager.addToFavoriteBtn.gameObject.SetActive(true);
        #endregion
    }
    //收藏与取消收藏操作
    internal void AddToFavorite()
    {
        #region 添加当前内容为收藏

        Manager.Instance.GetUIManager.menuBtn.interactable = false;
        Manager.Instance.GetUIManager.closeCurTargetUI.GetComponent<Button>().interactable = false;

        //当前呈现内容对象在本地的路径
        string curContentOnlocalPath = AppSettings.Instance.GetCurContentOnLocal();
        //判读当前呈现内容对象是否存在本地
        bool exists = FileOpeartion.GetFileOpeartion().CheckingFile(curContentOnlocalPath);
        string curContentName = Manager.Instance.GetScaneManager.GetCurContentName;
        if (!exists)
        {
            if (Manager.Instance.GetScaneManager.assets.Count > 0)
            {
                mAsset = new Assets()
                {
                    alias = Manager.Instance.GetScaneManager.assets[0].alias,
                    assetname = Manager.Instance.GetScaneManager.assets[0].assetname,
                    bundletype = Manager.Instance.GetScaneManager.assets[0].bundletype,
                    videourl = "file://" + curContentOnlocalPath,
                    androidUrl = "file://" + curContentOnlocalPath,
                    iosUrl = "file://" + curContentOnlocalPath,
                    thumbnails = AppSettings.Instance.GetCurContentThumbilOnLocal(),
                    version = Manager.Instance.GetScaneManager.assets[0].version
                };
            }
            else
            {
                ReadFavoriteItem(curContentName, ref mAsset);
            }
            if (thumbnail != null && thumbnail.Length > 0)
            {
                SaveThumbnailToLocal(thumbnail);
            }
            else
            {
                if (Manager.Instance.GetAllContentsManager.mClickElement == null)
                {
                    //缩略图地址，以及下载缩略图
                    string thumbil = Manager.Instance.GetScaneManager.assets[0].thumbnails;
                    InitWWWOperat(thumbil);
                }
                else
                {
                    Texture2D t2d = Manager.Instance.GetAllContentsManager.mClickElement.mThunmbilImage.texture as Texture2D;
                    if (mAsset == null)
                    {
                        AllContentsManager acm = Manager.Instance.GetAllContentsManager;
                        Assets asset = acm.contentAssets[acm.mClickElement.guid];
                        mAsset = new Assets()
                        {
                            alias = asset.alias,
                            assetname = asset.assetname,
                            bundletype = asset.bundletype,
                            videourl = asset.videourl,
                            androidUrl = "file://" + curContentOnlocalPath,
                            iosUrl = "file://" + curContentOnlocalPath,
                            thumbnails = AppSettings.Instance.GetCurContentThumbilOnLocal(),
                        };
                    }
                    if (t2d != null)
                        FileOpeartion.GetFileOpeartion()
                            .FileCreater(AppSettings.Instance.AppFavoriterPaths + "Thumbnails/" + mAsset.assetname + ".jpg", t2d.EncodeToJPG(), null);

                    SaveFavoriteDataOnLocal(mAsset);
                }
            }
            if (mAsset.GetBundleType() == BundleType.Video)
                curContentOnlocalPath = curContentOnlocalPath.Replace("assetbundle", "mp4");

            FileOpeartion.GetFileOpeartion()
                .FileCreater(curContentOnlocalPath, DownloadAbsNCreate.Instance.GetCurContentBytes, () =>
                {
                    Manager.Instance.GetUIManager.IsAddedToFavorite = true;
                });

        }
        #endregion
        Manager.Instance.GetUIManager.CalculaterCacheSize();
    }

    private void InitWWWOperat(string thumbil)
    {
        if (GeneralWWW.Instance.GetThumbilDownloader.DownloadOverSaveToLocal == null)
            GeneralWWW.Instance.GetThumbilDownloader.DownloadOverSaveToLocal += SaveThumbnailToLocal;
        IEnumerator it = GeneralWWW.Instance.GetThumbilDownloader.WWWDownloader(
            Manager.Instance.OnNetworkError, thumbil);
        GeneralWWW.Instance.OpenCoroutine(it);
    }

    //存储缩略图至本地
    private bool SaveThumbnailToLocal(byte[] bytes)
    {
        SaveFavoriteDataOnLocal(mAsset);
        string localPath = AppSettings.Instance.GetCurContentThumbilOnLocal();
        return FileOpeartion.GetFileOpeartion().FileCreater(localPath, bytes, null);
    }

    //创建收藏item ui
    private void CreatingFavoriteItems(Texture2D thumbails, string fileName)
    {
        #region 创建收藏UI控件

        if (favoriteGameObjects.ContainsKey(fileName)) return;
        GameObject go = Instantiate(Manager.Instance.GetUIManager.favoritePrefab);
        ContentsItemElement cie = go.GetComponent<ContentsItemElement>();
        Transform mTransform = go.transform;
        mTransform.SetParent(Manager.Instance.GetUIManager.favoriteRoot);
        mTransform.localScale = Vector3.one;
        mTransform.localPosition = Vector3.zero;
        mTransform.localRotation = Quaternion.identity;
        #endregion

        //TODO:此处数据加入位置放错，应该放到AddItemToFavorite
        #region 初始化数据

        Assets asset = GetItemFromFavoriteAssets(fileName);
        if (null == asset) return;
        cie.mAssetNameText.text = asset.alias;
        cie.mThunmbilImage.texture = thumbails;
        string path = null;
        switch (asset.GetBundleType())
        {
            case BundleType.Model:
                if (Application.platform == RuntimePlatform.Android)
                    path = asset.androidUrl;
                else
                    path = asset.androidUrl;

                break;
            case BundleType.Video:
                path = asset.videourl;
                break;
            case BundleType.Mixed:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        AddEventButton(cie.mDownloadBtn, () => StartCoroutine(WaitScendsToLoad(path)));
        #endregion
        AddItemToFavoriteGameobject(asset.assetname, go);
    }

    private IEnumerator WaitScendsToLoad(string path)
    {
        yield return new WaitForSeconds(0.125f);
        DownloadAbsNCreate.Instance.DownLoad(path);
    }
    #region sqlite操作
    //保存收藏数据至本地数据库
    private void SaveFavoriteDataOnLocal(Assets asset)
    {
        if (null == queryDatasFromLocal)
            queryDatasFromLocal = new QueryDatasFromLocal();
        queryDatasFromLocal.InsertFavoriteData(asset);
        OnFavoriteDisplay();
    }

    //从本地数据库中移除key为name的asset数据
    private void RemoveFavoriteDataOnLocal(string name)
    {
        if (null == queryDatasFromLocal) queryDatasFromLocal = new QueryDatasFromLocal();
        queryDatasFromLocal.DelectFavoriteData(name);
    }

    //从本地数据库中读取出key为curContentName的asset数据
    private void ReadFavoriteItem(string curContentName, ref Assets asset)
    {
        if (null == queryDatasFromLocal) queryDatasFromLocal = new QueryDatasFromLocal();
        queryDatasFromLocal.GetCurFavoriteItemData(curContentName, ref asset);
    }
    #endregion

    //代理-给按钮添加事件
    private void AddEventButton(Button btn, UnityAction act)
    {
        btn.onClick.AddListener(act);
        btn.onClick.AddListener(() => Manager.Instance.GetUIManager.ClickFavoriteEvent());
    }

    //读取收藏的数据
    private void ReadFavoriteDatas()
    {
        #region 读取本地数据库
        if (null == queryDatasFromLocal)
            queryDatasFromLocal = new QueryDatasFromLocal();
        queryDatasFromLocal.GetFavoriteDatas(ref favoriteAssets);
        #endregion
        StartCoroutine(WaitAMinute());
    }

    //缓冲
    private IEnumerator WaitAMinute()
    {
        yield return new WaitForSeconds(0.25f);
        foreach (var asset in favoriteAssets)
        {
            #region 加载本地缩略图，并创建UI-item
            byte[] bytes = FileOpeartion.GetFileOpeartion().ReadFileFromLocal(asset.Value.thumbnails);
            Texture2D texture = new Texture2D(256, 256);
            texture.LoadImage(bytes);
            texture.Apply();

            if (favoriteAssets.Count > 0)
                CreatingFavoriteItems(texture, asset.Value.assetname);
            else
            {
                //TODO:显示无收藏提示
            }

            #endregion
        }
    }

    //添加item到收藏字典缓存
    private void AddItemToFavoriteGameobject(string key, GameObject targetItem)
    {
        if (favoriteGameObjects.ContainsKey(key)) return;
        favoriteGameObjects.Add(key, targetItem);
    }

    //将item移除出收藏字典缓存并返回被移除的对象
    private GameObject RemoveAtFavoriteGameobject(string key)
    {
        //TODO:此处key有异常
        if (!favoriteGameObjects.ContainsKey(key)) return null;
        GameObject go = favoriteGameObjects[key];
        favoriteGameObjects.Remove(key);
        return go;
    }

    //将asset数据存入收藏字典
    private Assets GetItemFromFavoriteAssets(string key)
    {
        if (!favoriteAssets.ContainsKey(key)) return null;
        return favoriteAssets[key];
    }

    //将收藏字典中key=key的asset数据移除
    private void RemoveAtFavoriteAssets(string key)
    {
        if (!favoriteAssets.ContainsKey(key)) return;
        favoriteAssets.Remove(key);
    }
}
