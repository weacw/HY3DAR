

/*
 *Copyright(C) 2015 by WEACW All rights reserved.  
 *Author:       Well Tsai
 *http://weacw.com
 *推陈出新是我们无上诀窍
 * 用途：图像追踪状态
*/


using UnityEngine;
using Vuforia;

public class TrackableEvent : MonoBehaviour, ITrackableEventHandler
{
    public TrackableBehaviour GetTrackableBehaviour { get; private set; }
    private void Start() { Init(); }

    private void Init()
    {
        GetTrackableBehaviour = GetComponent<TrackableBehaviour>();
        GetTrackableBehaviour.RegisterTrackableEventHandler(this);
    }
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        switch (newStatus)
        {
            case TrackableBehaviour.Status.NOT_FOUND:
            case TrackableBehaviour.Status.UNKNOWN:
            case TrackableBehaviour.Status.UNDEFINED:
                OnTargetLost();
                break;
            case TrackableBehaviour.Status.DETECTED:
            case TrackableBehaviour.Status.TRACKED:
            case TrackableBehaviour.Status.EXTENDED_TRACKED:
                OnTargetFound();
                break;
        }
    }

    //on image target lost
    private void OnTargetLost()
    {
        if (Manager.Instance && Manager.Instance.GetScaneManager)
            Manager.Instance.GetScaneManager.OnLostEventMethod(GetTrackableBehaviour.TrackableName, false);
    }

    //on image target found
    private void OnTargetFound()
    {
        if (Manager.Instance && Manager.Instance.GetScaneManager)
            Manager.Instance.GetScaneManager.OnFoundEventMethod(GetTrackableBehaviour.TrackableName, true, this);
    }
}