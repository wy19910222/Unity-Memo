/*
 * @Author: wangyun
 * @CreateTime: 2024-07-01 00:21:17 715
 * @LastEditor: wangyun
 * @EditTime: 2024-07-01 00:21:17 719
 */

using UnityEditor;
using UnityEngine;

namespace Memo006_GetFieldPath.Editor {
	public static class GetFieldPathDemo {
		[MenuItem("Memo/Memo006/GetFieldPathDemo", priority = 6)]
		public static void Test() {
			Debug.Log( FieldPathUtils.GetFieldPath((GameObject go) => go.transform.position.magnitude));
		}
	}
}
