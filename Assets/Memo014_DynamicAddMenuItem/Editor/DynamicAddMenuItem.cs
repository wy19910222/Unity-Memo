/*
 * @Author: wangyun
 * @CreateTime: 2025-03-28 21:44:19 676
 * @LastEditor: wangyun
 * @EditTime: 2025-03-28 21:44:19 680
 */

using UnityEditor;
using UnityEngine;

namespace Memo014_DynamicAddMenuItem.Editor {
	public static class DynamicAddMenuItem {
		[MenuItem("Memo/Memo014/DynamicAddMenuItem", priority = 14)]
		private static void AddMenuItem() {
			for (int i = 0; i < 10; i++) {
				string menuName = $"Memo014Example/MenuItem{i}";
				Menu.AddMenuItem(menuName, string.Empty, false, 0, () => Debug.Log($"{menuName} Clicked!"), null);
			}
			EditorUtility.Internal_UpdateAllMenus();
		}
	}
}
