using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Tanac.Core.TaskExt
{
	/// <summary>
	/// 线程任务管理器
	/// </summary>
    public class TaskManager
    {
        private static List<Thread> tasks = new List<Thread>();
        private static bool isStoping = false;
        private Timer timer;
        public static int Count => tasks.Count;
		public static Thread Run(Action action, string taskName = "")
		{
			Thread thread = new Thread(action.Invoke);
			thread.IsBackground = true;
			if (taskName == "")
			{
				thread.Name = $"TaskManager：{action.Method.Name}-{thread.ManagedThreadId}";
			}
			else
			{
				thread.Name = taskName;
			}
			thread.Start();
			while (!thread.IsAlive)
			{
				Thread.Sleep(0);
			}
			tasks.Add(thread);
			return thread;
		}
		public static bool Wait(Thread thread, int timeout)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			if (thread?.IsAlive ?? false)
			{
				while (thread?.IsAlive ?? false)
				{
					if (stopwatch.ElapsedMilliseconds > timeout)
					{
						return false;
					}
					Thread.Sleep(1);
				}
			}
			return true;
		}
		public static void Wait(Thread thread)
		{
			if (thread?.IsAlive ?? false)
			{
				while (thread?.IsAlive ?? false)
				{
					Thread.Sleep(1);
				}
			}
		}
		public static void Clear()
		{
			List<Thread> list = tasks.FindAll((Thread t) => !t.IsAlive);
			for (int i = 0; i < list.Count; i++)
			{
				if (isStoping)
				{
					break;
				}
				tasks.Remove(list[i]);
				list[i] = null;
			}
		}
		public static string Status()
		{
			string text = "";
			if (tasks.Count == 0)
			{
				return "";
			}
			Clear();
			Thread[] array = new Thread[tasks.Count];
			tasks.CopyTo(array);
			Thread[] array2 = array;
			foreach (Thread thread in array2)
			{
				text = string.Concat(text, thread.Name, ":", thread.ThreadState, Environment.NewLine);
			}
			return text;
		}
		public static void StopAll()
		{
			try
			{
				Clear();
				isStoping = true;
				int i = 0;
				for (int count = tasks.Count; i < count; i++)
				{
					if (tasks.Count <= i)
					{
						continue;
					}
					Thread thread = tasks[i];
					if (thread?.IsAlive ?? false)
					{
						thread.Abort();
						while (thread.ThreadState != System.Threading.ThreadState.Aborted)
						{
							Thread.Sleep(1);
						}
					}
					thread = null;
				}
				tasks.Clear();
				isStoping = false;
			}
			catch (Exception)
			{
			}
		}
		public static void Stop(Thread thread)
		{
			try
			{
				if (thread?.IsAlive ?? false)
				{
					tasks.Remove(thread);
					thread.Abort();
					while (thread.ThreadState != System.Threading.ThreadState.Aborted)
					{
						Thread.Sleep(1);
					}
				}
				thread = null;
			}
			catch
			{
			}
		}
	}
}
