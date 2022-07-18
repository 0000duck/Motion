using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tanac.Core;
using Tanac.Core.MachineResources;
using Tanac.Core.StationLogic;
using Tanac.Core.TaskExt;
using Tanac.Log4Net;
using Tanac.Master.StationLogic;
using Tanac.Master.ViewModel;
using static Tanac.Core.CoreFunction;

namespace Tanac.Master.Services
{
    public class Machine
    {
        #region 定义设备的轴卡、轴、气缸、IO等,其实脚本模式下并不需要定义

        #region IO输出定义
        public static OutputSetting do_红灯;
        public static OutputSetting do_黄灯;
        public static OutputSetting do_绿灯;
        public static OutputSetting do_蜂鸣器;
        #endregion

        #region IO输入定义
        public static InputSetting di_启动;
        public static InputSetting di_停止;
        public static InputSetting di_复位;
        public static InputSetting di_急停;
        public static InputSetting di_安全门;
        public static InputSetting di_光栅信号;
        #endregion

        #region 轴定义

        public static Axis ax_轴1 = AxisManager.Get("轴1");
        public static Axis ax_轴2 = AxisManager.Get("轴2");
        public static Axis ax_轴3 = AxisManager.Get("轴3");
        public static Axis ax_轴4 = AxisManager.Get("轴4");

        #endregion

        #region 气缸定义
        public static Cylinder cy_气缸1;
        public static Cylinder cy_气缸2;
        public static Cylinder cy_气缸3;
        public static Cylinder cy_气缸4;
        #endregion
        #endregion


        public static IOCard GTS卡;

        public static MotionCard GTS轴卡;

        public static Axis 轴1;

