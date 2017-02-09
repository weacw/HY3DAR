/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;

public class MediaPlayerEvent : MonoBehaviour {


	public MediaPlayerCtrl m_srcVideo;

	// Use this for initialization
	void Start () {
		m_srcVideo.OnReady += OnReady;
		m_srcVideo.OnVideoFirstFrameReady += OnFirstFrameReady;
		m_srcVideo.OnVideoError += OnError;
		m_srcVideo.OnEnd += OnEnd;
		m_srcVideo.OnResize += OnResize;

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnReady() {

		Debug.Log ("OnReady");
	}

	void OnFirstFrameReady() {
		Debug.Log ("OnFirstFrameReady");
	}

	void OnEnd() {
		Debug.Log ("OnEnd");
	}

	void OnResize()
	{
		Debug.Log ("OnResize");
	}

	void OnError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra){
		Debug.Log ("OnError");
	}
}
