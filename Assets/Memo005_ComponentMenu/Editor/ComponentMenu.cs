/*
 * @Author: wangyun
 * @CreateTime: 2024-01-27 00:27:49 808
 * @LastEditor: wangyun
 * @EditTime: 2024-01-27 00:27:49 812
 */

using UnityEditor;
using UnityEngine;

namespace Memo5_ComponentMenu.Editor {
	public static class ComponentMenu {
		[MenuItem("CONTEXT/Component/LogComponent")]
		private static void LogComponent(MenuCommand command) {
			Debug.Log(command.context);
		}
	}
}
