using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
	/// <summary>
	/// 设备状态
	/// </summary>
	public enum MachineStatus
	{
		[Description("未知状态")]
		Unknown,
		[Description("初始化完成")]
		Initialized,
		[Description("运行状态")]
		Running,
		[Description("暂停状态")]
		Pause,
		[Description("复位中")]
		Homing,
		[Description("复位完成")]
		Ready,
		[Description("急停状态")]
		Emerg,
		[Description("报警状态")]
		Alarm,
		[Description("停止中")]
		Stopping,
		[Description("退出状态")]
		Quit
	}
}
