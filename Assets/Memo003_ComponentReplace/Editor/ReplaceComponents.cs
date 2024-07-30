/*
 * @Author: wangyun
 * @CreateTime: 2023-01-31 22:48:35 809
 * @LastEditor: wangyun
 * @EditTime: 2023-02-02 12:39:19 958
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

using UObject = UnityEngine.Object;

namespace Memo003_ComponentReplace.Editor {
	using TFrom = MonoBehaviour;
	using TTo = MonoBehaviour;
	using Converter = Converter_Default;
	
	public static class ReplaceComponents {
		[MenuItem("Memo/Memo003/老脚本废弃方案——组件替换", priority = 3)]
		private static void ReplaceComponent() {
			List<UObject> assets = new List<UObject>(Selection.objects);
			string[] selectedPaths = assets.ConvertAll(AssetDatabase.GetAssetPath).ToArray();
	
			HashSet<string> pathSet = new HashSet<string>();
			foreach (string selectedPath in selectedPaths) {
				if (File.Exists(selectedPath)) {
					if (selectedPath.EndsWith(".prefab")) {
						pathSet.Add(selectedPath.Replace("\\", "/"));
					}
				} else {
					string[] files = Directory.GetFiles(selectedPath, "*", SearchOption.AllDirectories);
					foreach (string filePath in files) {
						if (filePath.EndsWith(".prefab")) {
							pathSet.Add(filePath.Replace("\\", "/"));
						}
					}
				}
			}
			
			ReplaceMonoBehaviour<TFrom, TTo>(pathSet.ToArray(), Converter.CompConverter, Converter.ModificationConverter);
		}
	
		// 需要替换掉：Prefab内不属于子孙Prefab的TSrc组件、对子孙Prefab实例新增的TSrc组件、对子孙Prefab实例移除的TSrc组件、对子孙Prefab实例TSrc组件的修改。
		// 对子孙Prefab实例新增的TSrc组件：可以和Prefab内不属于子孙Prefab的TSrc组件一起改掉。
		// 对子孙Prefab实例移除的TSrc组件：只要子孙Prefab替换时不改变fileID，则不需要管。
		// 最终就需要操作两项：Prefab内不属于子孙Prefab的TSrc组件（包括对子孙Prefab实例新增的TSrc组件）、对子孙Prefab实例TSrc组件的修改。
		private static void ReplaceMonoBehaviour<TSrc, TDst>(
			IEnumerable<string> prefabPaths,
			Action<TSrc, TDst> compConverter,
			Action<List<PropertyModification>> modificationsConverter
		) where TSrc : MonoBehaviour where TDst : MonoBehaviour {
			Dictionary<string, List<(string, string, string)>> pathSrcIDDstIDTempListDict = new Dictionary<string, List<(string, string, string)>>();
			foreach (var prefabPath in prefabPaths) {
				if (!pathSrcIDDstIDTempListDict.ContainsKey(prefabPath)) {
					GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
					TSrc[] comps = go.GetComponentsInChildren<TSrc>(true);
					List<TSrc> srcComps = new List<TSrc>();
					HashSet<GameObject> subPrefabSet = new HashSet<GameObject>();
					foreach (var comp in comps) {
						if (PrefabUtility.IsAddedComponentOverride(comp)) {
							srcComps.Add(comp);
						} else {
							GameObject subPrefab = PrefabUtility.GetOutermostPrefabInstanceRoot(comp);
							if (!subPrefab) {
								srcComps.Add(comp);
							} else {
								subPrefabSet.Add(subPrefab);
							}
						}
					}
					if (srcComps.Count > 0) {
						List<(string, string, string)> srcIDDstIDTempList = new List<(string, string, string)>();
						pathSrcIDDstIDTempListDict.Add(prefabPath, srcIDDstIDTempList);
						foreach (var src in srcComps) {
							TDst dst = src.gameObject.AddComponent<TDst>();
							compConverter?.Invoke(src, dst);
							long srcFileID = GetLocalIdentifierInFile(src);
							long dstFileID = GetLocalIdentifierInFile(dst);
							srcIDDstIDTempList.Add(("--- !u!114 &" + srcFileID, "--- !u!114 &" + dstFileID, "--- !u!114 &" + srcFileID + "_Temp"));
						}
						EditorUtility.SetDirty(go);
					}
					if (subPrefabSet.Count > 0) {
						foreach (var subPrefab in subPrefabSet) {
							List<PropertyModification> modificationList = new List<PropertyModification>(PrefabUtility.GetPropertyModifications(subPrefab));
							modificationsConverter?.Invoke(modificationList);
							PrefabUtility.SetPropertyModifications(subPrefab, modificationList.ToArray());
						}
						EditorUtility.SetDirty(go);
					}
					if (srcComps.Count > 0 || subPrefabSet.Count > 0) {
						AssetDatabase.SaveAssets();
					}
				}
			}
			AssetDatabase.Refresh();	// 让Library里的数据记录下新的FileID
			foreach (var pair in pathSrcIDDstIDTempListDict) {
				string prefabPath = pair.Key;
				List<(string, string, string)> srcIDDstIDTempList = pair.Value;
				
				FileInfo file = new FileInfo(prefabPath);
				string text;
				using (FileStream fs = file.OpenRead()) {
					using (MemoryStream ms = new MemoryStream()) {
						var bytesTemp = new byte[4096];
						int readLength;
						while ((readLength = fs.Read(bytesTemp, 0, 4096)) > 0) {
							ms.Write(bytesTemp, 0, readLength);
						}
						ms.Flush();
						text = Encoding.UTF8.GetString(ms.ToArray());
					}
				}
				
				if (!string.IsNullOrEmpty(text)) {
					foreach (var (item1, _, item3) in srcIDDstIDTempList) {
						text = text.Replace(item1, item3);
					}
					foreach (var (item1, item2, _) in srcIDDstIDTempList) {
						text = text.Replace(item2, item1);
					}
					foreach (var (_, item2, item3) in srcIDDstIDTempList) {
						text = text.Replace(item3, item2);
					}
					using (FileStream fs = file.OpenWrite()) {
						byte[] bytes = Encoding.UTF8.GetBytes(text); 
						fs.Write(bytes, 0, bytes.Length); 
						fs.Flush();
					}
				}
			}
			AssetDatabase.Refresh();	// 让Library里的数据更新成修改后的状态
			foreach (var pair in pathSrcIDDstIDTempListDict) {
				string prefabPath = pair.Key;
				GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
				TSrc[] _srcComps = Array.FindAll(
					go.GetComponentsInChildren<TSrc>(true),
					comp => PrefabUtility.IsAddedComponentOverride(comp) || !PrefabUtility.GetNearestPrefabInstanceRoot(comp)
				);
				foreach (var src in _srcComps) {
					UObject.DestroyImmediate(src, true);
				}
				EditorUtility.SetDirty(go);
				AssetDatabase.SaveAssetIfDirty(go);
			}
		}
	
		[MenuItem("Assets/老脚本废弃方案/打印Prefab内对子孙Prefab的修改的路径", priority = 0)]
		private static void LogPrefabsModificationPropertyPath() {
			List<UObject> assets = new List<UObject>(Selection.objects);
			string[] selectedPaths = assets.ConvertAll(AssetDatabase.GetAssetPath).ToArray();
	
			HashSet<string> pathSet = new HashSet<string>();
			foreach (string selectedPath in selectedPaths) {
				if (File.Exists(selectedPath)) {
					if (selectedPath.EndsWith(".prefab")) {
						pathSet.Add(selectedPath.Replace("\\", "/"));
					}
				} else {
					string[] files = Directory.GetFiles(selectedPath, "*", SearchOption.AllDirectories);
					foreach (string filePath in files) {
						if (filePath.EndsWith(".prefab")) {
							pathSet.Add(filePath.Replace("\\", "/"));
						}
					}
				}
			}
			LogPrefabModificationPropertyPath<TFrom>(pathSet.ToArray());
		}
	
		private static readonly string[] ignorePathPatterns = {
			// "^m_Enabled$",
			
			// "^title$",
			// "^autoTrigger$",
			// "^triggerDelay$",
			// "^validInactive$",
			
			// "^singleProcess$",
			
			// "^steps\\.Array\\.data\\[\\d+\\]\\.title$",
			// "^steps\\.Array\\.data\\[\\d+\\]\\.time$",
			// "^steps\\.Array\\.data\\[\\d+\\]\\.delayFrames$",
			// "^steps\\.Array\\.data\\[\\d+\\]\\.unityEvent\\."
		};
		// 打印Prefab内对子孙Prefab的修改里针对TSrc修改的PropertyPath，分析后方便写modificationsConverter
		private static void LogPrefabModificationPropertyPath<TSrc>(IEnumerable<string> prefabPaths) where TSrc : MonoBehaviour {
			foreach (var prefabPath in new HashSet<string>(prefabPaths)) {
				GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
				TSrc[] comps = go.GetComponentsInChildren<TSrc>(true);
				HashSet<GameObject> subPrefabSet = new HashSet<GameObject>();
				foreach (var comp in comps) {
					if (!PrefabUtility.IsAddedComponentOverride(comp)) {
						GameObject subPrefab = PrefabUtility.GetOutermostPrefabInstanceRoot(comp);
						if (subPrefab) {
							PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(subPrefab);
							foreach (var modification in modifications) {
								if (modification.target is TSrc) {
									if (!Array.Exists(ignorePathPatterns, pattern => Regex.IsMatch(modification.propertyPath, pattern))) {
										subPrefabSet.Add(subPrefab);
										break;
									}
								}
							}
						}
					}
				}
				if (subPrefabSet.Count > 0) {
					Debug.LogError("<color=red>Prefab：</color>" + prefabPath, go);
					foreach (var subPrefab in subPrefabSet) {
						List<PropertyModification> modificationList = new List<PropertyModification>(PrefabUtility.GetPropertyModifications(subPrefab));
						Debug.LogError("<color=orange>SubPrefab：</color>" + subPrefab.name, subPrefab);
						foreach (var modification in modificationList) {
							if (modification.target is TSrc) {
								if (!Array.Exists(ignorePathPatterns, pattern => Regex.IsMatch(modification.propertyPath, pattern))) {
									Debug.LogError("<color=yellow>Target：</color>" + modification.target.name, modification.target);
									Debug.LogError("<color=cyan>Path：</color>" + modification.propertyPath);
								}
							}
						}
					}
					EditorUtility.SetDirty(go);
				}
			}
			Debug.LogError("打印结束");
		}
	
		private static long GetLocalIdentifierInFile(UObject obj) {
			// return new SerializedObject(obj) { inspectorMode = InspectorMode.Debug }.FindProperty("m_LocalIdentfierInFile").longValue;
			SerializedObject so = new SerializedObject(obj);
			PropertyInfo inspectorModePI = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.Instance | BindingFlags.NonPublic);
			inspectorModePI?.SetValue(so, InspectorMode.Debug, null);
			return so.FindProperty("m_LocalIdentfierInFile").longValue;
		}
	
		[MenuItem("Assets/老脚本废弃方案/查找引用", priority = 0)]
		private static void FindReference() {
			if (Selection.activeObject is GameObject go) {
				Dictionary<long, TFrom> dict = new Dictionary<long, TFrom>();
				TFrom[] comps = go.GetComponentsInChildren<TFrom>();
				foreach (var comp in comps) {
					long fileID = GetLocalIdentifierInFile(comp);
					dict.Add(fileID, comp);
				}
	
				string prefabPath = AssetDatabase.GetAssetPath(go);
				FileInfo file = new FileInfo(prefabPath);
				string text;
				using (FileStream fs = file.OpenRead()) {
					using (MemoryStream ms = new MemoryStream()) {
						var bytesTemp = new byte[4096];
						int readLength;
						while ((readLength = fs.Read(bytesTemp, 0, 4096)) > 0) {
							ms.Write(bytesTemp, 0, readLength);
						}
						ms.Flush();
						text = Encoding.UTF8.GetString(ms.ToArray());
					}
				}
				foreach (var fileID in dict.Keys) {
					if (text.Contains(fileID + string.Empty)) {
						Debug.LogWarning(dict[fileID], dict[fileID]);
					} else {
						Debug.LogError(dict[fileID], dict[fileID]);
					}
				}
			}
		}
	}
}
