/*
 * @Author: wangyun
 * @CreateTime: 2024-08-08 21:44:43 731
 * @LastEditor: wangyun
 * @EditTime: 2024-08-08 21:44:43 735
 */

using System;
using UnityEditor;
using UnityEngine;

namespace Memo012_CustomEditorGUI.Editor {
	public class PopupContent : PopupWindowContent {
		private float Width { get; }
		private float Height { get; }
		private Action<Rect> OnDraw { get; }

		public PopupContent(float width, float height, Action<Rect> onDraw) {
			Width = width;
			Height = height;
			OnDraw = onDraw;
		}

		public override void OnGUI(Rect rect) => OnDraw?.Invoke(rect);

		public override Vector2 GetWindowSize() => new Vector2(Width, Height);
	}
}
