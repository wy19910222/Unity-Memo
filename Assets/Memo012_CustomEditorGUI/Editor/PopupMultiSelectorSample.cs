/*
 * @Author: wangyun
 * @CreateTime: 2024-08-08 21:57:12 534
 * @LastEditor: wangyun
 * @EditTime: 2024-08-08 21:57:12 545
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Memo012_CustomEditorGUI.Editor {
	public class PopupMultiSelectorSample : EditorWindow {
		[MenuItem("Memo/Memo012/PopupMultiSelectorSample", priority = 12)]
		public static void ShowWindow() {
			var window = GetWindow<PopupMultiSelectorSample>();
			window.minSize = new Vector2(600F, 160F);
			window.Show();
		}

		private void ShowButton(Rect rect) {
			if (GUI.Button(rect, EditorGUIUtility.IconContent("_Help"), "IconButton")) {
				PopupWindow.Show(rect, new PopupContent(130, 30, popupRect => {
					popupRect.x += 6;
					EditorGUI.LabelField(popupRect, "这里是通用弹窗示例。");
				}));
			}
		}

		private void OnGUI() {
			EditorGUILayout.HelpBox(
					"这里是提供一个超过32项的多选解决方案。\n" + 
					"无论是 EnumFlagsField 还是 MaskField ，因为是通过int值按位或得到的，所以最多支持32个选项。\n" + 
					$"为了方便展示，这里只设置{m_Options.Count}个选项，但支持任意数量的选项。"
			, MessageType.Info);
			
			GUILayout.Space(10);
			
			DrawPopupMultiSelectSample();
			
			GUILayout.Space(10);
			
			DrawInitPopupMultiSelectSample();
			
		}

		private readonly List<(string, bool)> m_Options = new List<(string, bool)> {
			("Option0", false),
			("Option1", false),
			("Option2", false),
			("Option3", false),
			("Option4", false),
			("Option5", false),
			("Option6", false),
			("Option7", false)
		};
		private void DrawPopupMultiSelectSample() {
			EditorGUILayout.LabelField("PopupMultiSelect 接口适用于数据的数量和顺序都固定的选项", (GUIStyle) "BoldLabel");

			Rect rect = CustomEditorGUI.GetLastRect();
			rect.y += rect.height;
			rect.height = 1;
			rect.width = position.width;
			EditorGUI.DrawRect(rect, Color.gray);
			
			EditorGUILayout.LabelField("复选按钮：");
			EditorGUILayout.BeginHorizontal();
			for (int i = 0, length = m_Options.Count; i < length; ++i) {
				(string optionName, bool isSelected) = m_Options[i];
				bool newIsSelected = GUILayout.Toggle(isSelected, optionName, "Button");
				if (newIsSelected != isSelected) {
					Undo.RecordObject(this, "OptionState");
					m_Options[i] = (optionName, newIsSelected);
				}
			}
			EditorGUILayout.EndHorizontal();
			
			string[] displayedOptions = new string[m_Options.Count];
			bool[] isSelecteds = new bool[m_Options.Count];
			for (int i = 0, length = m_Options.Count; i < length; ++i) {
				(string optionName, bool isSelected) = m_Options[i];
				displayedOptions[i] = optionName;
				isSelecteds[i] = isSelected;
			}
			
			EditorGUILayout.LabelField("非定制化复选下拉框：");
			PopupMultiSelector.PopupMultiSelect(newIsSelecteds => {
				Undo.RecordObject(this, "OptionState");
				for (int i = 0, length = m_Options.Count; i < length; ++i) {
					(string optionName, bool isSelected) = m_Options[i];
					if (isSelected != newIsSelecteds[i]) {
						m_Options[i] = (optionName, newIsSelecteds[i]);
					}
				}
				Repaint();
			}, "Options复选下拉按钮", isSelecteds, displayedOptions);
			
			EditorGUILayout.LabelField("以选中内容为按钮文本的复选下拉框：");
			PopupMultiSelect(newIsSelecteds => {
				Undo.RecordObject(this, "OptionState");
				for (int i = 0, length = m_Options.Count; i < length; ++i) {
					(string optionName, bool isSelected) = m_Options[i];
					if (isSelected != newIsSelecteds[i]) {
						m_Options[i] = (optionName, newIsSelecteds[i]);
					}
				}
				Repaint();
			}, isSelecteds, displayedOptions);
		}

		private readonly List<(int, string)> m_IdOptions = new List<(int, string)> {
			(0, "Option0"),
			(1, "Option1"),
			(2, "Option2"),
			(3, "Option3"),
			(4, "Option4"),
			(5, "Option5"),
			(6, "Option6"),
			(7, "Option7")
		};
		private readonly List<int> m_SelectedIds = new List<int>();
		private void DrawInitPopupMultiSelectSample() {
			EditorGUILayout.LabelField("IntPopupMultiSelect 接口适用于使用id储存选中状态的数据", (GUIStyle) "BoldLabel");

			Rect rect = CustomEditorGUI.GetLastRect();
			rect.y += rect.height;
			rect.height = 1;
			rect.width = position.width;
			EditorGUI.DrawRect(rect, Color.gray);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("复选按钮：");
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("-", GUILayout.Width(40F))) {
				Undo.RecordObject(this, "IdOptionCount");
				m_IdOptions.RemoveAt(m_IdOptions.Count - 1);
			}
			if (GUILayout.Button("+", GUILayout.Width(40F))) {
				Undo.RecordObject(this, "IdOptionCount");
				int newId = m_IdOptions.Count;
				string newOptionName = $"Option{newId}";
				m_IdOptions.Add((newId, newOptionName));
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			for (int i = 0, length = m_IdOptions.Count; i < length; ++i) {
				(int id, string optionName) = m_IdOptions[i];
				bool isSelected = m_SelectedIds.Contains(id);
				bool newIsSelected = GUILayout.Toggle(isSelected, optionName, "Button");
				if (newIsSelected != isSelected) {
					Undo.RecordObject(this, "IdOptionState");
					if (newIsSelected) {
						m_SelectedIds.Add(id);
					} else {
						m_SelectedIds.Remove(id);
					}
				}
			}
			EditorGUILayout.EndHorizontal();
			
			int[] optionIds = new int[m_IdOptions.Count];
			string[] optionNames = new string[m_IdOptions.Count];
			for (int i = 0, length = m_IdOptions.Count; i < length; ++i) {
				(int id, string optionName) = m_IdOptions[i];
				optionIds[i] = id;
				optionNames[i] = optionName;
			}
			
			EditorGUILayout.LabelField("非定制化复选下拉框：");
			PopupMultiSelector.IntPopupMultiSelect(newSelectedIds => {
				m_SelectedIds.Clear();
				m_SelectedIds.AddRange(newSelectedIds);
				Repaint();
			}, "Options复选下拉按钮", m_SelectedIds.ToArray(), optionNames, optionIds);
			
			GUILayout.Space(10);
			
			EditorGUILayout.LabelField("以选中内容为按钮文本的复选下拉框：");
			IntPopupMultiSelect(newSelectedIds => {
				m_SelectedIds.Clear();
				m_SelectedIds.AddRange(newSelectedIds);
				Repaint();
			}, m_SelectedIds.ToArray(), optionNames, optionIds);
		}

		private static void PopupMultiSelect(Action<bool[]> onChange, bool[] isSelecteds, string[] displayedOptions) {
			string label;
			int totalLength = isSelecteds.Length;
			List<string> selectedOptions = new List<string>();
			for (int i = 0; i < totalLength; ++i) {
				if (isSelecteds[i]) {
					selectedOptions.Add(displayedOptions[i]);
				}
			}
			int selectedLength = selectedOptions.Count;
			if (selectedLength <= 0) {
				label = "Nothing";
			} else if (selectedLength >= totalLength) {
				label = "Everything";
			} else {
				label = string.Join(",", selectedOptions);
			}
			PopupMultiSelector.PopupMultiSelect(onChange, label, isSelecteds, displayedOptions);
		}
		
		private static void IntPopupMultiSelect(Action<int[]> onChange, int[] selectedIds, string[] displayedOptions, int[] optionIds) {
			string label;
			HashSet<int> selectedIdSet = new HashSet<int>(selectedIds);
			bool everything = Array.TrueForAll(optionIds, uid => selectedIdSet.Contains(uid));
			if (everything) {
				label = "Everything";
			} else {
				List<string> selectedNames = new List<string>();
				for (int i = 0, length = optionIds.Length; i < length; ++i) {
					int optionId = optionIds[i];
					if (selectedIdSet.Contains(optionId)) {
						selectedNames.Add(displayedOptions[i]);
					}
				}
				label = selectedNames.Count > 0 ? string.Join(",", selectedNames) : "Nothing";
			}
			HashSet<int> optionIdSet = new HashSet<int>(optionIds);
			bool selectionIsDirty = Array.Exists(selectedIds, uid => !optionIdSet.Contains(uid));
			if (selectionIsDirty) {
				label += " (Dirty)";
			}
			PopupMultiSelector.IntPopupMultiSelect(onChange, label, selectedIds, displayedOptions, optionIds);
		}
	}
}
