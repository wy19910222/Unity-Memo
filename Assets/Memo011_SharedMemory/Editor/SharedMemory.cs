/*
 * @Author: wangyun
 * @CreateTime: 2024-08-04 22:00:45 615
 * @LastEditor: wangyun
 * @EditTime: 2024-08-04 22:00:45 619
 */

using System;
using System.IO.MemoryMappedFiles;
using System.Threading;
using UnityEngine;

namespace Memo11_SharedMemory.Editor {
	public class SharedMemory : IDisposable {
		public int Capacity { get; }
		public string MemoryMappedFileName { get; }
		public string MutexName { get; }
		
		private readonly MemoryMappedFile m_File;
		private readonly Mutex m_Mutex;

		public SharedMemory(string name, int capacity = 1024) {
			if (capacity <= 0) {
				throw new ArgumentException("无效容量！", nameof(capacity));
			}
			Capacity = capacity;
			MemoryMappedFileName = "MemoryMappedFile_" + name;
			MutexName = "Mutex_" + name;
			m_File = MemoryMappedFile.CreateOrOpen(MemoryMappedFileName, Capacity + 4L);
			// name不能与MemoryMappedFile的name相同，否则拿不到锁
			m_Mutex = new Mutex(false, MutexName);
		}

		public void SetString(string data) {
			SetData(data == null ? null : System.Text.Encoding.UTF8.GetBytes(data));
		}

		public void SetData(byte[] data) {
			int dataLength = data == null ? 0 : data.Length;
			if (dataLength > Capacity) {
				throw new ArgumentException("数据长度超出容量！", nameof(data));
			}
			if (m_Mutex.WaitOne()) {
				using (MemoryMappedViewAccessor writer = m_File.CreateViewAccessor(0, dataLength + 4L)) {
					byte[] bytes = BitConverter.GetBytes(dataLength);
					writer.WriteArray(0, bytes, 0, 4);
					if (dataLength > 0) {
						writer.WriteArray(4, data, 0, dataLength);
					}
				}
				m_Mutex.ReleaseMutex();
			} else {
				Debug.LogError("获取锁失败！");
			}
		}

		public string GetString() {
			byte[] data = GetData();
			return data == null ? null : System.Text.Encoding.UTF8.GetString(data);
		}

		public byte[] GetData() {
			if (m_Mutex.WaitOne()) {
				int dataLength;
				using (MemoryMappedViewAccessor reader = m_File.CreateViewAccessor(0, 4)) {
					byte[] bytes = new byte[4];
					reader.ReadArray(0, bytes, 0, 4);
					dataLength = BitConverter.ToInt32(bytes, 0);
				}
				byte[] data;
				if (dataLength > 0) {
					data = new byte[dataLength];
					using (MemoryMappedViewAccessor reader = m_File.CreateViewAccessor(4, dataLength)) {
						reader.ReadArray(0, data, 0, dataLength);
					}
				} else {
					data = Array.Empty<byte>();
				}
				m_Mutex.ReleaseMutex();
				return data;
			} else {
				Debug.LogError("获取锁失败！");
				return null;
			}
		}

		public void Dispose() {
			m_File.Dispose();
			m_Mutex.Dispose();
		}
	}
}
