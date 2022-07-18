using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tanac.Mvvm;

namespace Tanac.Core.MachineResources
{
    /// <summary>
    /// 设备状态管理器
    /// </summary>
    public class MachineStatusManager
    {
        private static MachineStatus _currentMachineStatus = MachineStatus.Unknown;
        private static MachineStatus _lastMachineStatus = MachineStatus.Unknown;
        public static MachineStatus CurrentMachineStatus
        {
            get
            {
                return _currentMachineStatus;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_currentMachineStatus != value)
                {
                    _lastMachineStatus = _currentMachineStatus;
                    _currentMachineStatus = value;                
                }
            }
        }
        public static MachineStatus LastMachineStatus => _lastMachineStatus;
    }
}
