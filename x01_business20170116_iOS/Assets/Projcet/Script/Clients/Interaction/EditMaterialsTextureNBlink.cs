/****
创建人：NSWell
用途：控制材质球颜色变化
******/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class EditMaterialsTextureNBlink : BaseInteraction
{
    public List<MaterialsTextureNBlinkUnit> m_MaterialsConf = new List<MaterialsTextureNBlinkUnit>();
    private bool isInvoke;
    public override void DoInteraction(GameObject target,int id)
    {
        if (isInvoke) return;
        base.DoInteraction(target,id);
        foreach (MaterialsTextureNBlinkUnit unit in m_MaterialsConf)
        {
            if (string.Compare(unit.m_unit_Id, target.name, StringComparison.Ordinal) != 0)
            {
                unit.m_Target_Display.SetActive(false);
                continue;
            }

            //Set default vale
            unit.orginalTexture = unit.m_ToSetSourceTexture;
            unit.orginalColor = unit.m_Renderer.material.GetColor("_TintColor");

            unit.m_Renderer.enabled = true;
            Material mat = unit.m_Renderer.material;
            mat.SetTexture("_MainTex", unit.m_ToSetSourceTexture);
            Tween tw = unit.m_Renderer.material.DOColor(unit.m_blinkColor, "_TintColor", unit.m_blinkTime);
            tw.SetLoops(unit.blinkLoopAmount, LoopType.Yoyo);
            tw.SetAutoKill(true);
            tw.OnStart(() =>
            {
                isInvoke = true;
            });
            var unit1 = unit;
            tw.OnComplete(() =>
            {
                unit1.m_Renderer.material.DOColor(unit1.orginalColor, "_TintColor", unit1.m_blinkTime);
                unit1.m_Target_Display.SetActive(true);
                isInvoke = false;
            });
        }
    }

    public override void Init()
    {
        base.Init();
        if (m_MaterialsConf.Count < 0) return;
    }

    public override void Start()
    {
        base.Start();
    }
}
[System.Serializable]
public class MaterialsTextureNBlinkUnit
{
    public Renderer m_Renderer;
    public GameObject m_Target_Display;
    public float m_blinkTime;
    public int blinkLoopAmount;
    public Color m_blinkColor;
    public string m_unit_Id;
    public Texture2D m_ToSetSourceTexture;

    internal Color orginalColor;
    internal Texture2D orginalTexture;
}