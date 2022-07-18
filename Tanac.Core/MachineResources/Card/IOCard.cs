using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
    [Serializable]
    public abstract class IOCard
    {
        /// <summary>
        /// 卡的编号
        /// </summary>
        public int Encode { get; set; }
        /// <summary>
        /// 卡的类型
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 卡的名称
        /// </summary>
        public string CardName { get; set; }
        /// <summary>
        /// 同一品牌下的卡号
        /// </summary>
        public int CardID { get; set; }
        /// <summary>
        /// 卡的备注
        /// </summary>
        public string CardRemarks { get; set; }

        /// <summary>
        /// 输入点总数
        /// </summary>
        public int InputCount { get; set; }
        /// <summary>
        /// 输出点总数
        /// </summary>
        public int OutputCount { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public abstract bool CardInit();
        public abstract bool GetDI(int port, int group = 0);
        public abstract bool GetDO(int port, int group = 0);
        public abstract bool SetDO(int port, int ONOrOFF, int group = 0);

    }
}
