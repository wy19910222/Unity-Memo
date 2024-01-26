/*
 * @Author: wangyun
 * @CreateTime: 2024-01-25 18:55:48 302
 * @LastEditor: wangyun
 * @EditTime: 2024-01-25 18:55:48 306
 */

using UnityEngine;
using UnityEditor;

namespace Memo1_IMESettings.Editor {
	public static class IMESettings {
		[MenuItem("Memo/Memo1/IMECompositionModeAuto")]
		private static void IMECompositionModeAuto() {
			Input.imeCompositionMode = IMECompositionMode.Auto;
		}
		[MenuItem("Memo/Memo1/IMECompositionModeOn")]
		private static void IMECompositionModeOn() {
			Input.imeCompositionMode = IMECompositionMode.On;
		}
		[MenuItem("Memo/Memo1/IMECompositionModeOff")]
		private static void IMECompositionModeOff() {
			Input.imeCompositionMode = IMECompositionMode.Off;
		}
		[MenuItem("Memo/Memo1/LogIMECompositionMode")]
		private static void LogIMECompositionMode() {
			Debug.Log(Input.imeCompositionMode);;
		}
	}
}
