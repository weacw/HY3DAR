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

public class MediaPlayerFullScreenCtrl : MonoBehaviour {
	
	public GameObject m_objVideo;
	
	int m_iOrgWidth = 0;
	int m_iOrgHeight = 0;
	// Use this for initialization
	void Start () {
		Resize ();
		
	}
	
	// Update is called once per frame
	void Update () {
		if( m_iOrgWidth != Screen.width)
			Resize();
		
		if( m_iOrgHeight != Screen.height)
			Resize();
		
	
	}
	


	
	void Resize()
	{
		m_iOrgWidth = Screen.width;
		m_iOrgHeight = Screen.height;
		
		float fRatio = (float) m_iOrgHeight / (float)m_iOrgWidth;
		
		m_objVideo.transform.localScale = new Vector3( 20.0f / fRatio, 20.0f / fRatio, 1.0f);

	#if !UNITY_WEBGL
		m_objVideo.transform.GetComponent<MediaPlayerCtrl>().Resize();
	#endif
	}
	
	
	
	
}
