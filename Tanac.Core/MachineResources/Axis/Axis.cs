using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tanac.Log4Net;

namespace Tanac.Core.MachineResources
{
    public enum Direction
    {
        POSITIVE = 0,
        NEGATIVE
    };

    [Serializable]
    public class Axis
    {
        /// <summary>
        /// 绑定轴卡名称
        /// </summary>
        public string CardName;
        /// <summary>
        /// 轴号
        /// </summary>
        public short AxisID;
        /// <summary>
        /// 轴名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 脉冲与实际位置换算比例
        /// </summary>
        public double Rate=1;
        /// <summary>
        /// 诊断到位与否的误差脉冲数
        /// </summary>
        public int Band=20;
        /// <summary>
        /// 回零速度
        /// </summary>
        public double HomeVel;
        /// <summary>
        /// 回零加速度
        /// </summary>
        public double HomeAcc=10;
        /// <summary>
        /// 回零减速度
        /// </summary>
        public double HomeDec=10;
        /// <summary>
        /// 回零方向
        /// </summary>
        public Direction HomeDir;
        /// <summary>
        /// 移动速度
        /// </summary>
        public double MoveVel;
        /// <summary>
        /// 移动加速度
        /// </summary>
        public double MoveAcc=10;
        /// <summary>
        /// 移动减速度
        /// </summary>
        public double MoveDec=10;
        /// <summary>
        /// JOG速度
        /// </summary>
        public double JogVel;
        /// <summary>
        /// JOG加速度
        /// </summary>
        public double JogAcc=10;
        /// <summary>
        /// JOG减速度
        /// </summary>
        public double JogDec=10;
        [NonSerialized]
        public MotionCard Card;

        public Axis(short axisID, string name, MotionCard card)
        {
            AxisID = axisID;
            Name = name;
            Card = card;
            CardName = card.CardName;
        }

        public Axis()
        {
        }

        /// <summary>
        /// 绑定卡
        /// </summary>
        /// <returns></returns>
        public bool BindCard()
        {
           MotionCard card = MotionCardManager.Get(CardName);
           if(card == null) return false;
           Card=card;
           return true;
        }
        /// <summary>
        /// 使能关闭
        /// </summary>
        /// <returns></returns>
        public bool ServoOff()
        {
            return Card.ServoOff(AxisID);
        }
        /// <summary>
        /// 使能打开
        /// </summary>
        /// <returns></returns>
        public bool ServoOn()
        {
           return Card.ServoOn(AxisID);
        }
        /// <summary>
        /// 获取使能状态
        /// </summary>
        /// <returns></returns>
        public bool GetServoOnStatus()
        {
             return Card.GetServoOnStatus(AxisID); ;
        }
        /// <summary>
        /// 急停
        /// </summary>
        /// <returns></returns>
        public bool StopEmg()
        {
            return Card.StopEmg(AxisID); ;
        }
        /// <summary>
        /// 停止移动
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public bool StopMove()
        {
            return Card.StopMove(AxisID);
        }
        public bool GetCmdPosition(ref double position)
        {
            double commandPosition = position * Rate;
            bool res=Card.GetCmdPosition(AxisID,ref commandPosition);
            if(res)
            {
                position = commandPosition/Rate;
                return true;
            }
            return false;
        }
        public bool GetFeedbackPosition(ref double position)
        {
            double feedbackPosition = position * Rate;
            bool res=Card.GetFeedbackPosition(AxisID,ref feedbackPosition);
            if (res)
            {
                position = feedbackPosition/Rate;
                return true;
            }
            return false;
        }
        public bool HomeMove()
        {
            return Card.HomeMove(AxisID,HomeDir,HomeAcc * Rate, HomeDec * Rate, HomeVel * Rate, HomeVel*Rate/2);
        }
        /// <summary>
        /// 等待回零完成
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitHomeDone(int timeout = int.MaxValue)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            while (true)
            {
                int res = Card.IsHomeNStop(AxisID);
                switch (res)
                {
                    case -1:
                        Thread.Sleep(10);
                        if (stopwatch.ElapsedMilliseconds >= timeout)
                        {
                            AxisAlarmPause(7);
                            //回零超时报警
                            return false;
                        }
                        break;
                    case 0:
                        return true;
                    default:
                        {
                            AxisAlarmPause(res);
                            return false;
                        }
                }

            }
        }
        /// <summary>
        /// 等待运动完成
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitMoveDone(int timeout = int.MaxValue)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            while(true)
            {
                int res=Card.CheckMoveDone(AxisID);
                switch (res)
                {
                    case -1:
                        Thread.Sleep(10);
                        if (stopwatch.ElapsedMilliseconds >= timeout)
                        {
                            AxisAlarmPause(8);
                            //定位超时报警
                            return false;
                        }
                        break;
                    case 0:
                        return true;
                    default:
                    {
                        AxisAlarmPause(res);
                        return false;   
                    }
                }

            }
        }
        public bool AbsMove(double position)
        {
            return Card.AbsMove(AxisID, MoveVel*Rate, MoveAcc * Rate, MoveDec * Rate, position * Rate);
        }
        public bool AbsMove(double vm, double acc, double dec, double position)
        {
            return Card.AbsMove(AxisID, vm*Rate, acc*Rate, dec*Rate, position*Rate);
        }
        public bool RelMove(double position)
        {
            return false;
        }
        public bool RelMove(double vm, double acc, double dec, double position)
        {
            return false;
        }
        public bool JogMoveStart(Direction dir)
        {
            return Card.JogMoveStart(AxisID, dir, JogVel*Rate, JogAcc*Rate);
        }
        public bool JogMoveStop()
        {
            return Card.JogMoveStop(AxisID);
        }

        /// <summary>
        /// 报警并暂停
        /// </summary>
        /// <param name="intError">错误类型</param>
        /// <param name="bShowDialog">是否显示提示对话框</param>
        /// <param name="bPause">设备是否暂停</param>
        protected void AxisAlarmPause(int intError,bool bShowDialog = true, bool bPause = true)
        {
            string[] strError = new string[9] {"未知错误","急停", "报警", "未励磁", "正限位", "负限位", "停止后位置超限","回零超时","定位超时"};
            string excMsg = "未知错误";
            if (intError>0&intError<9)
            {
                excMsg=strError[intError];
            }
            Log.Error(Name+excMsg);
            if (bPause)
            {
                MachineStatusManager.CurrentMachineStatus = MachineStatus.Alarm;//切换设备成报警
            }
            if (bShowDialog)
            {
                ShowAlarmDlg(Name+"报警", excMsg);
            }
        }
        /// <summary>
        /// 显示错误提示对话框
        /// </summary>
        protected DialogResult ShowAlarmDlg(string title, string excMsg)
        {
            DateTime startTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DialogResult dr = MessageBox.Show(excMsg,title,MessageBoxButtons.OKCancel);
            DateTime endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString());
            int timeCount = (endTime - startTime).Seconds;
            return dr;
        }
    }
}
