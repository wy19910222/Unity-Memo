/*
 * @Author: wangyun
 * @CreateTime: 2024-01-26 14:31:35 494
 * @LastEditor: wangyun
 * @EditTime: 2024-01-26 14:31:35 497
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Memo3_ComponentReplace.Editor {
	public static class Converter_Default {
		public static void CompConverter(MonoBehaviour fromBehaviour, MonoBehaviour toBehaviour) {
			// 在这里把fromBehaviour的所有数据复制给toBehaviour
		}

		public static void ModificationConverter(List<PropertyModification> modifications) {
			// 在这里把针对原组件的Modification改为针对现组件的Modification
		}
	}
}