        /// <summary>
        /// 设备初始化
        /// </summary>
        public static void MachineInit()
        {

            if(IOCardManager.IOCardList.Count == 0)
            {
                IOCardManager.CreateIOCard("GTS",0);
            }

            if (MotionCardManager.MotionCardList.Count == 0)
            {
                MotionCardManager.CreateMotionCard("GTS",0);
            }
            GTS轴卡= MotionCardManager.Get("MotionCard0");
            GTS卡 = IOCardManager.Get("IOCard0");

            if(AxisManager.AxisList.Count==0)
            {
                ax_轴1 = new Axis(1,"轴1",GTS轴卡);
                ax_轴1.Rate = 1000;
                ax_轴2 = new Axis(2, "轴2", GTS轴卡);
                ax_轴2.Rate = 1000;
                ax_轴3 = new Axis(3, "轴3", GTS轴卡);
                ax_轴3.Rate = 1000;
                ax_轴4 = new Axis(4, "轴4", GTS轴卡);
                ax_轴4.Rate = 1000;
                AxisManager.Add(ax_轴1);
                AxisManager.Add(ax_轴2);
                AxisManager.Add(ax_轴3);
                AxisManager.Add(ax_轴4);
            }
            else
            {
                ax_轴1 = AxisManager.Get("轴1");
                ax_轴2 = AxisManager.Get("轴2");
                ax_轴3 = AxisManager.Get("轴3");
                ax_轴4 = AxisManager.Get("轴4");

            }


            if(InputManager.InputSettingList.Count<6)
            {
                InputManager.Clear();
                di_启动 = new InputSetting("启动",GTS卡,1);
                di_停止 = new InputSetting("停止", GTS卡,2);
                di_复位 = new InputSetting("复位", GTS卡, 3);
                di_急停 = new InputSetting("急停", GTS卡, 4);
                di_安全门 = new InputSetting("安全门", GTS卡, 5);
                di_光栅信号 = new InputSetting("光栅信号", GTS卡, 6);

                InputManager.Add(di_启动);
                InputManager.Add(di_停止);
                InputManager.Add(di_复位);
                InputManager.Add(di_急停);
                InputManager.Add(di_安全门);
                InputManager.Add(di_光栅信号);
            }
            else
            {
                di_启动 = InputManager.Get("启动");
                di_停止 = InputManager.Get("停止");
                di_复位 = InputManager.Get("复位");
                di_急停 = InputManager.Get("急停");
                di_安全门 = InputManager.Get("安全门");
                di_光栅信号 = InputManager.Get("光栅信号");
            }
            if (OutputManager.OutputSettingList.Count < 4)
            {
                OutputManager.Clear();
                do_红灯 = new OutputSetting("红灯",GTS卡,1);

                do_黄灯 = new OutputSetting("黄灯", GTS卡, 2);

                do_绿灯 = new OutputSetting("绿灯", GTS卡, 3);

                do_蜂鸣器 = new OutputSetting("蜂鸣器", GTS卡, 4);

                OutputManager.Add(do_红灯);
                OutputManager.Add(do_黄灯);
                OutputManager.Add(do_绿灯);
                OutputManager.Add(do_蜂鸣器);
            }
            else
            {
                do_红灯 = OutputManager.Get("红灯");
                do_黄灯 = OutputManager.Get("黄灯");
                do_绿灯 = OutputManager.Get("绿灯");
                do_蜂鸣器 = OutputManager.Get("蜂鸣器");
            }
            if (CylinderManager.CylinderList.Count == 0)
            {
                cy_气缸1 = new DoubleDriveCylinder("气缸1", do_红灯, do_红灯, di_启动, di_启动);
                CylinderManager.Add(cy_气缸1);
            }
            else
            {
                cy_气缸1 = CylinderManager.Get("气缸1");
            }

            if(PointPosManager.PointPosList.Count==0)
            {

                var point_1 = new XPoint("点位1",ax_轴1);
                var point_2 = new XPoint("点位2", ax_轴1);
                PointPosManager.PointPosList.Add(point_1);
                PointPosManager.PointPosList.Add(point_2);
            }
            var res1 = GTS轴卡.CardInit();
            var res2=GTS卡.CardInit();
            if(res1&&res2)
            {
                MachineStatusManager.CurrentMachineStatus = MachineStatus.Initialized;
            }
            MachineResourceInit();//设备资源初始化
            Task.Factory.StartNew(GoHomeTask);//开启回零线程
            Task.Factory.StartNew(ScanButtonTask);//开启按钮扫描线程
            Task.Factory.StartNew(TriColorLampTask);//开启三色灯线程           
        }
        /// <summary>
        /// 设备关闭
        /// </summary>
        public static void MachineClose()
        {

        }
        /// <summary>
        /// 设备资源初始化
        /// </summary>
        public static void MachineResourceInit()
        {
            InputManager.BindCard();
            OutputManager.BindCard();
            CylinderManager.BindIoSetting();
            AxisManager.BindCard();
            PointPosManager.BindAxis();
              
        }
        /// <summary>
        /// 设备加载系统配置
        /// </summary>
        public static void MachineLoadConfig()
        {
            IOCardManager.LoadConfig();//加载IO卡配置
            MotionCardManager.LoadConfig();//加载轴卡配置
            InputManager.LoadConfig();//加载输入IO配置
            OutputManager.LoadConfig();//加载输出IO配置
            AxisManager.LoadConfig();//加载轴配置
            PointPosManager.LoadConfig();//加载点位配置
            CylinderManager.LoadConfig();//加载气缸配置
        }

