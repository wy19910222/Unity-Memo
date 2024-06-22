﻿/*
 * @Author: wangyun
 * @CreateTime: 2024-06-21 16:02:40 059
 * @LastEditor: wangyun
 * @EditTime: 2024-06-21 16:02:40 062
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AudioClipper : EditorWindow {
	[MenuItem("Memo/Memo6/音频剪辑器")]
	public static void ShowWindow() {
		GetWindow<AudioClipper>("音频剪辑器");
	}
	
	private const int WAVEFORM_HEIGHT = 128;
	private const float BACKGROUND_CELL_WIDTH = 28;	// 需要显示时间，不能太小
	private const float BACKGROUND_CELL_HEIGHT = 16;
	private const float RULER_LABEL_HEIGHT = 16;
	private const float RULER_LINE_HEIGHT = 8;
	
	private static readonly Color COLOR_TRANSPARENT = new Color(0, 0, 0, 0);
	private static readonly Color COLOR_RULER = Color.white;
	private static readonly Color COLOR_CHANNEL_BORDER = Color.white;
	private static readonly Color COLOR_BACKGROUND = Color.black;
	private static readonly Color COLOR_BACKGROUND_GRID = new Color(1, 0.5F, 0, 0.2F);
	private static readonly Color COLOR_WAVEFORM = new Color(1, 0.5F, 0);
	private static readonly Color COLOR_SELECTED = new Color(1, 1, 1, 0.3F);
	private static readonly Color COLOR_SELECTOR = new Color(0.5F, 1, 1);
	private static readonly Color COLOR_CURRENT = new Color(1, 0, 0);

	[SerializeField] private AudioClip m_Clip;
	[SerializeField] private float[] m_ClipData;
	[SerializeField] private float m_Duration;
	
	[SerializeField] private float m_StartTime;
	[SerializeField] private float m_EndTime;
	[SerializeField] private float m_VolumeScale = 1;
	
	[SerializeField] private AudioClip m_ClippedClip;
	
	[SerializeField] private Texture2D[] m_WaveformTextures = Array.Empty<Texture2D>();
	[SerializeField] private AudioSource m_AudioSource;
	private readonly Stack<Texture2D> m_TexturePool = new Stack<Texture2D>();
	
	private bool m_IsDragging;
	private int m_DraggingType;
	private Vector2 m_PrevMousePos;

	private GUIStyle m_RulerStyle; 
	
	private void OnEnable() {
		m_AudioSource = EditorUtility.CreateGameObjectWithHideFlags("[AudioClipper]", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
	}

	private void Update() {
		if (m_AudioSource.isPlaying) {
			Repaint();
		}
	}

	private void OnGUI() {
		if (m_RulerStyle == null) {
			m_RulerStyle = "CenteredLabel";
			m_RulerStyle.fontSize = 10;
		}

		AudioClip newClip = EditorGUILayout.ObjectField("声音源文件", m_Clip, typeof(AudioClip), false) as AudioClip;
		if (newClip != m_Clip) {
			// Undo.RecordObject(this, "Clip");
			m_Clip = newClip;
			m_ClippedClip = null;
			if (newClip != null) {
				m_ClipData = new float[newClip.samples * newClip.channels];
				m_Duration = newClip.length;
				m_StartTime = 0;
				m_EndTime = m_Duration;
				m_VolumeScale = 1;
				newClip.GetData(m_ClipData, 0);
			} else {
				m_ClipData = null;
				m_Duration = 0;
				m_StartTime = 0;
				m_EndTime = m_Duration;
				m_VolumeScale = 1;
			}
			UpdateWaveformTexture();
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField($"时长: {((m_Clip ? m_Duration.ToString("F2") + "s" : "-"))}");
		EditorGUILayout.LabelField($"声道: {(m_Clip ? m_Clip.channels : "-")}");
		EditorGUILayout.LabelField($"采样率: {(m_Clip ? m_Clip.frequency : "-")}");
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(10);

		EditorGUILayout.BeginHorizontal();
		EditorGUI.BeginChangeCheck();
		float startTime = m_StartTime, endTime = m_EndTime;
		startTime = Mathf.Clamp(EditorGUILayout.FloatField(startTime, GUILayout.Width(120F)), 0, m_Duration);
		EditorGUILayout.MinMaxSlider(ref startTime, ref endTime, 0, m_Duration);
		endTime = Mathf.Clamp(EditorGUILayout.FloatField(endTime, GUILayout.Width(120F)), 0, m_Duration);
		if (EditorGUI.EndChangeCheck()) {
			// Undo.RecordObject(this, "Time");
			m_StartTime = startTime;
			m_EndTime = endTime;
			m_ClippedClip = null;
		}
		EditorGUILayout.EndHorizontal();
		
		if (m_Clip) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(2);
			Rect canvasRect = EditorGUILayout.BeginVertical();
			int blocksMax = Mathf.FloorToInt(canvasRect.width / BACKGROUND_CELL_WIDTH);
			float[] gaps = { 0.01F, 0.02F, 0.05F, 0.1F, 0.2F, 0.5F };
			float blockDuration = 0;
			for (int i = 0, length = gaps.Length; i < length; i++) {
				float gap = gaps[i];
				if (m_Duration < gap * blocksMax) {
					blockDuration = gap;
					break;
				}
			}
			if (blockDuration == 0) {
				blockDuration = Mathf.Ceil(m_Duration / blocksMax);
			}
			int blocks = Mathf.FloorToInt(m_Duration / blockDuration);
			float blockWidth = blockDuration / m_Duration * canvasRect.width;
			Rect rulerRect = GUILayoutUtility.GetRect(canvasRect.width, RULER_LABEL_HEIGHT + RULER_LINE_HEIGHT);
			for (int i = 1; i <= blocks; i++) {
				Rect labelRect = new Rect(rulerRect.x + blockWidth * i - BACKGROUND_CELL_WIDTH * 0.5F, rulerRect.y, BACKGROUND_CELL_WIDTH, RULER_LABEL_HEIGHT);
				EditorGUI.LabelField(labelRect, $"{(blockDuration * i):F2}", m_RulerStyle);
				Rect shortLineRect = new Rect(rulerRect.x + blockWidth * (i - 0.5F), rulerRect.y + RULER_LABEL_HEIGHT + RULER_LINE_HEIGHT * 0.5F, 1, RULER_LINE_HEIGHT * 0.5F);
				EditorGUI.DrawRect(shortLineRect, COLOR_RULER);
				Rect longLineRect = new Rect(rulerRect.x + blockWidth * i, rulerRect.y + RULER_LABEL_HEIGHT, 1, RULER_LINE_HEIGHT);
				EditorGUI.DrawRect(longLineRect, COLOR_RULER);
			}
			
			int waveformCount = m_WaveformTextures.Length;
			Rect waveformRect = GUILayoutUtility.GetRect(canvasRect.width, WAVEFORM_HEIGHT * waveformCount);
			for (int i = 0; i < waveformCount; i++) {
				float yTop = waveformRect.y + WAVEFORM_HEIGHT * i;
				Rect rect = new Rect(waveformRect.x, yTop, waveformRect.width, WAVEFORM_HEIGHT);
				EditorGUI.DrawRect(rect, COLOR_BACKGROUND);
				float yCenter = yTop + WAVEFORM_HEIGHT * 0.5F;
				for (int j = 0; j * BACKGROUND_CELL_HEIGHT * 2 < WAVEFORM_HEIGHT; j++) {
					Rect lineRect = new Rect(waveformRect.x, yCenter + j * BACKGROUND_CELL_HEIGHT, waveformRect.width, 1);
					EditorGUI.DrawRect(lineRect, COLOR_BACKGROUND_GRID);
					lineRect.y = yCenter - j * BACKGROUND_CELL_HEIGHT;
					EditorGUI.DrawRect(lineRect, COLOR_BACKGROUND_GRID);
				}
				if (i > 0) {
					Rect lineRect = new Rect(waveformRect.x, yTop, waveformRect.width, 1);
					EditorGUI.DrawRect(lineRect, COLOR_CHANNEL_BORDER);
				}
			}
			for (int i = 1; i <= blocks; i++) {
				Rect lineRect = new Rect(blockWidth * i, waveformRect.y, 1, waveformRect.height);
				EditorGUI.DrawRect(lineRect, COLOR_BACKGROUND_GRID);
			}
			for (int i = 0; i < waveformCount; i++) {
				Rect rect = new Rect(waveformRect.x, waveformRect.y + WAVEFORM_HEIGHT * i, waveformRect.width, WAVEFORM_HEIGHT);
				GUI.DrawTexture(rect, m_WaveformTextures[i]);
			}

			float start = m_StartTime / m_Duration;
			float end = m_EndTime / m_Duration;
			Rect selectedRect = new Rect(waveformRect.x + waveformRect.width * start, waveformRect.y, waveformRect.width * (end - start), waveformRect.height);
			EditorGUI.DrawRect(selectedRect, COLOR_SELECTED);
			
			Rect startLineRect = new Rect(waveformRect.x + waveformRect.width * start, waveformRect.y, -1, waveformRect.height);
			EditorGUI.DrawRect(startLineRect, COLOR_SELECTOR);
			Rect endLineRect = new Rect(waveformRect.x + waveformRect.width * end, waveformRect.y, -1, waveformRect.height);
			EditorGUI.DrawRect(endLineRect, COLOR_SELECTOR);

			if (m_AudioSource.isPlaying) {
				float current = (m_StartTime + m_AudioSource.time) / m_Duration;
				Rect currentLineRect = new Rect(waveformRect.x + waveformRect.width * current, waveformRect.y, -1, waveformRect.height);
				EditorGUI.DrawRect(currentLineRect, COLOR_CURRENT);
			}

			switch (Event.current.type) {
				case EventType.MouseDown: {
					Vector2 mousePos = Event.current.mousePosition;
					if (mousePos.y >= waveformRect.yMin && waveformRect.y <= waveformRect.yMax) {
						float middle = (startLineRect.x + endLineRect.x) * 0.5F;
						float temp1 = Mathf.Min(startLineRect.x + 10, middle);
						float temp2 = Mathf.Max(endLineRect.x - 10, middle);
						if (mousePos.x > startLineRect.x - 10 && mousePos.x < temp1) {
							m_IsDragging = true;
							m_DraggingType = -1;
							m_PrevMousePos = mousePos;
						} else if (mousePos.x >= temp1 && mousePos.x <= temp2) {
							m_IsDragging = true;
							m_DraggingType = 0;
							m_PrevMousePos = mousePos;
						} else if (mousePos.x >= temp2 && mousePos.x < endLineRect.x + 10) {
							m_IsDragging = true;
							m_DraggingType = 1;
							m_PrevMousePos = mousePos;
						}
					}
					break;
				}
				case EventType.MouseDrag: {
					if (m_IsDragging) {
						Vector2 mousePos = Event.current.mousePosition;
						float deltaX = mousePos.x - m_PrevMousePos.x;
						m_PrevMousePos = mousePos;
						// Undo.RecordObject(this, "Time");
						float deltaTime = deltaX / waveformRect.width * m_Duration;
						switch (m_DraggingType) {
							case -1:
								m_StartTime = Mathf.Clamp(m_StartTime + deltaTime, 0, m_EndTime);
								break;
							case 0:
								deltaTime = Mathf.Clamp(deltaTime, -m_StartTime, m_Duration - m_EndTime);
								m_StartTime += deltaTime;
								m_EndTime += deltaTime;
								break;
							case 1:
								m_EndTime = Mathf.Clamp(m_EndTime + deltaTime, m_StartTime, m_Duration);
								break;
						}
						m_ClippedClip = null;
						Repaint();
					}
					break;
				}
				case EventType.MouseUp:
				case EventType.Ignore:
					m_IsDragging = false;
					break;
			}
			EditorGUILayout.EndVertical();
			GUILayout.Space(2);
			EditorGUILayout.EndHorizontal();
		} else {
			EditorGUI.DrawRect(GUILayoutUtility.GetRect(position.width, WAVEFORM_HEIGHT), COLOR_BACKGROUND);
		}

		GUILayout.Space(10);
			
		EditorGUI.BeginChangeCheck();
		float newVolume = EditorGUILayout.Slider("音量缩放", m_VolumeScale, 0, 2);
		if (EditorGUI.EndChangeCheck()) {
			// Undo.RecordObject(this, "VolumeScale");
			m_VolumeScale = newVolume;
			m_ClippedClip = null;
			UpdateWaveformTexture();
		}
		GUILayout.Space(10);
		if (m_AudioSource.isPlaying) {
			if (GUILayout.Button("结束试听", GUILayout.Height(40))) {
				StopPlayClippedAudio();
			}
		} else {
			if (GUILayout.Button("试听片段", GUILayout.Height(40))) {
				PlayClippedAudio();
			}
		}
		GUILayout.Space(10);
		if (GUILayout.Button("保存片段", GUILayout.Height(40))) {
			WriteClippedAudio();
		}
	}

	private void PlayClippedAudio() {
		if (m_Clip != null) {
			if (m_ClippedClip == null) {
				m_ClippedClip = ClipAudio(m_Clip, m_StartTime, m_EndTime, m_VolumeScale);
			}
			m_AudioSource.clip = m_ClippedClip;
			m_AudioSource.Play();
		} else {
			Debug.LogError("Clip is none!");
		}
	}

	private void StopPlayClippedAudio() {
		m_AudioSource.Stop();
	}

	private void WriteClippedAudio() {
		if (m_Clip != null) {
			if (m_ClippedClip == null) {
				m_ClippedClip = ClipAudio(m_Clip, m_StartTime, m_EndTime, m_VolumeScale);
			}
			string srcFilePath = AssetDatabase.GetAssetPath(m_Clip);
			string directory = File.Exists(srcFilePath) ? srcFilePath[..srcFilePath.LastIndexOfAny(new[] {'/', '\\'})] : "Assets";
			string filePath = EditorUtility.SaveFilePanel("保存剪辑的音频", directory, m_Clip.name + "_New", "wav");
			if (!string.IsNullOrEmpty(filePath)) {
				AudioClipWriter.WriteToFile(filePath, m_ClippedClip, 16);
				AssetDatabase.Refresh();
			}
		} else {
			Debug.LogError("Clip is none!");
		}
	}

	private void UpdateWaveformTexture() {
		if (m_ClipData != null) {
			foreach (Texture2D waveformTexture in m_WaveformTextures) {
				m_TexturePool.Push(waveformTexture);
			}
			m_WaveformTextures = GenerateWaveTextures(m_ClipData, m_Clip.channels, m_Clip.samples, WAVEFORM_HEIGHT);
		}
	}

	private Texture2D[] GenerateWaveTextures(IReadOnlyList<float> data, int channels, int samples, int height, int width = 2048) {
		Texture2D[] waveformTextures = new Texture2D[channels];
		Color[] colors = new Color[width * height];
		for (int channelIndex = 0; channelIndex < channels; channelIndex++) {
			Texture2D waveformTexture = m_TexturePool.Count > 0 ? m_TexturePool.Pop() : new Texture2D(width, height, TextureFormat.RGBA32, false);
			float samplePerPixel = (float) samples / width;
			for (int x = 0; x < width; x++) {
				float sampleValueMax = -float.MaxValue;
				float sampleValueMin = float.MaxValue;
				int sampleStart = Mathf.FloorToInt(x * samplePerPixel);
				int sampleEnd = Mathf.FloorToInt((x + 1) * samplePerPixel);
				for (int sampleIndex = sampleStart; sampleIndex <= sampleEnd && sampleIndex < samples; sampleIndex++) {
					float sampleValue = data[sampleIndex * channels + channelIndex] * m_VolumeScale;
					if (sampleValue > sampleValueMax) sampleValueMax = sampleValue;
					if (sampleValue < sampleValueMin) sampleValueMin = sampleValue;
				}
				int heightValueMax = Mathf.Clamp((int) ((sampleValueMax * 0.5F + 0.5F) * height), 0, height - 1);
				int heightValueMin = Mathf.Clamp((int) ((sampleValueMin * 0.5F + 0.5F) * height), 0, height - 1);
				for (int y = 0; y < heightValueMin; y++) {
					int colorIndex = y * width + x;
					colors[colorIndex] = COLOR_TRANSPARENT;
				}
				for (int y = heightValueMin; y <= heightValueMax; y++) {
					int colorIndex = y * width + x;
					colors[colorIndex] = COLOR_WAVEFORM;
				}
				for (int y = heightValueMax + 1; y < height; y++) {
					int colorIndex = y * width + x;
					colors[colorIndex] = COLOR_TRANSPARENT;
				}
			}
			waveformTexture.SetPixels(colors);
			waveformTexture.Apply();
			waveformTextures[channelIndex] = waveformTexture;
		}
		return waveformTextures;
	}

	private static AudioClip ClipAudio(AudioClip clip, float start, float end, float volumeScale) {
		if (!clip) {
			throw new NullReferenceException("Clip is none.");
		}
		if (start > end) {
			throw new ArgumentException("Argument 'end' is less than 'start'.");
		}
		if (start < 0) {
			throw new OverflowException("Argument 'start' is less than 0.");
		}
		if (end > clip.length) {
			throw new OverflowException("Argument 'end' is greater than original length.");
		}
		volumeScale = Mathf.Max(volumeScale, 0);
		
		int channels = clip.channels;
		int frequency = clip.frequency;
		int startSample = (int) (start * frequency);
		int endSample = (int) (end * frequency);
		int lengthSamples = endSample - startSample;
		
		int newDataLength = lengthSamples * channels;
		float[] newData = new float[newDataLength];
		clip.GetData(newData, startSample * channels);
		for (int i = 0; i < newDataLength; i++) {
			newData[i] *= volumeScale;
		}

		AudioClip clippedAudioClip = AudioClip.Create("ClippedAudio", lengthSamples, channels, frequency, false);
		clippedAudioClip.SetData(newData, 0);

		return clippedAudioClip;
	}
}