/*
 * @Author: wangyun
 * @CreateTime: 2024-07-01 00:21:17 715
 * @LastEditor: wangyun
 * @EditTime: 2024-07-01 00:21:17 719
 */

using UnityEditor;
using UnityEngine;

public static class GetFieldPathDemo {
	[MenuItem("Memo/Memo6/GetFieldPathDemo")]
	public static void Test() {
		Debug.Log( FieldPathUtils.GetFieldPath((GameObject go) => go.transform.position.magnitude));
	}
}
