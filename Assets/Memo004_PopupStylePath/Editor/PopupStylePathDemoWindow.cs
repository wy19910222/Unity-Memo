/*
 * @Author: wangyun
 * @CreateTime: 2024-01-26 20:05:46 230
 * @LastEditor: wangyun
 * @EditTime: 2024-01-26 20:05:46 234
 */

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Memo004_PopupStylePath.Editor {
	public class PopupStylePathDemoWindow : EditorWindow {
		[MenuItem("Memo/Memo004/PopupStylePathDemoWindow", priority = 4)]
		public static void ShowWindow() {
			GetWindow<PopupStylePathDemoWindow>().Show();
		}

		[SerializeField]
		private string m_NativePath = "Assets";
		
		private readonly PopupStylePathDrawer m_PathDrawer = new PopupStylePathDrawer();

		private void OnGUI() {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("本地路径：", GUILayout.Width(60F));
			if (GUILayout.Button("打开目录", GUILayout.Width(80F))) {
				EditorUtility.RevealInFinder(m_NativePath);
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(4);
			{
				string newNativePath = m_PathDrawer.DrawDirectory(m_NativePath, NativeListChildren);
				if (newNativePath != m_NativePath) {
					Undo.RecordObject(this, "NativePath");
					m_NativePath = newNativePath;
				}
			}
			if (GUILayout.Button("…", "ButtonMid", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(24))) {
				string newNativePath = EditorUtility.OpenFolderPanel("Select Folder", m_NativePath, "").Replace('\\', '/');
				if (!string.IsNullOrEmpty(newNativePath)) {
					string relativePrefix = new DirectoryInfo(".").FullName.Replace('\\', '/') + "/";
					if (newNativePath.StartsWith(relativePrefix)) {
						newNativePath = newNativePath.Substring(relativePrefix.Length);
					}
					if (newNativePath != m_NativePath) {
						Undo.RecordObject(this, "NativePath");
						m_NativePath = newNativePath;
					}
				}
			}
			GUILayout.Space(4);
			EditorGUILayout.EndHorizontal();
		}

		private static string[] NativeListChildren(string path) {
			if (string.IsNullOrEmpty(path)) {
				path = ".";
			}
			if (Directory.Exists(path)) {
				string[] children = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
				return Array.ConvertAll(children, child => child.Substring(child.LastIndexOfAny(new[] {'/', '\\'}) + 1));
			}
			return null;
		}
	}
}
