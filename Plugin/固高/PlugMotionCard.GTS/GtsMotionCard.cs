using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanac.Core.MachineResources;
using gts;
using System.ComponentModel;
using System.Threading;
using Tanac.Log4Net;

namespace PlugMotionCard.GTS
{
    [Category("固高")]
    [DisplayName("GTS")]
    [Serializable]
    public class GtsMotionCard : MotionCard
    {
        /// <summary>
        /// 轴卡初始化
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override bool CardInit()
        {
            try
            {
                short Rnt = mc.GT_Open((short)CardID, 0, 1);
                Rnt += mc.GT_Reset((short)CardID);
                Rnt += mc.GT_Open((short)CardID, 0, 1);
                Rnt += mc.GT_LoadConfig((short)CardID, "GTS400_" + CardID.ToString() + ".cfg");
                if (Rnt==0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 轴卡关闭
        /// </summary>
        /// <returns></returns>
        public override bool Finalize()
        {
            try
            {
              if(mc.GT_Close((short)CardID)==0)
              {
                    return true;
              }
              else
              {
                return false;
              }
            }
            catch (Exception)
            { 
                return false;
            }
        }
        /// <summary>
        /// 伺服上电
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public override bool ServoOn(int axisId)
        {
            mc.GT_ClrSts((short)CardID, (short)axisId, 1);
            if (gts.mc.GT_AxisOn((short)CardID, (short)axisId)==0)
            {              
                return true;
            }
            return false;
        }
        /// <summary>
        /// 伺服断电
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public override bool ServoOff(int axisId)
        {
            if(mc.GT_AxisOff((short)CardID, (short)axisId)==0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 急停
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public override bool StopEmg(short axisId)
        {
            short rtn = mc.GT_Stop((short)CardID, 1 << (axisId - 1), 1 << (axisId-1));
            if (0 == rtn)
            {
                return true;
            }
            else
            {             
                return false;
            }
        }
        /// <summary>
        /// 清空位置
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public override bool ZeroPos(int axisId)
        {
           if(mc.GT_ZeroPos((short)CardID,(short)axisId,1)==0)
           {
                return true;
           }
           else
           {
                return false; 
           }
        }
        /// <summary>
        /// 获取轴IO状态
        /// </summary>
        /// <param name="axisId"></param>
        /// <param name="ioStatusBit"></param>
        /// <returns></returns>
        public override bool GetMotionIoSatus(short axisId, int ioStatusBit)
        {
            int sts = 0;
            uint clk = 0;
            if (0 == mc.GT_GetSts((short)CardID, (short)axisId, out sts, 1, out clk))
            {
                return 0 != (sts & (1 << ioStatusBit));
            }
            return true;
        }
        /// <summary>
        /// 使能信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public override bool GetServoOnStatus(short axisId)
        {
            return GetMotionIoSatus(axisId,9);
        }
        /// <summary>
        /// 到位信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public override bool GetINPStatus(short axisId)
        {
            return GetMotionIoSatus(axisId,11);
        }
        /// <summary>
        /// 报警信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public override bool GetALMStatus(short axisId)
        {
            return GetMotionIoSatus(axisId,1);
        }
        /// <summary>
        /// 急停信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public override bool GetEMGStatus(short axisId)
        {
            return GetMotionIoSatus(axisId, 8);
        }
        /// <summary>
        /// 正限位信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public override bool GetPELStatus(short axisId)
        {
            return GetMotionIoSatus(axisId, 5);
        }
        /// <summary>
        /// 负限位信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public override bool GetNELStatus(short axisId)
        {
            return GetMotionIoSatus(axisId, 6);
        }
        public override bool HomeMove(short axisId, Direction homeDir, double homeAcc,double homeDec, double homeVm, double homeVo)
        {
            short rtn;
            mc.THomePrm prm = new mc.THomePrm();
            rtn = mc.GT_ClrSts((short)CardID, (short)axisId,1);//清除状态
            rtn += mc.GT_AxisOn((short)CardID, (short)axisId);//伺服使能
            rtn += mc.GT_ZeroPos((short)CardID, (short)axisId, 1);//位置清零
            rtn += mc.GT_GetHomePrm((short)CardID, (short)axisId, out prm);//获取回零参数
            if (rtn != 0) return false;
            prm.mode = mc.HOME_MODE_LIMIT_HOME;//回零模式
            prm.homeOffset = 0;// 最终停止的位置相对于原点的偏移量
            switch (homeDir)
            {
                case Direction.POSITIVE:
                    prm.moveDir = 1;//回原方向
                    prm.indexDir = -1; //找相方向
                    break;
                case Direction.NEGATIVE:
                    prm.moveDir = -1;//回原方向
                    prm.indexDir = 1; //找相方向
                    break;
            }
            prm.searchHomeDistance = 0;// 设定的搜索 Home 的搜索范围，0 表示搜索距离为 805306368
            prm.searchIndexDistance = 0;//// 设定的搜索 Index 的搜索范围， 0 表示搜索距离为 805306368
            prm.smoothTime = 0;//回原点运动的平滑时间
            prm.velHigh = homeVm/1000;// 回原点运动的高速速度
            prm.velLow = homeVo/1000;// 回原点运动的低速速度
            prm.acc = homeAcc/Math.Pow(1000, 2);// 回原点运动的加速度
            prm.dec = homeDec/Math.Pow(1000, 2);// 回原点运动的减速度
            prm.edge = 1;// 设置捕获沿：0-下降沿，1-上升沿
            prm.escapeStep = 1000;// 采用“限位回原点” 方式时，反方向离开
            rtn=mc.GT_GoHome((short)CardID, (short)axisId, ref prm);//执行回零
            if (rtn == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否回原点完成
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns>-1:未到到位,0:回零完成,2:伺服报警,3:未使能</returns>
        public override int IsHomeNStop(short axisId)
        {
            mc.THomeStatus homeStatus = new mc.THomeStatus();
            mc.GT_GetHomeStatus((short)CardID, (short)axisId, out homeStatus);
            if(homeStatus.stage==mc.HOME_STAGE_END)//回原点已完成
            {
                if(homeStatus.error==mc.HOME_ERROR_NONE)
                {
                    Thread.Sleep(100);
                    return 0;
                }
                else if(homeStatus.error==mc.HOME_ERROR_DISABLE)
                {
                    return 3;//未使能
                }
                else if(homeStatus.error == mc.HOME_ERROR_DISABLE)
                {
                    return 2;//报警
                }
                else
                {
                    return homeStatus.error+10;//其他异常
                }
            }
            else
            {
                return -1;
            }


        }
        /// <summary>
        /// 轴是否正常停止
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns>-1:未到位,0:正常停止 1:急停 2:伺服报警 3:未使能 4:正限位 5:负限位</returns>
        public override int IsNStop(short axisId)
        {
            int sts = 0;
            uint clk = 0;
            if (0 == mc.GT_GetSts((short)CardID, (short)axisId, out sts, 1, out clk))
            {
                if (0 != (sts & (1 << 1)))
                {
                    return 2;   //驱动器异常报警
                }
                else if(0 != (sts & (1 << 8)))
                {
                    return 1;   //急停
                }
                else if (0 != (sts & (1 << 5)))
                {
                    return 4;   //正限位触发
                }
                else if (0 != (sts & (1 << 6)))
                {
                    return 5;   //负限位触发
                }
                else if (0 == (sts & (1 << 9)))
                {
                    return 3;//伺服掉电
                }

                if (0 == (sts & (1 << 10)))
                {
                    return 0;   //正常停止
                }
                return -1;  //正在运动中
            }
            return -1;
        }
        /// <summary>
        /// 轴定位完成
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns>-1:未到位,0:定位完成 1:急停 2:伺服报警 3:未使能 4:正限位 5:负限位</returns>
        public override int CheckMoveDone(short axisId)
        {
            int sts = 0;
            uint clk = 0;
            if (0 == mc.GT_GetSts((short)CardID, (short)axisId, out sts, 1, out clk))
            {
                if (0 != (sts & (1 << 1)))
                {
                    return 2;   //驱动器异常报警
                }
                else if (0 != (sts & (1 << 8)))
                {
                    return 1;   //急停
                }
                else if (0 != (sts & (1 << 5)))
                {
                    return 4;   //正限位触发
                }
                else if (0 != (sts & (1 << 6)))
                {
                    return 5;   //负限位触发
                }
                else if (0 == (sts & (1 << 9)))
                {
                    return 3;//伺服掉电
                }
                if (0 != (sts & (1 << 11)))
                {
                    return 0;   //轴到位完成
                }
                return -1;  //正在运动中
            }
            return -1;

        }

        /// <summary>
        /// 以绝对位置移动
        /// </summary>
        /// <param name="axisId"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public override bool AbsMove(short axisId, double vm, double acc, double dec, double position)
        {
            short rtn;
            mc.GT_SetAxisBand((short)CardID, (short)axisId,20, 5);
            mc.TTrapPrm trap = new mc.TTrapPrm();
            trap.acc = acc / Math.Pow(1000, 2);//加速度
            trap.dec =dec / Math.Pow(1000, 2);//减速度
            trap.velStart = 0;//起始速度
            trap.smoothTime = 25;//平滑时间
            rtn = mc.GT_ClrSts((short)CardID, (short)axisId, 1);//// 清除报警和限位
            rtn += mc.GT_AxisOn((short)CardID, (short)axisId);//伺服使能
            rtn += mc.GT_PrfTrap((short)CardID, (short)axisId);//设置成点位模式
            rtn += mc.GT_SetTrapPrm((short)CardID, (short)axisId, ref trap);//设置点位运动参数
            rtn += mc.GT_SetVel((short)CardID, (short)axisId, (double)vm/1000);//设置目标速度;
            rtn += mc.GT_SetPos((short)CardID, (short)axisId,(int)position);//设置目标位置
            if (rtn != 0) return false;
            rtn = mc.GT_Update((short)CardID,1 << (axisId - 1));//启动轴运动
            if(rtn == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool RelMove(short axisId, double vm, double acc, double dec, double position)
        {
            throw new NotImplementedException();
        }
        public override bool JogMoveStart(short axisId, Direction dir, double jogVm, double jogAcc)
        {
            short rtn;
            mc.TJogPrm trap = new mc.TJogPrm();
            rtn = mc.GT_ClrSts((short)CardID, (short)axisId, 1);//// 清除报警和限位
            rtn += mc.GT_AxisOn((short)CardID, (short)axisId);//伺服使能
            rtn += mc.GT_PrfJog((short)CardID, (short)axisId);//设置成点位模式
            rtn += mc.GT_GetJogPrm((short)CardID, (short)axisId, out trap);
            trap.acc = jogAcc/Math.Pow(1000,2);
            trap.dec = jogAcc/Math.Pow(1000, 2);
            rtn += mc.GT_SetJogPrm((short)CardID, (short)axisId, ref trap);//设置点位运动参数
            double setvel = 0;
            setvel = (dir == Direction.POSITIVE) ? jogVm:-jogVm;
            rtn += mc.GT_SetVel((short)CardID, (short)axisId, (double)setvel/1000);//设置目标速度;
            if(rtn!=0) return false;
            rtn += mc.GT_Update((short)CardID, 1 << (axisId - 1));//启动轴运动
            if (rtn == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool JogMoveStop(short axisId)
        {
            return StopMove(axisId);
        }
        public override bool IsMoving(short axisId)
        {
            throw new NotImplementedException();
        }
        public override bool StopMove(short axisId)
        {
            short rtn = mc.GT_Stop((short)CardID, 1 << (axisId - 1),0);
            if (0 == rtn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
 
        /// <summary>
        /// 获取规划位置
        /// </summary>
        /// <param name="axisId"></param>
        /// <param name="commandPosition"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override bool GetCmdPosition(short axisId, ref double commandPosition)
        {
            uint pLock = 0;
            if (mc.GT_GetAxisPrfPos((short)CardID,axisId,out commandPosition,1,out pLock)==0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取实际位置
        /// </summary>
        /// <param name="axisId"></param>
        /// <param name="feedbackPosition"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override bool GetFeedbackPosition(short axisId, ref double feedbackPosition)
        {
            uint pLock = 0;
            if (mc.GT_GetAxisEncPos((short)CardID, axisId, out feedbackPosition, 1, out pLock) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取实际位置与规划位置差值
        /// </summary>
        /// <param name="axisId"></param>
        /// <param name="errorPosition"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override bool GetErrorPosition(short axisId, ref double errorPosition)
        {
            uint pLock = 0;
            if (mc.GT_GetAxisError((short)CardID, axisId, out errorPosition, 1, out pLock) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 多轴直线插补
        /// </summary>
        /// <param name="nAixsArray"></param>
        /// <param name="fPosArray"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <returns></returns>
        public override bool AbsLineMove(short[] nAixsArray, double[] fPosArray, double vm, double acc, double dec)
        {
            short sRtn;
            mc.TCrdPrm crdPrm = new mc.TCrdPrm();
            short coordinateIndex = 1;
            crdPrm.dimension = (short)nAixsArray.Length;// 建立坐标系
            crdPrm.synVelMax = vm/1000;// 最大合成速度
            crdPrm.synAccMax = acc/Math.Pow(1000, 2);// 最大加速度
            crdPrm.evenTime = 0; // 最小匀速时间

            crdPrm.profile1 = nAixsArray.Length > 0 ? (short)nAixsArray[0] : (short)0; // 规划器1对应到X轴                       
            crdPrm.profile2 = nAixsArray.Length > 1 ? (short)nAixsArray[1] : (short)0; // 规划器2对应到Y轴
            crdPrm.profile3 = nAixsArray.Length > 2 ? (short)nAixsArray[2] : (short)0; // 规划器3对应到Z轴
            crdPrm.profile4 = nAixsArray.Length > 3 ? (short)nAixsArray[3] : (short)0;
            crdPrm.profile5 = nAixsArray.Length > 4 ? (short)nAixsArray[4] : (short)0;
            crdPrm.profile6 = nAixsArray.Length > 5 ? (short)nAixsArray[5] : (short)0;
            crdPrm.profile7 = nAixsArray.Length > 6 ? (short)nAixsArray[6] : (short)0;
            crdPrm.profile8 = nAixsArray.Length > 7 ? (short)nAixsArray[7] : (short)0;
            crdPrm.setOriginFlag = 1;// 需要设置加工坐标系原点位置
            crdPrm.originPos1 = 0;// 加工坐标系原点位置在(0,0,0)，即与机床坐标系原点重合
            crdPrm.originPos2 = 0;
            crdPrm.originPos3 = 0;
            crdPrm.originPos4 = 0;
            crdPrm.originPos5 = 0;
            crdPrm.originPos6 = 0;
            crdPrm.originPos7 = 0;
            crdPrm.originPos8 = 0;
            sRtn = mc.GT_SetCrdPrm((short)CardID, coordinateIndex, ref crdPrm);//设置坐标系参数
            sRtn += mc.GT_CrdClear((short)CardID, coordinateIndex, 0);//清空缓存区
            if(sRtn == 0)
            {
                if(nAixsArray.Length == 2)
                    sRtn+= mc.GT_LnXY((short)CardID, coordinateIndex, (int)fPosArray[0], (int)fPosArray[1], vm, acc,0, 0);
                else if(nAixsArray.Length==3)
                    sRtn+= mc.GT_LnXYZ((short)CardID, coordinateIndex, (int)fPosArray[0], (int)fPosArray[1], (int)fPosArray[2], vm, acc,0, 0);
                else if(nAixsArray.Length == 4)
                    sRtn+= mc.GT_LnXYZA((short)CardID, coordinateIndex, (int)fPosArray[0], (int)fPosArray[1], (int)fPosArray[2], (int)fPosArray[3], vm, acc,0, 0);
                sRtn+= mc.GT_CrdStart((short)CardID, 1, 0);//启动插补运动
            }
            if(sRtn == 0&&nAixsArray.Length<5)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public override bool RelLineMove(short[] nAixsArray, double[] fPosArray, double vm, double acc, double dec)
        {
            throw new NotImplementedException();
        }
    }
}
