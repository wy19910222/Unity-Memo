/*
 * @Author: wangyun
 * @CreateTime: 2024-09-07 1:43:29 740
 * @LastEditor: wangyun
 * @EditTime: 2024-09-07 02:39:22 346
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Memo013_PreventSaving.Editor {
	public class PreventSaving : UnityEditor.AssetModificationProcessor {
		private static bool s_IsPreventSavingPrefab;
		
		private static string[] OnWillSaveAssets(string[] paths) {
			if (s_IsPreventSavingPrefab) {
				List<string> pathList = new List<string>(paths.Length);
				List<string> preventedList = new List<string>(paths.Length);
				foreach (string path in paths) {
					if (path.EndsWith(".prefab")) {
						preventedList.Add(path);
						Debug.LogWarning($"文件未保存：{path}");
					} else {
						pathList.Add(path);
					}
				}
				int preventedCount = preventedList.Count;
				if (preventedCount > 0) {
					EditorUtility.DisplayDialog("警告", $"已阻止{preventedCount}个文件的保存", "确定");
				}
				return pathList.ToArray();
			}
			return paths;
		}
		
		[MenuItem("Memo/Memo013/EnablePreventSavingPrefab", priority = 13)]
		private static void EnablePreventSavingPrefab() {
			s_IsPreventSavingPrefab = true;
			EditorUtility.DisplayDialog("警告", "现在已经禁止保存Prefab了", "确定");
		}
		
		[MenuItem("Memo/Memo013/DisablePreventSavingPrefab", priority = 13)]
		private static void DisablePreventSavingPrefab() {
			s_IsPreventSavingPrefab = false;
			EditorUtility.DisplayDialog("提示", "现在允许保存Prefab了", "确定");
		}
		
		[MenuItem("Memo/Memo013/EnablePreventSavingPrefab", true)]
		private static bool CheckPreventSavingPrefabCanEnable() {
			return !s_IsPreventSavingPrefab;
		}
		
		[MenuItem("Memo/Memo013/DisablePreventSavingPrefab", true)]
		private static bool CheckPreventSavingPrefabCanDisable() {
			return s_IsPreventSavingPrefab;
		}
	}
}
