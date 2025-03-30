/*
 * @Author: wangyun
 * @CreateTime: 2025-03-28 21:44:19 676
 * @LastEditor: wangyun
 * @EditTime: 2025-03-28 21:44:19 680
 */

using UnityEditor;
using UnityEngine;

namespace Memo014_DynamicAddMenuItems.Editor {
	public static class DynamicAddMenuItems {
		[MenuItem("Memo/Memo014/DynamicAddMenuItems", priority = 14)]
		private static void AddMenuItems() {
			for (int i = 0; i < 10; i++) {
				string menuName = $"Memo014Example/MenuItem{i}";
				Menu.AddMenuItem(menuName, string.Empty, false, 0, () => Debug.Log($"{menuName} Clicked!"), null);
			}
			EditorUtility.Internal_UpdateAllMenus();
		}
		
		[MenuItem("Memo/Memo014/DynamicRemoveMenuItem", priority = 14)]
		private static void RemoveMenuItems() {
			foreach (ScriptingMenuItem scriptingMenuItem in Menu.GetMenuItems("Memo014Example/", false, true)) {
				Menu.RemoveMenuItem(scriptingMenuItem.path);
			}
			// To make the refresh works
			Menu.AddMenuItem("Help/", string.Empty, false, 0, () => { }, null);
			EditorUtility.Internal_UpdateAllMenus();
		}
	}
}
