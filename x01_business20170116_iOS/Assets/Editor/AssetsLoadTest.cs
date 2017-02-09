using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class AssetsLoadTest {

	[Test]
	public void EditorTest()
	{
	  MonoBehaviour.FindObjectOfType<DownloadImageTargetsZip>().Download();

	}
}
