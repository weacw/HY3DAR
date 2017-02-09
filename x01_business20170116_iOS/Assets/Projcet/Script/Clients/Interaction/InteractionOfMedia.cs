/****
创建人：NSWell
用途：视频控制器
******/
using System;
using UnityEngine;
using System.Collections;

public class InteractionOfMedia : BaseInteraction
{
    private MediaPlayerCtrl media;


    public override void DoInteraction(GameObject target, int id)
    {
        base.DoInteraction(target, id);
        if (media == null)
            media = target.GetComponent<MediaPlayerCtrl>();
        
        DoVPBAction();
    }

    private void DoVPBAction()
    {
        
        switch (media.GetCurrentState())
        {
            case MediaPlayerCtrl.MEDIAPLAYER_STATE.END:
            case MediaPlayerCtrl.MEDIAPLAYER_STATE.READY:
                
                media.Play();
                media.SeekTo(0);
                break;
            case MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED:
                media.Play();
                media.SeekTo(media.GetCurrentSeekPercent());
                break;
//            case VideoPlayerHelper.MediaState.STOPPED:
//                vpb.VideoPlayer.Init(false);
//                vpb.VideoPlayer.Play(false, 0);
//                break;
            case MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING:
                media.Pause();
                break;

            case MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY:
            case MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR:
                media.UnLoad();
                FindObjectOfType<CallNativeFun>().ShowNativeToast("无法播放视频");
                break;          
        }

    }

  
    public override void Init()
    {
        base.Init();
        media = GetComponent<MediaPlayerCtrl>();
    }

    public override void Start()
    {
        base.Start();
        Init();
        media.Play();
        transform.localScale = new Vector3(transform.localScale.x * 0.25f, transform.localScale.y * 0.25f, 1);
    }

    private void OnDisable()
    {
        media.UnLoad();
        Resources.UnloadUnusedAssets();
    }
}
