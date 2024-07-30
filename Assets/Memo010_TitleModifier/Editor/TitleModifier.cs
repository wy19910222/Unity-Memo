﻿using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace ToolKits {
	public static class TitleModifier {
		[InitializeOnLoadMethod]
		private static void Init() {
			EditorApplication.updateMainWindowTitle += OnUpdateMainWindowTitle;
		}

		[MenuItem("Memo/Memo010/UpdateMainWindowTitle", priority = 10)]
		private static void UpdateMainWindowTitle() {
			EditorApplication.UpdateMainWindowTitle();
		}

		private static void OnUpdateMainWindowTitle(ApplicationTitleDescriptor desc) {
			string currentGitBranchName = GetCurrentGitBranchName();
			if (!string.IsNullOrEmpty(currentGitBranchName)) {
				desc.title += $" - {currentGitBranchName}";
			}
		}
		
		private static string GetCurrentGitBranchName() {
			ProcessStartInfo startInfo = new ProcessStartInfo() {
				FileName = "git",
				Arguments = "rev-parse --abbrev-ref HEAD",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};
			try {
				using (Process process = Process.Start(startInfo)) {
					using (StreamReader reader = process?.StandardOutput) {
						string readToEnd = reader?.ReadToEnd();
						return readToEnd?.Trim();
					}
				}
			} catch (Win32Exception ex) {
				// 处理找不到 git 可执行文件的情况
				UnityEngine.Debug.LogError($"{ex.GetType().Name}: {ex.Message}");
				return null;
			}
		}
	}
}
