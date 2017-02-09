/****
创建人：NSWell
用途：触发交互
******/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaycastInteraction : MonoBehaviour
{
    private Ray m_ray;
    private RaycastHit m_raycastHit;
    public InputType inputType;
    public Camera mainCamera;
    public float dist = 5000;
    public LayerMask layerMask;
    public List<string> hitTag = new List<string>();

    private float timecalculater;
    private const float pressInterval = 2;
    private bool hited;
    private float timeIntervalToNextClick = 0.5f;
    private float intervalToNextClick;
    public enum InputType
    {
        MouseClick,
        MouseDrag,
        MousePress
    };

    private void FixedUpdate()
    {
        if(intervalToNextClick < timeIntervalToNextClick)
            intervalToNextClick += Time.deltaTime;
        switch (inputType)
        {
            case InputType.MouseClick:
                if (Input.GetMouseButtonUp(0))
                {
                    RayChecking();
                }

                break;
            case InputType.MouseDrag:
                if (Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0))
                {
                    RayChecking();
                }
                break;
            case InputType.MousePress:
                if (Input.GetMouseButton(0))
                {
                    timecalculater += Time.deltaTime;
                    if (timecalculater >= pressInterval)
                    {
                        timecalculater = 0;
                        RayChecking();
                    }
                }
                break;
        }
    }

    private void RayChecking()
    {
        if (intervalToNextClick<timeIntervalToNextClick) return;
        m_ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        hited = Physics.Raycast(m_ray, out m_raycastHit, dist, layerMask);
        if (!hited) return;
        foreach (string tag in hitTag)
        {
            if (!m_raycastHit.transform.CompareTag(tag)) continue;
            //BaseOperatInteraction boi = GetComponent<BaseOperatInteraction>();
            ////TODO:Do boi interaction
            //boi.InitInteraction();
            //boi.DoingInteraction();
            Debug.Log((m_raycastHit.transform.GetComponentInParent<BaseInteraction>().ToString()));
            if (m_raycastHit.transform.GetComponentInParent<BaseInteraction>() == null) break;
            BaseInteraction bi = m_raycastHit.transform.GetComponentInParent<BaseInteraction>();
            bi.DoInteraction(m_raycastHit.collider.gameObject,0);
            intervalToNextClick = 0;
        }
    }
}
