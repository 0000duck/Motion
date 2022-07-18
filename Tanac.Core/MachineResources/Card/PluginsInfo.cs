using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
    /// <summary>
    /// 卡插件信息
    /// </summary>
    public class PluginsInfo
    {
        /// <summary>
        /// 卡对象类
        /// </summary>
        public Type CardObjType { get; set; }
        /// <summary>
        /// 卡插件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类别号
        /// </summary>
        public int SortNO;
        /// <summary>
        /// 子类
        /// </summary>
        public string Category { get; set; }
    }
}
