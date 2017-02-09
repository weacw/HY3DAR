/******
创建人：NSWell
用途：数据类型
******/
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContentsItemElement : MonoBehaviour
{
    public int guid;
    public Text mAssetNameText;
    public Button mDownloadBtn;    
    public RawImage mThunmbilImage;
    public Image dontDownloadMask;
    public Text downloadProgressText;
    public Image downloadProgressBar;
}
