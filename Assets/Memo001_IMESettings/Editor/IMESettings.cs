/*
 * @Author: wangyun
 * @CreateTime: 2024-01-25 18:55:48 302
 * @LastEditor: wangyun
 * @EditTime: 2024-01-25 18:55:48 306
 */

using UnityEngine;
using UnityEditor;

namespace Memo001_IMESettings.Editor {
	public static class IMESettings {
		[MenuItem("Memo/Memo001/IMECompositionModeAuto", priority = 1)]
		private static void IMECompositionModeAuto() {
			Input.imeCompositionMode = IMECompositionMode.Auto;
		}
		[MenuItem("Memo/Memo001/IMECompositionModeOn", priority = 1)]
		private static void IMECompositionModeOn() {
			Input.imeCompositionMode = IMECompositionMode.On;
		}
		[MenuItem("Memo/Memo001/IMECompositionModeOff", priority = 1)]
		private static void IMECompositionModeOff() {
			Input.imeCompositionMode = IMECompositionMode.Off;
		}
		[MenuItem("Memo/Memo001/LogIMECompositionMode", priority = 1)]
		private static void LogIMECompositionMode() {
			Debug.Log(Input.imeCompositionMode);;
		}
	}
}
