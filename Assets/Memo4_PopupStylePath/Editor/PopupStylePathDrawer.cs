/*
 * @Author: wangyun
 * @CreateTime: 2024-01-26 17:08:27 616
 * @LastEditor: wangyun
 * @EditTime: 2024-01-26 17:08:27 620
 */

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Memo4_PopupStylePath.Editor {
	public class PopupStylePathDrawer {
		private GUIStyle m_ButtonMidStyle;
		private GUIStyle ButtonMidStyle => m_ButtonMidStyle ??= "ButtonMid";
		
		private bool m_IsEditingPath;
		
		public string DrawDirectory(string dirPath, Func<string, string[]> listChildrenFunc) {
			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();
			if (!m_IsEditingPath) {
				string path = dirPath?.Trim() ?? "";
				if (path.EndsWith("/") || path.EndsWith("\\")) {
					path = path.Substring(0, path.Length - 1);
				}
				string[] list = string.IsNullOrEmpty(path) ? Array.Empty<string>() : path.Split('/', '\\');
				for (int i = 0, length = list.Length; i <= length; ++i) {
					string currentName = i < length ? list[i] : null;
					string dir = string.Join("/", list.Take(i));
					string[] childNames = listChildrenFunc?.Invoke(dir);
					List<GUIContent> children = new List<GUIContent> {
						EditorGUIUtility.TrTextContent(".")
					};
					int selected = 0;
					if (childNames != null) {
						for (int j = 0, childrenCount = childNames.Length; j < childrenCount; ++j) {
							string childName = childNames[j];
							children.Add(EditorGUIUtility.TrTextContent(childName));
							if (childName == currentName) {
								selected = j + 1;
							}
						}
					}
					if (currentName != null) {
						if (childNames == null || Array.IndexOf(childNames, currentName) == -1) {
							children.Insert(0, EditorGUIUtility.TrTextContent(list[i]));
						}
					}
					float width = ButtonMidStyle.CalcSize(new GUIContent(children[selected])).x;
					GUILayout.Space(width);
				}
			}
			GUI.SetNextControlName("InputField");
			if (m_IsEditingPath) {
				dirPath = EditorGUILayout.DelayedTextField(dirPath);
			} else {
				EditorGUILayout.TextField(string.Empty);
			}
	
			if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint) {
				m_IsEditingPath = GUI.GetNameOfFocusedControl() == "InputField" && EditorGUIUtility.editingTextField;
				if (m_IsEditingPath) {
					if (Event.current.type is EventType.MouseDown or EventType.MouseUp
							&& !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
						m_IsEditingPath = false;
						Type viewType = typeof(EditorGUIUtility).Assembly.GetType("UnityEditor.GUIView");
						PropertyInfo currentPI = viewType?.GetProperty("current", BindingFlags.Static | BindingFlags.Public);
						object view = currentPI?.GetValue(null);
						MethodInfo repaintMI = viewType?.GetMethod("Repaint", BindingFlags.Instance | BindingFlags.Public);
						EditorApplication.delayCall += () => {
							repaintMI?.Invoke(view, null);
						};
					}
				}
			}
			EditorGUILayout.EndHorizontal();
	
			if (!m_IsEditingPath) {
				GUILayout.Space(-EditorGUIUtility.singleLineHeight - 2);
				
				EditorGUILayout.BeginHorizontal();
				string path = dirPath?.Trim() ?? "";
				if (path.EndsWith("/") || path.EndsWith("\\")) {
					path = path.Substring(0, path.Length - 1);
				}
	
				string[] list = string.IsNullOrEmpty(path) ? Array.Empty<string>() : path.Split('/', '\\');
				for (int i = 0, length = list.Length; i <= length; ++i) {
					string currentName = i < length ? list[i] : null;
					string dir = string.Join("/", list.Take(i));
					string[] childNames = listChildrenFunc?.Invoke(dir);
					List<GUIContent> children = new List<GUIContent> {
						EditorGUIUtility.TrTextContent(".")
					};
					int selected = 0;
					if (childNames != null) {
						for (int j = 0, childrenCount = childNames.Length; j < childrenCount; ++j) {
							string childName = childNames[j];
							children.Add(EditorGUIUtility.TrTextContent(childName));
							if (childName == currentName) {
								selected = j + 1;
							}
						}
					}
					Color prevColor = GUI.contentColor;
					if (currentName != null) {
						if (childNames == null || Array.IndexOf(childNames, currentName) == -1) {
							children.Insert(0, EditorGUIUtility.TrTextContent(list[i]));
							GUI.contentColor = Color.cyan;
						}
					}
					float width = ButtonMidStyle.CalcSize(new GUIContent(children[selected])).x;
					int newSelected = EditorGUILayout.Popup(selected, children.ToArray(), ButtonMidStyle, GUILayout.Width(width));
					GUI.contentColor = prevColor;
					if (newSelected != selected) {
						if (newSelected == 0) {
							dirPath = dir;
						} else if (dir == string.Empty) {
							dirPath = children[newSelected].text;
							if (Event.current.control) {
								dirPath += "/" + string.Join("/", list.Skip(i + 1));
							}
						} else {
							dirPath = dir + "/" + children[newSelected].text;
							if (Event.current.control) {
								dirPath += "/" + string.Join("/", list.Skip(i + 1));
							}
						}
						break;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			return dirPath;
		}
	}
}
