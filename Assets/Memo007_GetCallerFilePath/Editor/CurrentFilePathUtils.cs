/*
 * @Author: wangyun
 * @CreateTime: 2024-07-03 20:13:22 127
 * @LastEditor: wangyun
 * @EditTime: 2024-07-03 20:13:22 131
 */

using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;

using Debug = UnityEngine.Debug;

public static class CurrentFilePathUtils {
	[MenuItem("Memo/Memo007/LogCurrentFilePath", priority = 7)]
	public static void LogCurrentFilePath() {
		Debug.Log("方法1：通过[CallerFilePath]修饰函数参数，在编译时将源码文件完整路径传递给该参数。通用性高，但必须通过一次传参。");
		Debug.Log(GetFilePathByCaller());
		Debug.Log("方法2：从调用堆栈里获取文件路径，在运行时生成，可能会影响性能。");
		Debug.Log(GetFilePathByStackTrace());
		Debug.Log("方法3：通过ScriptableObject（或MoniBehaviour）对象获取到MonoScript对象，再通过AssetDatabase获取到MonoScript对象的路径，适用于本身就在MonoScript（或MoniBehaviour）派生类内的调用。");
		Debug.Log(GetFilePathByScriptableObject());
		Debug.Log("方法4：通过一个已知的独一无二的文件名，搜索文件路径，适用于已知文件但不确定文件被放在哪里的情况。");
		Debug.Log(GetFilePathByFilename());
	}
	
	public static string GetFilePathByCaller([CallerFilePath] string callerFilePath = null) {
		return callerFilePath;
	}
	
	public static string GetFilePathByStackTrace() {
		return new StackTrace(true).GetFrame(0).GetFileName();
	}
	
	public static string GetFilePathByScriptableObject(ScriptableObject so = null) {
		if (!so) {
			so = ScriptableObject.CreateInstance<EmptyScriptableObject>();
		}
		return AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(so));;
	}

	public static string GetFilePathByFilename(string filename = nameof(CurrentFilePathUtils)) {
		string[] guids = AssetDatabase.FindAssets($"{filename}");
		foreach (string guid in guids) {
			string path = AssetDatabase.GUIDToAssetPath(guid);
			int pathLength = path.Length;
			int lastSlashIndex = path.LastIndexOf('/');
			int lastPointIndex = path.LastIndexOf('.', pathLength - 1, pathLength - lastSlashIndex);
			if (lastPointIndex == -1) {
				lastPointIndex = pathLength;
			}
			if (path.Substring(lastSlashIndex + 1, lastPointIndex - lastSlashIndex - 1) == filename) {
				return path;
			}
		}
		return null;
	}
}
