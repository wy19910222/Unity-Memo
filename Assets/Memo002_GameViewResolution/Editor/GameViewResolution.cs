/*
 * @Author: wangyun
 * @CreateTime: 2024-01-26 17:08:14 874
 * @LastEditor: wangyun
 * @EditTime: 2024-01-26 17:08:14 881
 */

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Memo2_GameViewResolution.Editor {
	public static class GameViewResolution {
		[MenuItem("Memo/Memo002/SetGameViewResolution_1920_1080", priority = 2)]
		private static void SetGameViewResolution_1920_1080() {
			ForceSetGameViewResolution(1920, 1080, true);
		}

		public static void ForceSetGameViewResolution(int width, int height, bool isFixed, string baseText = null) {
			GameViewSizeType sizeType = isFixed ? GameViewSizeType.FixedResolution : GameViewSizeType.AspectRatio;
			GameViewSizes gameViewSizesInstance = ScriptableSingleton<GameViewSizes>.instance;
			GameViewSizeGroup group = gameViewSizesInstance.GetGroup(gameViewSizesInstance.currentGroupType);
			int resolutionCount = group.GetTotalCount();
			for (int i = 0; i < resolutionCount; ++i) {
				GameViewSize size = group.GetGameViewSize(i);
				if (size.width == width && size.height == height && size.sizeType == sizeType) {
					CurrentIndex = i;
					return;
				}
			}
			group.AddCustomSize(new GameViewSize(sizeType, width, height, baseText ?? $"{width}:{height}"));
			CurrentIndex = resolutionCount;
		}

		public static void AddGameViewSize(string baseText, bool isFixed, int width, int height) {
			GameViewSizeType sizeType = isFixed ? GameViewSizeType.FixedResolution : GameViewSizeType.AspectRatio;
			GameViewSizeGroup.AddCustomSize(new GameViewSize(sizeType, width, height, baseText ?? $"{width}:{height}"));
		}
		public static bool RemoveGameViewSizeAt(int index) {
			GameViewSizeGroup group = GameViewSizeGroup;
			index -= group.GetBuiltinCount();
			if (index >= 0 && index < group.GetCustomCount()) {
				group.RemoveCustomSize(index);
				return true;
			}
			return false;
		}
		public static bool UpdateGameViewSize(int index, string baseText, bool isFixed, int width, int height) {
			GameViewSizeGroup group = GameViewSizeGroup;
			if (index >= 0 && index < group.GetTotalCount()) {
				GameViewSize size = group.GetGameViewSize(index);
				size.baseText = baseText;
				size.sizeType = isFixed ? GameViewSizeType.FixedResolution : GameViewSizeType.AspectRatio;
				size.width = width;
				size.height = height;
				return true;
			}
			return false;
		}

		private static int IndexOfGameViewSize(Func<string, bool, int, int, bool> match) {
			GameViewSizeGroup group = GameViewSizeGroup;
			int resolutionCount = group.GetTotalCount();
			for (int i = 0; i < resolutionCount; ++i) {
				GameViewSize size = group.GetGameViewSize(i);
				if (match(size.baseText, size.sizeType == GameViewSizeType.FixedResolution, size.width, size.height)) {
					return i;
				}
			}
			return -1;
		}

		public static int GameViewSizeCount => GameViewSizeGroup.GetTotalCount();

		public static (string baseText, bool isFixed, int width, int height) GetGameViewSize(int index) {
			if (index < 0) {
				Debug.LogError("Invalid index: {index}");
				return ("", false, 0, 0);
			} else {
				GameViewSize size = GameViewSizeGroup.GetGameViewSize(index);
				return (size.baseText, size.sizeType == GameViewSizeType.FixedResolution, size.width, size.height);
			}
		}

		public static (string baseText, bool isFixed, int width, int height) CurrentGameViewSize => GetGameViewSize(CurrentIndex);

		public static int CurrentIndex {
			get {
				// EditorWindow.GetWindow<GameView>().selectedSizeIndex;
				PropertyInfo selectedSizeIndexPI = typeof(GameView).GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic);
				return selectedSizeIndexPI?.GetValue(EditorWindow.GetWindow<GameView>()) is int selectedSizeIndex ? selectedSizeIndex : -1;
			}
			set {
				GameView gameView = EditorWindow.GetWindow<GameView>();
				gameView.SizeSelectionCallback(value, null);
			}
		}

		private static GameViewSizeGroup GameViewSizeGroup {
			get {
				GameViewSizes gameViewSizesInstance = ScriptableSingleton<GameViewSizes>.instance;
				return gameViewSizesInstance.GetGroup(gameViewSizesInstance.currentGroupType);
			}
		}
	}
}
