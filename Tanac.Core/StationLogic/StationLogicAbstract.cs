/********************************************************************
	created:	2022/06/10
	filename: 	StationLogic
	file ext:	cs
	author:		zy
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tanac.Core.ExceptionExt;
using Tanac.Core.MachineResources;
using Tanac.Core.TaskExt;
using Tanac.Mvvm;

namespace Tanac.Core.StationLogic
{
    public abstract class StationLogicAbstract
    {
		private Thread processThread;

		public double stationCT = 0.0;

		public Stopwatch swStationCT = new Stopwatch();

		public Stopwatch swStepTime = new Stopwatch();

		public Stopwatch swActionTimeout = new Stopwatch();
		public bool IsInAction { get; private set; } = false;
		public bool IsActionStoped { get; private set; } = false;
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		protected Thread ProcessThread => processThread;
		public StationLogicAbstract()
		{
			if (Name == string.Empty)
			{
				Name = GetType().Name;
			}
			if (Description == string.Empty)
			{
				Description = GetType().FullName;
			}
		}
		protected virtual DialogResult AlarmPause(string title, string excMsg, string exSolution, string okOperationTip, string cancelOperationTip)
		{
			MachineStatusManager.CurrentMachineStatus = MachineStatus.Alarm;
			return MessageBox.Show(excMsg + Environment.NewLine + "【原因(解决方案)】" + exSolution + Environment.NewLine + "【确定】" + okOperationTip + Environment.NewLine + "【取消】" + cancelOperationTip, title, MessageBoxButtons.OKCancel);
		}
		protected virtual DialogResult AlarmPause(string title, string excMsg, string exSolution, string okOperationTip)
		{
			MachineStatusManager.CurrentMachineStatus = MachineStatus.Alarm;
			return MessageBox.Show(excMsg + Environment.NewLine + "【原因(解决方案)】" + exSolution + Environment.NewLine + "【确定】" + okOperationTip + Environment.NewLine, title, MessageBoxButtons.OK);
		}
		protected virtual DialogResult AlarmPause(string title, string excMsg, string exSolution, string yesOperationTip, string noOperationTip, string cancelOperationTip)
		{
			MachineStatusManager.CurrentMachineStatus = MachineStatus.Alarm;
			return MessageBox.Show(excMsg + Environment.NewLine + "【原因(解决方案)】" + exSolution + Environment.NewLine + "【是】" + yesOperationTip + Environment.NewLine + "【否】" + noOperationTip + Environment.NewLine + "【取消】" + cancelOperationTip, title, MessageBoxButtons.YesNoCancel);
		}
		/// <summary>
		/// 工站初始化
		/// </summary>
		public virtual void Init()
		{
			if (processThread == null || processThread.ThreadState == System.Threading.ThreadState.Stopped || processThread.ThreadState == System.Threading.ThreadState.Aborted)
			{
				string name = GetType().Name;
				processThread = TaskManager.Run(delegate
				{
					Run();
				}, "StationThread--" + name);
				IsActionStoped = false;
				IsInAction = false;
				StationManager.StationList.Add(this);
			}
		}
		/// <summary>
		/// 工站停止
		/// </summary>
		public virtual void Stop()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			while (!IsActionStoped && stopwatch.ElapsedMilliseconds <= 100)
			{
				Thread.Sleep(5);
			}
			if (processThread != null && processThread.IsAlive)
			{
				processThread.Abort();
			}
			TaskManager.Wait(processThread);
		}
		protected virtual void Run()
		{
			while (MachineStatusManager.CurrentMachineStatus != MachineStatus.Quit && MachineStatusManager.CurrentMachineStatus != MachineStatus.Stopping)
			{
				while (MachineStatusManager.CurrentMachineStatus == MachineStatus.Running)
				{
					try
					{
						if (StationManager.MachineProductionMode == RunningMode.ProductionMode)
						{
							IsInAction = true;
							ActionProcess();
							IsInAction = false;
						}
						else if (StationManager.MachineProductionMode == RunningMode.EmptyMode)
						{
							IsInAction = true;
							EmptyActionProcess();
							IsInAction = false;
						}
						else if (StationManager.MachineProductionMode == RunningMode.EngineeringMode)
						{
							IsInAction = true;
							IsInAction = false;
						}
						else if (StationManager.MachineProductionMode == RunningMode.GRRMode)
						{
							IsInAction = true;
							GRRActionProcess();
							IsInAction = false;
						}
						else
						{
							AlarmPause("提示", "运行模式错误！", "请去登录页面选择对应的运行模式！", "");
						}
					}
					catch (Exception ex)
					{
						if (ex is ThreadAbortException)
						{
							continue;
						}
						if (ex is IOException)
						{
							AlarmPause("IO读写异常", ex.Message, "", "");
							continue;
						}
						if (ex is AxisException)
						{
							AlarmPause("轴异常", ex.Message, "", "");
							continue;
						}
						AlarmPause("未知异常", ex.StackTrace, "请联系软件工程师", "");
					}
					Thread.Sleep(5);
				}
				Thread.Sleep(5);
			}
			IsActionStoped = true;
		}
		/// <summary>
		/// GRR流程
		/// </summary>
		public abstract void GRRActionProcess();
		/// <summary>
		/// 空跑流程
		/// </summary>
		public abstract void EmptyActionProcess();
		/// <summary>
		/// 生产流程
		/// </summary>
		public abstract void ActionProcess();
	}
}
