/****
创建人：NSWell
用途：导航页动画
******/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class NavigationHUDLogics : MonoBehaviour
{
    public List<GameObject> navigationList = new List<GameObject>();
    public List<GameObject> pageMarker = new List<GameObject>();
    public float intervalMoving;
    public CanvasGroup navigationRoot;
    [SerializeField]
    private int curNavigationPageIndex;
    [SerializeField]
    private float timeLeft;
    private float left, mid, right;

    public void Start()
    {
        Init();
    }

    public void Update()
    {
        CalculaterTime();
    }

    public void TapToStart()
    {
        navigationRoot.DOFade(0, 0.35f).OnComplete(() =>
        {
            navigationRoot.gameObject.SetActive(false);
        });
    }

    private void Init()
    {
        left = -Screen.width;
        mid = Mathf.Abs(left) / 2;
        right = Mathf.Abs(left);
    }

    private void CalculaterTime()
    {
        if (timeLeft >= intervalMoving)
        {
            //TODO:moving navigation-image to next pos
            DoMovingNextNavigation();
            timeLeft = 0;
            return;
        }
        timeLeft += Time.deltaTime;
    }

    private void DoMovingNextNavigation()
    {
        Transform trans = navigationList[curNavigationPageIndex].transform;
        Image image = navigationList[curNavigationPageIndex].GetComponent<Image>();
        trans.DOLocalMoveX(left, 0.5f).OnStart(() =>
        {
            curNavigationPageIndex++;
            if (curNavigationPageIndex > navigationList.Count - 1)
                curNavigationPageIndex = 0;

            trans = navigationList[curNavigationPageIndex].transform;
            image = navigationList[curNavigationPageIndex].GetComponent<Image>();
            trans.DOMoveX(mid, 0.5f);
            image.DOFade(1, 0.4f);
        }).OnComplete(() =>
        {
            pageMarker[curNavigationPageIndex].SetActive(true);
            int preIndex = curNavigationPageIndex - 1;
            if ((preIndex) < 0)
                preIndex = 2;
            pageMarker[preIndex].SetActive(false);
            trans = navigationList[preIndex].transform;
            image = navigationList[preIndex].GetComponent<Image>();
            image.color = new Color(1, 1, 1, 0);
            trans.DOLocalMoveX(right, 0.5f).SetDelay(0.125f);
        });
        image.DOFade(0, 0.8f);
    }
}
