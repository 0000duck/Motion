using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
    [Serializable]
    public abstract class MotionCard
    {
        /// <summary>
        /// 卡的编号
        /// </summary>
        public int Encode { get; set; }
        /// <summary>
        /// 卡的名称
        /// </summary>
        public string CardName { get; set; }
        /// <summary>
        /// 同一品牌下的卡号
        /// </summary>
        public int CardID { get; set; }
        /// <summary>
        /// 卡的类型
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 卡的备注
        /// </summary>
        public string CardRemarks { get; set; }
        /// <summary>
        /// 板卡初始化
        /// </summary>
        /// <returns></returns>
        public abstract bool CardInit();
        /// <summary>
        /// 板卡反初始化
        /// </summary>
        /// <returns></returns>
        public abstract bool Finalize();
        public abstract bool GetCmdPosition(short axisId, ref double commandPosition);
        public abstract bool GetFeedbackPosition(short axisId, ref double feedbackPosition);
        public abstract bool GetErrorPosition(short axisId, ref double errorPosition);

        /// <summary>
        /// 获取轴的IO状态
        /// </summary>
        /// <param name="axisId"></param>
        /// <param name="ioStatusBit"></param>
        /// <returns></returns>
        public abstract bool GetMotionIoSatus(short axisId, int ioStatusBit);

        /// <summary>
        /// 使能信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool GetServoOnStatus(short axisId);
        /// <summary>
        /// 到位信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool GetINPStatus(short axisId);
        /// <summary>
        /// 报警信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool GetALMStatus(short axisId);
        /// <summary>
        /// 急停信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool GetEMGStatus(short axisId);
        /// <summary>
        /// 正限位信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool GetPELStatus(short axisId);
        /// <summary>
        /// 负限位信号
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool GetNELStatus(short axisId);

        /// <summary>
        /// 单轴使能
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool ServoOn(int axisId);
        /// <summary>
        /// 单轴使能失效
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool ServoOff(int axisId);
        /// <summary>
        /// 急停
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool StopEmg(short axisId);
        public abstract bool ZeroPos(int axisId);
        public abstract bool HomeMove(short axisId, Direction homeDir, double homeAcc,double homeDec,double homeVm,double homeVo);
        /// <summary>
        /// 判断是否回零完成
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract int IsHomeNStop(short axisId);
        /// <summary>
        /// 轴是否正常停止
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns>-1:未到位,0:正常停止 1:急停 2:伺服报警 3:未使能 4:正限位 5:负限位</returns>
        public abstract int IsNStop(short axisId);

        /// <summary>
        /// 绝对运动
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool AbsMove(short axisId, double vm, double acc, double dec, double position);
        /// <summary>
        /// 相对运动
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool RelMove(short axisId, double vm, double acc, double dec, double position);
        /// <summary>
        /// Jog点动开始
        /// </summary>
        /// <param name="axisId"></param>
        /// <param name="dir"></param>
        /// <param name="jogVm"></param>
        /// <param name="jogAcc"></param>
        /// <returns></returns>
        public abstract bool JogMoveStart(short axisId,Direction dir,double jogVm,double jogAcc);
        /// <summary>
        /// JOG点动停止
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool JogMoveStop(short axisId);
        /// <summary>
        /// 检查轴是否在运动
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool IsMoving(short axisId);
        /// <summary>
        /// 减速停止
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract bool StopMove(short axisId);
        /// <summary>
        /// 检测运动是否完成
        /// </summary>
        /// <param name="axisId"></param>
        /// <returns></returns>
        public abstract int CheckMoveDone(short axisId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nAixsArray">轴数组</param>
        /// <param name="fPosArray">坐标数组</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <returns></returns>
        public abstract bool AbsLineMove(short[] nAixsArray,double[] fPosArray,double vm,double acc,double dec);
        public abstract bool RelLineMove(short[] nAixsArray, double[] fPosArray, double vm, double acc, double dec);
    }
}
