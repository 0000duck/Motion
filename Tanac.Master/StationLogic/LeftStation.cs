using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanac.Core.StationLogic;
using Tanac.Master.Services;

namespace Tanac.Master.StationLogic
{
    public class LeftStation : StationLogicAbstract
    {
        public static LeftStation Instance = new LeftStation();
        //记录当前步骤，用于步骤变化的比较
        private StationStep _curStep = StationStep.None;
        public StationStep Step
        {
            get
            {
                return _curStep;
            }
            set
            {
                if (_curStep != value)
                {
                    _curStep = value;
                }
            }
        }

        /// <summary>
        /// 工位步骤定义
        /// </summary>
        public enum StationStep
        {
            [Description("等待机台初始化")]
            None,
            [Description("预压流程")]
            Press,
            [Description("预压流程升起")]
            Press2,
            [Description("定位流程")]
            Location,
            [Description("拍照流程")]
            Capture,
            [Description("准备回位流程")]
            Setout,
            [Description("结束流程")]
            Stop,
            [Description("机械臂在加一个结束流程")]
            StopRobot,
            [Description("异常流程流程")]
            Error,
        }

        /// <summary>
        /// 工位初始化
        /// </summary>
        public override void Init()
        {
            Step = StationStep.None;
            base.Init();
        }
        /// <summary>
        /// 生产流程
        /// </summary>
        public override void ActionProcess()
        {

        

        }
        /// <summary>
        /// 空跑流程
        /// </summary>
        public override void EmptyActionProcess()
        {

        }
        /// <summary>
        /// GRR流程
        /// </summary>
        public override void GRRActionProcess()
        {

        }
    }
}
