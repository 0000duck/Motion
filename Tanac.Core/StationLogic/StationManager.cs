using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.StationLogic
{

    /// <summary>
    /// 工位管理器
    /// </summary>
    public class StationManager
    {
        public static RunningMode MachineProductionMode = RunningMode.ProductionMode;

        public static List<StationLogicAbstract> StationList = new List<StationLogicAbstract>();

    }
}
