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
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;



#if !UNITY_WEBGL

public class SeekBarCtrl : MonoBehaviour ,IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler{

	public MediaPlayerCtrl m_srcVideo;
	public Slider m_srcSlider;
	public float m_fDragTime = 0.2f;


	bool m_bActiveDrag = true;
	bool m_bUpdate = true;

	float m_fDeltaTime = 0.0f;
	float m_fLastValue = 0.0f;
	float m_fLastSetValue = 0.0f;

	// Use this for initialization
	void Start () {
	
	}


	// Update is called once per frame
	void Update () {

		if (m_bActiveDrag == false) {
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > m_fDragTime) {
				m_bActiveDrag = true;
				m_fDeltaTime = 0.0f;
				//if(m_fLastSetValue != m_fLastValue)
				//	m_srcVideo.SetSeekBarValue (m_fLastValue);

			}
		}



		if (m_bUpdate == false)
			return;
			
		if (m_srcVideo != null) {

			if (m_srcSlider != null) {
				m_srcSlider.value = m_srcVideo.GetSeekBarValue();

			}
			
		}
	
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("OnPointerEnter:");  

		m_bUpdate = false;



	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("OnPointerExit:");

		m_bUpdate = true;


	}

	public void OnPointerDown(PointerEventData eventData)
	{


	}

	public void OnPointerUp(PointerEventData eventData)
	{
	
		m_srcVideo.SetSeekBarValue (m_srcSlider.value);






	}


	public void OnDrag(PointerEventData eventData)
	{
		 Debug.Log("OnDrag:"+ eventData);   

		if (m_bActiveDrag == false) 
		{
			m_fLastValue = m_srcSlider.value;
			return;
		}

		m_srcVideo.SetSeekBarValue (m_srcSlider.value);
		m_fLastSetValue = m_srcSlider.value;
		m_bActiveDrag = false;
	
	}

}
#endif
