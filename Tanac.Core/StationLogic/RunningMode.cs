using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.StationLogic
{
	public enum RunningMode
	{
		[Description("生产模式")]
		ProductionMode,
		[Description("工程师模式")]
		EngineeringMode,
		[Description("GRR模式")]
		GRRMode,
		[Description("空跑模式")]
		EmptyMode,
		[Description("未知模式")]
		NullMode
	}
}