        /// <summary>
        /// 设备保存系统配置
        /// </summary>
        public static void MachineSaveConfig()
        {
            IOCardManager.SaveConfig();//保存IO卡配置
            MotionCardManager.SaveConfig();//保存轴卡配置
            InputManager.SaveConfig();//保存输入IO配置
            OutputManager.SaveConfig();//保存输出IO配置
            AxisManager.SaveConfig();//保存轴配置
            PointPosManager.SaveConfig();//保存点位配置
            CylinderManager.SaveConfig();//保存气缸配置
        }
        /// <summary>
        /// 复位
        /// </summary>
        public static void Intial()
        {
            if (MachineStatusManager.CurrentMachineStatus == MachineStatus.Initialized)//整机复位
            {
                Log.Info("点击复位...");
                MachineStatusManager.CurrentMachineStatus = MachineStatus.Homing;
                return;
            }
            else if (MachineStatusManager.CurrentMachineStatus == MachineStatus.Alarm)
            {
                Log.Info("点击复位...");
                if (MachineStatusManager.LastMachineStatus == MachineStatus.Running)//如果报警前处于运行状态，报警复位后转到暂停状态
                {
                    MachineStatusManager.CurrentMachineStatus = MachineStatus.Pause;
                }
                else//否则恢复上次的状态
                {
                    MachineStatusManager.CurrentMachineStatus = MachineStatusManager.LastMachineStatus;
                }
                return;
            }
            else if (MachineStatusManager.CurrentMachineStatus == MachineStatus.Pause)
            {
                Log.Info("点击复位...");
                if (MachineStatusManager.LastMachineStatus == MachineStatus.Homing)//如果报警前处于运行状态，报警复位后转到暂停状态
                {
                    MachineStatusManager.CurrentMachineStatus = MachineStatusManager.LastMachineStatus;
                }
                return;
            }
        }
        /// <summary>
        /// 启动
        /// </summary>
        public static void Start()
        {
            if ((MachineStatusManager.CurrentMachineStatus == MachineStatus.Ready
                 || MachineStatusManager.CurrentMachineStatus == MachineStatus.Pause)
                 && MachineStatusManager.CurrentMachineStatus != MachineStatus.Running)
            {               
                MachineStatusManager.CurrentMachineStatus = MachineStatus.Running;
                Log.Info("开始运行...");
            }
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public static void Pause()
        {
            if (MachineStatusManager.CurrentMachineStatus == MachineStatus.Running
                || MachineStatusManager.CurrentMachineStatus == MachineStatus.Homing)
            {
                Log.Info("点击暂停...");
                MachineStatusManager.CurrentMachineStatus = MachineStatus.Pause;

                Log.Info("暂停中...");
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        public static void Stop()
        {
            if (MachineStatusManager.CurrentMachineStatus == MachineStatus.Running
                || MachineStatusManager.CurrentMachineStatus == MachineStatus.Pause
                || MachineStatusManager.CurrentMachineStatus == MachineStatus.Emerg
                || MachineStatusManager.CurrentMachineStatus == MachineStatus.Alarm
                || MachineStatusManager.CurrentMachineStatus == MachineStatus.Ready
                )
            {
                Log.Info("点击停止...");
                MachineStatusManager.CurrentMachineStatus = MachineStatus.Stopping;
                Log.Info("停止中...");
                StationManager.StationList.ForEach(s =>
                {
                    s?.Stop();
                });
                StationManager.StationList.Clear();
                TaskManager.StopAll();
                AxisManager.AllStopEmg();
                Log.Info("已停止！");
                MachineStatusManager.CurrentMachineStatus = MachineStatus.Initialized;
            }
        }
        /// <summary>
        /// 安全门触发停止
        /// </summary>
        public static void PauseSafetyDoor()
        {
            if (MachineStatusManager.CurrentMachineStatus == MachineStatus.Running
                || MachineStatusManager.CurrentMachineStatus == MachineStatus.Homing
                || MachineStatusManager.CurrentMachineStatus == MachineStatus.Ready
                || MachineStatusManager.CurrentMachineStatus == MachineStatus.Initialized)
            {
                Log.Error("安全门打开...");
                MachineStatusManager.CurrentMachineStatus = MachineStatus.Alarm;
            }
        }
        /// <summary>
        /// 设备回零任务
        /// </summary>
        public static void GoHomeTask()
        {
            double pos=0;
            while (MachineStatusManager.CurrentMachineStatus != MachineStatus.Quit)
            {
                MainViewModel.Instance.CurrentState = MachineStatusManager.CurrentMachineStatus.ToString();//测试用
                if (((MachineStatusManager.CurrentMachineStatus == MachineStatus.Homing)))
                {
                    try
                    {
                        Log.Info("设备整机开始复位！");
                        Thread.Sleep(2000);
                        LeftStation.Instance.Init();//左工位初始化
                        RightStation.Instance.Init();//右工位初始化
                      
                        MachineStatusManager.CurrentMachineStatus = MachineStatus.Ready;
                        Log.Info("设备整机复位完成！");
                    }
                    catch (Exception ex)
                    {
                        MachineStatusManager.CurrentMachineStatus = MachineStatus.Alarm;
                        Log.Error("设备回零异常！");
                    }
                }
                Thread.Sleep(10);
            }
        }
        /// <summary>
        /// 按钮扫描任务
        /// </summary>
        public static void ScanButtonTask()
        {
            while(MachineStatusManager.CurrentMachineStatus!=MachineStatus.Quit)
            {
                try
                {
                    bool status_停止 = Machine.di_停止.GetStatus();
                    bool status_复位 = Machine.di_复位.GetStatus();
                    bool status_启动 = Machine.di_启动.GetStatus();
                    bool status_安全门磁 = Machine.di_安全门.GetStatus();
                    bool status_光幕 = Machine.di_光栅信号.GetStatus();
                }
                catch(Exception ex)
                {
                    if (AlarmPause("IO扫描异常", "按钮IO扫描异常", "点击确定，再试一次；点击取消，重启软件，检查板卡是否掉线", "再试一次", "重启软件，检查板卡") == System.Windows.Forms.DialogResult.Cancel)
                    {
                        Stop();//设备停止
                    }
                }
                Thread.Sleep(10);
            }
        }
        /// <summary>
        /// 三色灯任务
        /// </summary>
        public static void TriColorLampTask()
        {
            while(true)
            {
                try
                {
                    switch (MachineStatusManager.CurrentMachineStatus)
                    {
                        case MachineStatus.Unknown://未知状态,红灯闪烁
                            {
                                Machine.do_红灯.Toggle();
                                Machine.do_黄灯.OFF();
                                Machine.do_绿灯.OFF();
                                Machine.do_蜂鸣器.OFF();
                            }
                            break;
                        case MachineStatus.Initialized:
                            {
                                Machine.do_红灯.OFF();
                                Machine.do_黄灯.Toggle();
                                Machine.do_绿灯.OFF();
                                Machine.do_蜂鸣器.OFF();
                            }
                            break;
                        case MachineStatus.Homing://回零中状态,黄灯闪烁
                            {
                                Machine.do_红灯.OFF();
                                Machine.do_黄灯.Toggle();
                                Machine.do_绿灯.OFF();
                                Machine.do_蜂鸣器.OFF();
                            }
                            break;
                        case MachineStatus.Ready://就绪状态,绿灯闪烁
                            {
                                Machine.do_红灯.OFF();
                                Machine.do_黄灯.OFF();
                                Machine.do_绿灯.Toggle();
                                Machine.do_蜂鸣器.OFF();
                            }
                            break;
                        case MachineStatus.Running://运行状态,绿灯亮
                            {
                                Machine.do_红灯.OFF();
                                Machine.do_黄灯.OFF();
                                Machine.do_绿灯.ON();
                                Machine.do_蜂鸣器.OFF();
                            }
                            break;
                        case MachineStatus.Pause://暂停状态,黄灯亮
                            {
                                Machine.do_红灯.OFF();
                                Machine.do_黄灯.ON();
                                Machine.do_绿灯.OFF();
                                Machine.do_蜂鸣器.OFF();
                            }
                            break;
                        case MachineStatus.Alarm:
                        case MachineStatus.Emerg://急停/报警状态,红灯、蜂鸣器闪烁
                            {
                                Machine.do_红灯.Toggle();
                                Machine.do_黄灯.OFF();
                                Machine.do_绿灯.OFF();
                                Machine.do_蜂鸣器.Toggle();
                            }
                            break;
                        case MachineStatus.Quit://退出，关闭三色灯和蜂鸣器
                            {
                                Machine.do_红灯.OFF();
                                Machine.do_黄灯.OFF();
                                Machine.do_绿灯.OFF();
                                Machine.do_蜂鸣器.OFF();
                            }
                            return;
                        default:
                            break;
                    }
                }
                catch(Exception ex)
                {
                    MachineStatusManager.CurrentMachineStatus = MachineStatus.Alarm;
                    Log.Error("三色灯控制线程扫描异常,"+ex.StackTrace);
                    return;
                }
                Delay(500);
            }
        }

    }
}
