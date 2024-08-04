using UnityEditor;
using UnityEngine;

namespace Memo11_SharedMemory.Editor {
	public class SharedMemorySampleWindow : EditorWindow {
		[MenuItem("Memo/Memo011/SharedMemorySampleWindow", priority = 11)]
		public static void ShowWindow() {
			var window = GetWindow<SharedMemorySampleWindow>();
			window.minSize = new Vector2(300F, 160F);
			window.Show();
		}

		private string m_Words;
		private SharedMemory m_SharedMemory;

		private void OnEnable() {
			m_SharedMemory = new SharedMemory("SharedMemorySample", 128);
		}

		private void OnDisable() {
			m_SharedMemory.Dispose();
		}

		private void OnGUI() {
			EditorGUILayout.HelpBox("请打开多个工程的本窗口查看效果。", MessageType.Info);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("当前共享值", GUILayout.Width(EditorGUIUtility.labelWidth - 1F));
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.TextField(m_SharedMemory.GetString());
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			
			GUILayout.Space(10F);
			
			string newWords = EditorGUILayout.TextField("准备设置的值", m_Words);
			if (newWords != m_Words) {
				Undo.RecordObject(this, "Words");
				m_Words = newWords;
			}
			if (GUILayout.Button("设置", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2F))) {
				m_SharedMemory.SetString(m_Words);
			}
		}

		private void Update() {
			Repaint();
		}
	}
}
