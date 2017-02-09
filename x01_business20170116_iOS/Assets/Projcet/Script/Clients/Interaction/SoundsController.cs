/****
创建人：NSWell
用途：声音播放控制器
******/
using UnityEngine;
using System.Collections;

public class SoundsController : BaseInteraction
{
    private AudioSource m_AudioSource;
    private GameObject m_TargetObject;
    public AudioClip m_AudioClip;
    public ulong delay;

    public override void DoInteraction(GameObject target,int id)
    {
        base.DoInteraction(target,id);
        m_TargetObject = target;
        if (m_AudioSource == null)
            m_AudioSource = target.GetComponent<AudioSource>();
        if (m_AudioSource == null) return;
        m_AudioSource.clip = m_AudioClip;
        m_AudioSource.Play(delay);
    }

    public void OnDisable()
    {
        if (m_TargetObject != null)
            Destroy(m_TargetObject);
        Resources.UnloadUnusedAssets();
    }
}
