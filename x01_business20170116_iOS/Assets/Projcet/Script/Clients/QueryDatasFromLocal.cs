/****
创建人：NSWell
用途：检查收藏夹数据库
******/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using RequestDataResults;

public class QueryDatasFromLocal
{
    private SqliteHelper sqlite;

    //获取收藏夹数据
    public void GetFavoriteDatas(ref Dictionary<string, Assets> favoriteAssets)
    {        
        if (null == sqlite)
            sqlite = new SqliteHelper("URI=file:" + AppSettings.Instance.AppDataPaths + "/weacwDB.db");
        SqliteDataReader sqliteReader = sqlite.ReadFullTable("assets");
        while (sqliteReader.Read())
        {
            Assets asset = new Assets()
            {
                id = int.Parse(sqliteReader["id"].ToString()),
                alias = sqliteReader["alias"].ToString(),
                assetname = sqliteReader["assetname"].ToString(),
                bundletype = sqliteReader["bundletype"].ToString(),
                videourl = sqliteReader["videourl"].ToString(),
                androidUrl = sqliteReader["androidUrl"].ToString(),
                iosUrl = sqliteReader["iosUrl"].ToString(),
                thumbnails = sqliteReader["Thumbnails"].ToString(),
                version = float.Parse(sqliteReader["version"].ToString()),
            };
            if (!favoriteAssets.ContainsKey(asset.assetname))
                favoriteAssets.Add(asset.assetname, asset);
        }
    }

    //获取当前收藏夹数据
    public void GetCurFavoriteItemData(string queryName, ref Assets asset)
    {
        //curContentName
        if (null == sqlite)
            sqlite = new SqliteHelper("URI=file:" + AppSettings.Instance.AppDataPaths + "/weacwDB.db");
        SqliteDataReader sqliteReader = sqlite.SelectWhere("assets", new[] { "*" }, new[] { "assetname" }, new[] { "=" },
            new string[] { queryName });
        while (sqliteReader.Read())
        {
            asset = new Assets()
            {
                id = int.Parse(sqliteReader["id"].ToString()),
                alias = sqliteReader["alias"].ToString(),
                assetname = sqliteReader["assetname"].ToString(),
                bundletype = sqliteReader["bundletype"].ToString(),
                videourl = sqliteReader["videourl"].ToString(),
                androidUrl = sqliteReader["androidUrl"].ToString(),
                iosUrl = sqliteReader["iosUrl"].ToString(),
                thumbnails = sqliteReader["Thumbnails"].ToString(),
                version = float.Parse(sqliteReader["version"].ToString()),
            };
        }
    }

    //删除收藏
    public void DelectFavoriteData(string name)
    {
        if (null == sqlite)
            sqlite = new SqliteHelper("URI=file:" + AppSettings.Instance.AppDataPaths + "/weacwDB.db");
        string[] cols = new[] { "assetname" };
        string[] colsValue = new[] { name };
        sqlite.Delete("assets", cols, colsValue);
    }

    //增加收藏
    public void InsertFavoriteData(Assets asset)
    {
        if (null == sqlite)
            sqlite = new SqliteHelper("URI=file:" + AppSettings.Instance.AppDataPaths + "/weacwDB.db");
        string[] cols = new[] { "id", "alias", "assetname", "bundletype", "videourl", "androidUrl", "iosUrl", "thumbnails", "version" };
        string[] colsValue = new[]
        {
            asset.id.ToString(),
            asset.alias,
            asset.assetname,
            asset.bundletype,
            asset.videourl,
            asset.androidUrl,
            asset.iosUrl,
            asset.thumbnails,
            asset.version.ToString()
        };
        sqlite.InsertIntoSpecific("assets", cols, colsValue);
    }

    //清空收藏
    public void TruncateTable()
    {
        if (null == sqlite)
            sqlite = new SqliteHelper("URI=file:" + AppSettings.Instance.AppDataPaths + "/weacwDB.db");
        sqlite.ExecuteQuery("delete from assets");
    }
}
