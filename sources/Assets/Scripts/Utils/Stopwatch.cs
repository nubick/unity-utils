using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils
{
	public static class Stopwatch
	{
		private static readonly Dictionary<string, StopwatchBlock> Blocks = new Dictionary<string, StopwatchBlock>();
		
		[MenuItem("Window/Utils/Stopwatch: Log")]
		public static void Log()
		{
			Debug.Log("Stopwatch: Log");
			Blocks.Values.ForEach(_ => _.WriteLog());
		}

		private static StopwatchBlock Get(string blockId)
		{
			if (!Blocks.ContainsKey(blockId))
				Blocks[blockId] = new StopwatchBlock(blockId);
			return Blocks[blockId];
		}

		public static void Start(string blockId, LogType logType = LogType.Silent)
		{
			Get(blockId).Start(logType);
		}

		public static void Stop(string blockId)
		{
			Get(blockId).Stop();
		}

		public static void Clear(string blockId)
		{
			Blocks.Remove(blockId);
		}
		
		private class StopwatchBlock
		{
			private string Id { get; set; }
			private bool IsStarted { get; set; }
			private float StartTime { get; set; }
			private float LastTime { get; set; }
			private float SumTime { get; set; }
			private float MaxTime { get; set; }
			private int Times { get; set; }
			private LogType LogType { get; set; }
			
			public StopwatchBlock(string id)
			{
				Id = id;
			}

			public void Start(LogType logType)
			{
				if (IsStarted)
					Debug.LogError($"Trying to start block '{Id}' which is in started state.");

				LogType = logType;
				StartTime = Time.realtimeSinceStartup;
				IsStarted = true;
			}

			public void Stop()
			{
				if (!IsStarted)
					Debug.LogError($"Trying to stop block '{Id}' which is not in started state.");

				LastTime = (Time.realtimeSinceStartup - StartTime) * 1000f;
				MaxTime = Mathf.Max(MaxTime, LastTime);
				SumTime += LastTime;
				Times++;
				IsStarted = false;

				if (LogType == LogType.Always)
					Debug.Log($"Stopwatch: {Id}, {LastTime:0.00} ms");
			}

			public void WriteLog()
			{
				Debug.Log($"{Id}, last {LastTime:0.00} ms, max: {MaxTime:0.00} ms, average: {SumTime / Times:0.00} ms, times: {Times}");
			}
		}

		public enum LogType
		{
			Silent,
			Always
		}
	}
}