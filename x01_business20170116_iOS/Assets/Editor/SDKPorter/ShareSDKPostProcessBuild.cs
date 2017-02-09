using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using cn.sharesdk.unity3d.sdkporter;
using System.IO;


public static class ShareSDKPostProcessBuild {
	//[PostProcessBuild]
	[PostProcessBuildAttribute(1188)]
	public static void onPostProcessBuild(BuildTarget target,string targetPath){
		string unityEditorAssetPath = Application.dataPath;
        Debug.Log(unityEditorAssetPath);
		if (target != BuildTarget.iOS) {
			Debug.LogWarning ("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

		XCProject project = new XCProject (targetPath);

		//var files = System.IO.Directory.GetFiles( unityEditorAssetPath, "*.projmods", System.IO.SearchOption.AllDirectories );
		var files = System.IO.Directory.GetFiles( unityEditorAssetPath + "/Editor/SDKPorter", "*.projmods", System.IO.SearchOption.AllDirectories);
		foreach( var file in files ) {
			project.ApplyMod( file );
		}

		//如需要预配置Xocode中的URLScheme 和 白名单,请打开下两行代码,并自行配置相关键值
		string projPath = Path.GetFullPath (targetPath);
		EditInfoPlist (projPath);


		//Finally save the xcode project
		project.Save();

	}

	private static void EditInfoPlist(string projPath){

		XCPlist plist = new XCPlist (projPath);

		//URL Scheme 添加
		string PlistAdd = @"  
            <key>CFBundleURLTypes</key>
			<array>
				<dict>
					<key>CFBundleURLSchemes</key>
					<array>
					<string>QQ41E9C33C</string>
					<string>wx7b829987ab83fa91</string>
					<string>wb633737098</string>
					</array>
				</dict>
			</array>";

		//白名单添加
		string LSAdd = @"
		<key>LSApplicationQueriesSchemes</key>
			<array>
			<string>mqqopensdkapiV4</string>
			<string>weibosdk</string>
			<string>sinaweibohd</string>
			<string>sinaweibo</string>
            <string>weibosdk2.5</string>
			<string>mqqwpa</string>
			<string>instagram</string>
			<string>fbauth2</string>
			<string>renren</string>
			<string>renrenios</string>
			<string>renrenapi</string>
			<string>rm226427com.mob.demoShareSDK</string>
			<string>mqq</string>
			<string>mqqopensdkapiV2</string>
			<string>mqqopensdkapiV3</string>
			<string>wtloginmqq2</string>
			<string>mqqapi</string>
			<string>mqqOpensdkSSoLogin</string>
			<string>sinaweibohdsso</string>
			<string>sinaweibosso</string>
			<string>wechat</string>
			<string>weixin</string>
		</array>";


		//在plist里面增加一行
		plist.AddKey(PlistAdd);
		plist.AddKey (LSAdd);
		plist.Save();
	}


}