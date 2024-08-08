/*
 * @Author: wangyun
 * @CreateTime: 2024-08-08 21:54:02 998
 * @LastEditor: wangyun
 * @EditTime: 2024-08-08 21:54:03 005
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Memo012_CustomEditorGUI.Editor {
	public static class PopupMultiSelector {
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, int[] selectedValues, string[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, (GUIContent) null, selectedValues, displayedOptions, optionValues);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, string label, int[] selectedValues, string[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, label, selectedValues, displayedOptions, optionValues, "MiniPullDown");
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, GUIContent content, int[] selectedValues, string[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, content, selectedValues, displayedOptions, optionValues, "MiniPullDown");
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, string label, int[] selectedValues, string[] displayedOptions, int[] optionValues, GUIStyle style) {
			IntPopupMultiSelect(onChange, rect, label == null ? null : EditorGUIUtility.TrTempContent(label), selectedValues, displayedOptions, optionValues, style);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, GUIContent content, int[] selectedValues, string[] displayedOptions, int[] optionValues, GUIStyle style) {
			IntPopupMultiSelect(onChange, rect, content, selectedValues, EditorGUIUtility.TrTempContent(displayedOptions), optionValues, style);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, (GUIContent) null, selectedValues, displayedOptions, optionValues);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, string label, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, label, selectedValues, displayedOptions, optionValues, "MiniPullDown");
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, GUIContent content, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, content, selectedValues, displayedOptions, optionValues, "MiniPullDown");
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, string label, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style) {
			IntPopupMultiSelect(onChange, rect, label == null ? null : EditorGUIUtility.TrTempContent(label), selectedValues, displayedOptions, optionValues, style);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, GUIContent content, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style) {
			(bool[] isSelecteds, List<GUIContent> displayedContents) = GetSelects(selectedValues, displayedOptions, optionValues);
			PopupMultiSelect(onChange == null ? null : _isSelects => {
				List<int> selectValueList = new List<int>();
				for (int i = 0, optionLength = optionValues.Length; i < optionLength; ++i) {
					if (_isSelects[i]) {
						selectValueList.Add(optionValues[i]);
					}
				}
				onChange(selectValueList.ToArray());
			}, rect, content, isSelecteds, displayedContents.ToArray(), style);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, int[] selectedValues, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, (GUIContent) null, selectedValues, displayedOptions, optionValues, options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, string label, int[] selectedValues, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, label, selectedValues, displayedOptions, optionValues, "MiniPullDown", options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, GUIContent content, int[] selectedValues, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, content, selectedValues, displayedOptions, optionValues, "MiniPullDown", options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, string label, int[] selectedValues, string[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, label == null ? null : EditorGUIUtility.TrTempContent(label), selectedValues, displayedOptions, optionValues, style, options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, GUIContent content, int[] selectedValues, string[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, content, selectedValues, EditorGUIUtility.TrTempContent(displayedOptions), optionValues, style, options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, (GUIContent) null, selectedValues, displayedOptions, optionValues, options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, string label, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, label, selectedValues, displayedOptions, optionValues, "MiniPullDown", options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, GUIContent content, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, content, selectedValues, displayedOptions, optionValues, "MiniPullDown", options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, string label, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, label == null ? null : EditorGUIUtility.TrTempContent(label), selectedValues, displayedOptions, optionValues, style, options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, GUIContent content, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options) {
			(bool[] isSelecteds, List<GUIContent> displayedContents) = GetSelects(selectedValues, displayedOptions, optionValues);
			PopupMultiSelect(onChange == null ? null : newIsSelects => {
				List<int> selectValueList = new List<int>();
				for (int i = 0, optionLength = optionValues.Length; i < optionLength; ++i) {
					if (newIsSelects[i]) {
						selectValueList.Add(optionValues[i]);
					}
				}
				onChange(selectValueList.ToArray());
			}, content, isSelecteds, displayedContents.ToArray(), style, options);
		}
		private static (bool[] isSelecteds, List<GUIContent> displayedContents) GetSelects(int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues) {
			int optionLength = optionValues.Length;
			List<GUIContent> displayedContents = new List<GUIContent>(displayedOptions);
			for (int i = displayedContents.Count; i < optionLength; ++i) {
				displayedContents.Add(EditorGUIUtility.TrTempContent(optionValues[i] + ""));
			}
			for (int i = displayedContents.Count - 1; i >= optionLength; --i) {
				displayedContents.RemoveAt(i);
			}
			HashSet<int> selectedValueSet = new HashSet<int>(selectedValues);
			bool[] isSelecteds = new bool[optionLength];
			for (int i = 0; i < optionLength; ++i) {
				isSelecteds[i] = selectedValueSet.Contains(optionValues[i]);
			}
			return (isSelecteds, displayedContents);
		}
		
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, bool[] isSelecteds, string[] displayedOptions) {
			PopupMultiSelect(onChange, rect, (GUIContent) null, isSelecteds, displayedOptions);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, string label, bool[] isSelecteds, string[] displayedOptions) {
			PopupMultiSelect(onChange, rect, label, isSelecteds, displayedOptions, "MiniPullDown");
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, GUIContent content, bool[] isSelecteds, string[] displayedOptions) {
			PopupMultiSelect(onChange, rect, content, isSelecteds, displayedOptions, "MiniPullDown");
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, string label, bool[] isSelecteds, string[] displayedOptions, GUIStyle style) {
			PopupMultiSelect(onChange, rect, EditorGUIUtility.TrTempContent(label), isSelecteds, displayedOptions, style);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, GUIContent content, bool[] isSelecteds, string[] displayedOptions, GUIStyle style) {
			PopupMultiSelect(onChange, rect, content, isSelecteds, EditorGUIUtility.TrTempContent(displayedOptions), style);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, bool[] isSelecteds, GUIContent[] displayedOptions) {
			PopupMultiSelect(onChange, rect, (GUIContent) null, isSelecteds, displayedOptions);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, string label, bool[] isSelecteds, GUIContent[] displayedOptions) {
			PopupMultiSelect(onChange, rect, label, isSelecteds, displayedOptions, "MiniPullDown");
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, GUIContent content, bool[] isSelecteds, GUIContent[] displayedOptions) {
			PopupMultiSelect(onChange, rect, content, isSelecteds, displayedOptions, "MiniPullDown");
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, string label, bool[] isSelecteds, GUIContent[] displayedOptions, GUIStyle style) {
			PopupMultiSelect(onChange, rect, EditorGUIUtility.TrTempContent(label), isSelecteds, displayedOptions, style);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, GUIContent content, bool[] isSelecteds, GUIContent[] displayedOptions, GUIStyle style) {
			if (content == null) {
				content = GetContent(isSelecteds, displayedOptions);
				Debug.LogError(content);
			}
			if (EditorGUI.DropdownButton(rect, content, FocusType.Keyboard, style)) {
				PopupWindow.Show(rect, new PopupMultiSelectContent(onChange, isSelecteds, displayedOptions));
			}
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, bool[] isSelecteds, string[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, (GUIContent) null, isSelecteds, displayedOptions, options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, string label, bool[] isSelecteds, string[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, label, isSelecteds, displayedOptions, "MiniPullDown", options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, GUIContent content, bool[] isSelecteds, string[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, content, isSelecteds, displayedOptions, "MiniPullDown", options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, string label, bool[] isSelecteds, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, EditorGUIUtility.TrTempContent(label), isSelecteds, displayedOptions, style, options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, GUIContent content, bool[] isSelecteds, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, content, isSelecteds, EditorGUIUtility.TrTempContent(displayedOptions), style, options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, bool[] isSelecteds, GUIContent[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, (GUIContent) null, isSelecteds, displayedOptions, options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, string label, bool[] isSelecteds, GUIContent[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, label, isSelecteds, displayedOptions, "MiniPullDown", options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, GUIContent content, bool[] isSelecteds, GUIContent[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, content, isSelecteds, displayedOptions, "MiniPullDown", options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, string label, bool[] isSelecteds, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, EditorGUIUtility.TrTempContent(label), isSelecteds, displayedOptions, style, options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, GUIContent content, bool[] isSelecteds, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options) {
			if (content == null) {
				content = GetContent(isSelecteds, displayedOptions);
			}
			if (EditorGUILayout.DropdownButton(content, FocusType.Keyboard, style, options)) {
				PopupWindow.Show(CustomEditorGUI.GetLastRect(), new PopupMultiSelectContent(onChange, isSelecteds, displayedOptions));
			}
		}
		private static GUIContent GetContent(IReadOnlyList<bool> isSelecteds, IReadOnlyList<GUIContent> displayedOptions) {
			int isSelectLength = isSelecteds.Count;
			bool nothing = true;
			for (int i = 0; i < isSelectLength; ++i) {
				if (isSelecteds[i]) {
					nothing = false;
					break;
				}
			}
			if (nothing) {
				return EditorGUIUtility.TrTempContent("Nothing");
			}
			
			bool everything = true;
			int optionLength = displayedOptions.Count;
			if (isSelectLength != optionLength) {
				everything = false;
			} else {
				for (int i = 0; i < isSelectLength; ++i) {
					if (!isSelecteds[i]) {
						everything = false;
						break;
					}
				}
			}
			if (everything) {
				return EditorGUIUtility.TrTempContent("Everything");
			}
			
			List<string> list = new List<string>();
			for (int i = 0; i < isSelectLength; ++i) {
				if (isSelecteds[i]) {
					list.Add(i < optionLength ? displayedOptions[i].text : string.Empty);
				}
			}
			return EditorGUIUtility.TrTempContent(string.Join(",", list));
		}
		
		public class PopupMultiSelectContent : PopupWindowContent {
			private const float WIDTH_MIN = 100F;
			private const float WIDTH_MAX = 200F;
			private const float HEIGHT_MAX = 600F;
			private const float LINE_HEIGHT = 18F;
			
			private static readonly GUIStyle s_Style = "MenuToggleItem";
			
			private Action<bool[]> OnChange { get; }
			private List<bool> IsSelects { get; }
			private List<GUIContent> DisplayedOptions { get; }

			private Vector2 m_ScrollPosition;

			public PopupMultiSelectContent(Action<bool[]> onChange, bool[] isSelects, GUIContent[] displayedOptions) {
				OnChange = onChange;

				IsSelects = new List<bool>(isSelects);
				DisplayedOptions = new List<GUIContent>(displayedOptions);
				
				int optionCount = DisplayedOptions.Count;
				for (int i = IsSelects.Count; i < optionCount; ++i) {
					IsSelects.Add(false);
				}
				for (int i = IsSelects.Count - 1; i >= optionCount; --i) {
					IsSelects.RemoveAt(i);
				}
			}

			public override void OnGUI(Rect rect) {
				int optionCount = DisplayedOptions.Count;
				
				m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
				EditorGUI.BeginChangeCheck();
				
				bool everythingSelected = IsSelects.TrueForAll(select => select);
				bool newEverythingSelected = GUILayout.Toggle(everythingSelected, "All", "Button", GUILayout.Height(LINE_HEIGHT));
				if (newEverythingSelected != everythingSelected) {
					for (int i = 0; i < optionCount; ++i) {
						IsSelects[i] = newEverythingSelected;
					}
				}
				
				if (GUILayout.Button("Revert", "Button", GUILayout.Height(LINE_HEIGHT))) {
					for (int i = 0; i < optionCount; ++i) {
						IsSelects[i] = !IsSelects[i];
					}
				}
				
				for (int i = 0; i < optionCount; ++i) {
					IsSelects[i] = GUILayout.Toggle(IsSelects[i], DisplayedOptions[i], s_Style, GUILayout.Height(LINE_HEIGHT));
				}
				
				if (EditorGUI.EndChangeCheck()) {
					OnChange?.Invoke(IsSelects.ToArray());
				}
				EditorGUILayout.EndScrollView();
			}
			
			public override Vector2 GetWindowSize() {
				float width = 0;
				float height = 0;
				foreach (var displayedOption in DisplayedOptions) {
					float _width = s_Style.CalcSize(displayedOption).x;
					if (width < _width) {
						width = _width;
					}
					height += LINE_HEIGHT;
				}
				float scrollBarWidth = 0;
				float scrollBarHeight = 0;
				if (width > WIDTH_MAX) {
					width = WIDTH_MAX;
					scrollBarHeight = 13F;
				}
				if (height > HEIGHT_MAX) {
					height = HEIGHT_MAX;
					scrollBarWidth = 13F;
				}
				width = Mathf.Max(width, WIDTH_MIN) + scrollBarWidth;
				height = height + 2F + LINE_HEIGHT + 2F + LINE_HEIGHT + 2F + scrollBarHeight;
				return new Vector2(width, height);
			}
		}
	}
}
