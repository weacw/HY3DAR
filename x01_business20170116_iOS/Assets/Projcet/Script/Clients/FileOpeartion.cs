/******
创建人：NSWell
用途：文件操作模块
******/

using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;

public class FileOpeartion
{
    private static FileOpeartion _Instance;
    private FileOpeartion() { }


    public static FileOpeartion GetFileOpeartion()
    {
        return _Instance ?? (_Instance = new FileOpeartion());
    }
    //检查文件
    public bool CheckingFile(string path)
    {
        return File.Exists(path);
    }
    //创建文件
    public bool FileCreater(string path, byte[] bytes, Action mAction)
    {
        FileWriteThread fwt = new FileWriteThread()
        {
            mAction= mAction,
            savePath = path,
            bytes = bytes,
            OnCompleted = AbortThread
        };
        fwt.curThread = new Thread(fwt.OpeartionThread);
        fwt.curThread.Start();
        return fwt.CurStatus;
    }

    //删除文件
    public bool FileDelect(string path)
    {
        if (!CheckingFile(path)) return false;
        File.Delete(path);
        return true;
    }

    //查找并创建
    public bool FindWithCreateFile(string path, string savePath, string endwith)
    {
        FileFindWithCreate ffwc = new FileFindWithCreate()
        {
            savePath = savePath,
            findTarget = path,
            endwith = endwith,
            OnCompleted = AbortThread
        };
        ffwc.curThread = new Thread(ffwc.OpeartionThread);
        ffwc.curThread.Start();
        return ffwc.CurStatus;
    }

    //读取文件
    public byte[] ReadFileFromLocal(string localPath)
    {
        byte[] bytes = File.ReadAllBytes(localPath);
        return bytes;
    }

    private void AbortThread(Thread mThread)
    {
        Debug.Log("Stop");
        if (mThread == null) return;
        mThread.Abort();
        mThread = null;
    }
}

public abstract class OperateThread
{
    public Thread curThread;
    public string savePath;
    public byte[] bytes;
    public Action mAction;
    public delegate void OnCompletedHandler(Thread thread);
    public OnCompletedHandler OnCompleted;
    public bool CurStatus { get; protected set; }
    public abstract void OpeartionThread();
}

public class FileWriteThread : OperateThread
{
    public override void OpeartionThread()
    {
        try
        {
            if (FileOpeartion.GetFileOpeartion().CheckingFile(savePath))
            {
                CurStatus = false;
                if (OnCompleted != null) OnCompleted.Invoke(curThread);
                return;
            }
            FileStream fileStream = new FileStream(savePath, FileMode.Create);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
            CurStatus = true;
            if (mAction != null) mAction.Invoke();
            if (OnCompleted != null) OnCompleted.Invoke(curThread);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            CurStatus = false;
            if (OnCompleted != null) OnCompleted.Invoke(curThread);
        }
    }
}

public class FileFindWithCreate : OperateThread
{
    public string findTarget;
    public string endwith;
    public bool createOnDisk;
    public override void OpeartionThread()
    {
        DirectoryInfo dir = new DirectoryInfo(findTarget);
        var files = dir.GetFiles("*." + endwith, SearchOption.AllDirectories);
        var file = files.OrderByDescending(f => f.CreationTime).FirstOrDefault();
        if (file != null && string.IsNullOrEmpty(file.FullName)) { CurStatus = false; return; }
        if (createOnDisk)
        {
            byte[] bytes = FileOpeartion.GetFileOpeartion().ReadFileFromLocal(file.FullName);
            FileOpeartion.GetFileOpeartion().FileCreater(savePath, bytes,mAction);
        }
        CurStatus = true;
        if (OnCompleted != null) OnCompleted.Invoke(curThread);
    }
}