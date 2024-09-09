/*
 * @Author: wangyun
 * @CreateTime: 2023-01-26 22:26:26 084
 * @LastEditor: wangyun
 * @EditTime: 2023-01-26 22:26:26 090
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Memo012_CustomEditorGUI.Editor {
	public static class CustomEditorGUI {
		private static readonly Stack<float> s_LabelWidthStack = new Stack<float>();
		public static void BeginLabelWidth(float labelWidth) {
			s_LabelWidthStack.Push(EditorGUIUtility.labelWidth);
			EditorGUIUtility.labelWidth = labelWidth;
		}
		public static void EndLabelWidth() {
			if (s_LabelWidthStack.Count == 0) {
				Debug.LogError("LabelWidth stack is empty, did you call BeginLabelWidth first?");
			} else {
				EditorGUIUtility.labelWidth = s_LabelWidthStack.Pop();
			}
		}
		
		private static readonly Stack<float> s_FieldWidthStack = new Stack<float>();
		public static void BeginFieldWidth(float fieldWidth) {
			s_FieldWidthStack.Push(EditorGUIUtility.fieldWidth);
			EditorGUIUtility.fieldWidth = fieldWidth;
		}
		public static void EndFieldWidth() {
			if (s_FieldWidthStack.Count == 0) {
				Debug.LogError("FieldWidth stack is empty, did you call BeginFieldWidth first?");
			} else {
				EditorGUIUtility.fieldWidth = s_FieldWidthStack.Pop();
			}
		}
		
		private static readonly Stack<Vector2> s_IconSizeStack = new Stack<Vector2>();
		public static void BeginIconSize(float iconWidth, float iconHeight) {
			s_IconSizeStack.Push(EditorGUIUtility.GetIconSize());
			EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconHeight));
		}
		public static void EndIconSize() {
			if (s_IconSizeStack.Count == 0) {
				Debug.LogError("IconSize stack is empty, did you call BeginIconSize first?");
			} else {
				EditorGUIUtility.SetIconSize(s_IconSizeStack.Pop());
			}
		}
		
		private static readonly Stack<Color> s_ColorStack = new Stack<Color>();
		public static void BeginColor(Color color) {
			s_ColorStack.Push(GUI.color);
			GUI.color = color;
		}
		public static void EndColor() {
			if (s_ColorStack.Count == 0) {
				Debug.LogError("Color stack is empty, did you call BeginColor first?");
			} else {
				GUI.color = s_ColorStack.Pop();
			}
		}
		
		private static readonly Stack<Color> s_ContentColorStack = new Stack<Color>();
		public static void BeginContentColor(Color color) {
			s_ContentColorStack.Push(GUI.contentColor);
			GUI.contentColor = color;
		}
		public static void EndContentColor() {
			if (s_ContentColorStack.Count == 0) {
				Debug.LogError("ContentColor stack is empty, did you call BeginContentColor first?");
			} else {
				GUI.contentColor = s_ContentColorStack.Pop();
			}
		}
		
		private static readonly Stack<Color> s_BackgroundColorStack = new Stack<Color>();
		public static void BeginBackgroundColor(Color color) {
			s_BackgroundColorStack.Push(GUI.backgroundColor);
			GUI.backgroundColor = color;
		}
		public static void EndBackgroundColor() {
			if (s_BackgroundColorStack.Count == 0) {
				Debug.LogError("BackgroundColor stack is empty, did you call BeginBackgroundColor first?");
			} else {
				GUI.backgroundColor = s_BackgroundColorStack.Pop();
			}
		}
		
		private static readonly Stack<(float, Vector2)> s_RotateStack = new Stack<(float, Vector2)>();
		public static void BeginRotate(float angle) {
			BeginRotate(angle, GetNextRect().min);
		}
		public static void BeginRotate(float angle, Vector2 pivotPoint) {
			s_RotateStack.Push((angle, pivotPoint));
			GUIUtility.RotateAroundPivot(angle, pivotPoint);
		}
		public static void EndRotate() {
			if (s_RotateStack.Count == 0) {
				Debug.LogError("Rotate stack is empty, did you call BeginRotate first?");
			} else {
				(float angle, Vector2 pivotPoint) = s_RotateStack.Pop();
				GUIUtility.RotateAroundPivot(-angle, pivotPoint);
			}
		}
		
		private static readonly Stack<(Vector2, Vector2)> s_ScaleStack = new Stack<(Vector2, Vector2)>();
		public static void BeginScale(Vector2 scale) {
			BeginScale(scale, GetNextRect().min);
		}
		public static void BeginScale(Vector2 scale, Vector2 pivotPoint) {
			s_ScaleStack.Push((scale, pivotPoint));
			GUIUtility.ScaleAroundPivot(scale, pivotPoint);
		}
		public static void EndScale() {
			if (s_ScaleStack.Count == 0) {
				Debug.LogError("Rotate stack is empty, did you call BeginScale first?");
			} else {
				(Vector2 scale, Vector2 pivotPoint) = s_ScaleStack.Pop();
				GUIUtility.ScaleAroundPivot(Vector2.one / scale, pivotPoint);
			}
		}
		
		public static void BeginDisabled(bool disabled) {
			EditorGUI.BeginDisabledGroup(disabled);
		}
		public static void EndDisabled() {
			EditorGUI.EndDisabledGroup();
		}
		public static void ChangeDisabled(bool disabled) {
			EndDisabled();
			BeginDisabled(disabled);
		}
		
		private static readonly Stack<bool> s_BoldStack = new Stack<bool>();
		public static void BeginBold(bool isBold) {
			s_BoldStack.Push(IsBold());
			SetBold(isBold);
		}
		public static void EndBold() {
			if (s_BoldStack.Count == 0) {
				Debug.LogError("Bold stack is empty, did you call BeginBold first?");
			} else {
				bool isBold = s_BoldStack.Pop();
				SetBold(isBold);
			}
		}
		private static bool IsBold() {
			// return EditorGUIUtility.GetBoldDefaultFont();
			MethodInfo getBoldDefaultFontMI = typeof(EditorGUIUtility).GetMethod("GetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
			return getBoldDefaultFontMI?.Invoke(null, null) is bool isBold && isBold;
		}
		private static void SetBold(bool isBold) {
			// EditorGUIUtility.SetBoldDefaultFont(isBold);
			MethodInfo getBoldDefaultFontMI = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
			getBoldDefaultFontMI?.Invoke(null, new object[] { isBold });
		}
		
	
		public static void Space(float space, bool expand = true) {
			if (expand) {
				if (IsVertical()) {
					GUILayoutUtility.GetRect(0, space, GUILayout.ExpandWidth(true));
				} else {
					GUILayoutUtility.GetRect(space, 0, GUILayout.ExpandWidth(true));
				}
				if (Event.current.type == EventType.Layout) {
					// GUILayoutUtility.current.topLevel.entries[GUILayoutUtility.current.topLevel.entries.Count - 1].consideredForMargin = false;
					object topLevel = GetTopLevel();
					FieldInfo entriesFI = topLevel?.GetType().GetField("entries", BindingFlags.Instance | BindingFlags.Public);
					IList entries = entriesFI?.GetValue(topLevel) as IList;
					object last = entries?[entries.Count - 1];
					FieldInfo consideredForMarginFI = last?.GetType().GetField("consideredForMargin", BindingFlags.Instance | BindingFlags.Public);
					consideredForMarginFI?.SetValue(last, false);
				}
			} else {
				GUILayout.Space(space);
			}
		}
		
		public static float GetContextWidth() {
			// return EditorGUIUtility.contextWidth;
			PropertyInfo pi = typeof(EditorGUIUtility).GetProperty("contextWidth", BindingFlags.Static | BindingFlags.NonPublic);
			return (float) (pi?.GetValue(null) ?? EditorGUIUtility.currentViewWidth);
		}
		
		public static bool IsVertical() {
			// return GUILayoutUtility.current.topLevel.isVertical;
			object topLevel = GetTopLevel();
			FieldInfo isVerticalFI = topLevel?.GetType().GetField("isVertical", BindingFlags.Instance | BindingFlags.Public);
			return isVerticalFI?.GetValue(topLevel) is bool bIsVertical && bIsVertical;
		}
		
		public static int GetCursor() {
			// return GUILayoutUtility.current.topLevel.m_Cursor;
			object topLevel = GetTopLevel();
			FieldInfo cursorFI = topLevel?.GetType().GetField("m_Cursor", BindingFlags.Instance | BindingFlags.NonPublic);
			return cursorFI?.GetValue(topLevel) is int iCursor ? iCursor : 0;
		}
		
		public static bool IsPrefabComparing() {
			// return (GUIView.current as HostView)?.actualView is PopupWindowWithoutFocus;
			Type viewType = typeof(EditorGUIUtility).Assembly.GetType("UnityEditor.GUIView");
			PropertyInfo currentPI = viewType?.GetProperty("current", BindingFlags.Static | BindingFlags.Public);
			object view = currentPI?.GetValue(null);
			PropertyInfo actualViewPI = view?.GetType().GetProperty("actualView", BindingFlags.Instance | BindingFlags.NonPublic);
			object window = actualViewPI?.GetValue(view);
			return window?.GetType().Name == "PopupWindowWithoutFocus";
		}
		
		public static Rect GetLastRect() {
			switch (Event.current.type) {
				case EventType.Layout:
				case EventType.Repaint:
					return GUILayoutUtility.GetLastRect();
				default:
					// return EditorGUILayout.s_LastRect;
					FieldInfo lastRectFI = typeof(EditorGUILayout).GetField("s_LastRect", BindingFlags.Static | BindingFlags.NonPublic);
					return lastRectFI?.GetValue(null) is Rect rect ? rect : new Rect();
			}
		}
		
		public static Rect GetNextRect() {
			// GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
			// return topLevel.m_Cursor > 0 ? topLevel.PeekNext() : topLevel.rect;
			object topLevel = GetTopLevel();
			FieldInfo cursorFI = topLevel?.GetType().GetField("m_Cursor", BindingFlags.Instance | BindingFlags.NonPublic);
			if (cursorFI?.GetValue(topLevel) is int iCursor && iCursor > 0) {
				MethodInfo peekNextFI = topLevel.GetType().GetMethod("PeekNext", BindingFlags.Instance | BindingFlags.Public);
				return peekNextFI?.Invoke(topLevel, null) is Rect rect ? rect : new Rect();
			} else {
				FieldInfo rectFI = topLevel?.GetType().GetField("rect", BindingFlags.Instance | BindingFlags.Public);
				return rectFI?.GetValue(topLevel) is Rect rect ? rect : new Rect();
			}
		}
		
		public static object GetTopLevel() {
			// return GUILayoutUtility.current.topLevel;
			FieldInfo currentFI = typeof(GUILayoutUtility).GetField("current", BindingFlags.Static | BindingFlags.NonPublic);
			object current = currentFI?.GetValue(null);
			FieldInfo topLevelFI = current?.GetType().GetField("topLevel", BindingFlags.Instance | BindingFlags.NonPublic);
			return topLevelFI?.GetValue(current);
		}
		
		public static void Repaint() {
			// GUIView.current.Repaint();
			Type viewType = typeof(EditorGUIUtility).Assembly.GetType("UnityEditor.GUIView");
			PropertyInfo currentPI = viewType?.GetProperty("current", BindingFlags.Static | BindingFlags.Public);
			object view = currentPI?.GetValue(null);
			MethodInfo repaintMI = viewType?.GetMethod("Repaint", BindingFlags.Instance | BindingFlags.Public);
			repaintMI?.Invoke(view, null);
		}
	
		public static void RepaintAllInspectors() {
			// InspectorWindow.RepaintAllInspectors();
			Type inspectorType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
			MethodInfo repaintAllInspectorsFI = inspectorType?.GetMethod("RepaintAllInspectors", BindingFlags.Static | BindingFlags.NonPublic);
			repaintAllInspectorsFI?.Invoke(null, null);
			// // InspectorWindow.m_AllInspectors.ForEach(inspector => inspector.Repaint());
			// Type inspectorType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
			// FieldInfo allInspectorsFI = inspectorType?.GetField("m_AllInspectors", BindingFlags.Static | BindingFlags.NonPublic);
			// if (allInspectorsFI?.GetValue(null) is IList allInspectors) {
			// 	foreach (var inspector in allInspectors) {
			// 		if (inspector is EditorWindow window) {
			// 			window.Repaint();
			// 		}
			// 	}
			// }
		}
	
		public static void RepaintEditorWindows<T>() where T : EditorWindow {
			T[] windows = Resources.FindObjectsOfTypeAll<T>();
			foreach (var window in windows) {
				window.Repaint();
			}
		}
	
		public static void RepaintScene() {
			EditorApplication.QueuePlayerLoopUpdate();
		}
	
		public static void RepaintSceneImmediate() {
			SceneView.RepaintAll();
		}
	
		public static void RepaintAllViews() {
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}
	}
}
