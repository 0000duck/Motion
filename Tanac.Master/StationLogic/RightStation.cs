using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanac.Core;
using Tanac.Core.MachineResources;
using Tanac.Core.StationLogic;
using Tanac.Master.Services;

namespace Tanac.Master.StationLogic
{
    public class RightStation : StationLogicAbstract
    {

        public static RightStation Instance = new RightStation();
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

        public Cylinder cy_气缸1 = CylinderManager.Get("气缸1");

        public Axis ax_轴1 = AxisManager.Get("轴1");

        public XPoint 点位1 = PointPosManager.Get("点位1") as XPoint;

        public XPoint 点位2 = PointPosManager.Get("点位2") as XPoint;

        /// <summary>
        /// 生产流程
        /// </summary>
        public override void ActionProcess()
        {
            switch (Step)
            {
                case StationStep.None:
                    ax_轴1.AbsMove(1000, 100, 100,点位1.XPos);
                    Step = StationStep.Press;
                    break;
                case StationStep.Press:
                    if (ax_轴1.WaitMoveDone(10000))
                    {
                        CoreFunction.Delay(1000);
                        Step = StationStep.Press2;
                    }
                    else
                    {
                        Step = StationStep.None;
                    }
                    break;
                case StationStep.Press2:
                    ax_轴1.AbsMove(1000, 100, 100,点位2.XPos);
                    Step =StationStep.Location;
                    break;
                case StationStep.Location:
                    //if(cy_气缸1.HomeAndWaitDone(3000))
                    //{
                    //    Step=StationStep.None;
                    //}
                    if(ax_轴1.WaitMoveDone(20000))
                    {
                        CoreFunction.Delay(2000);
                        Step = StationStep.None;
                    }
                    else
                    {
                        Step = StationStep.Press2;
                    }
                    break;
                default:
                    break;
            }
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
