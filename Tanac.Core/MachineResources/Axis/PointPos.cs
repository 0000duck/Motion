using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
    public enum PointPosType
    {
        X,
        XY,
        XYZ,
        XYZA,
    }
    [Serializable]
    public abstract class PointPos
    {
        public double XPos;
        public double YPos;
        public double ZPos;
        public double APos;
        public string AxisX_Name;
        public string AxisY_Name;
        public string AxisZ_Name;
        public string AxisA_Name;
        [NonSerialized]
        public Axis AxisX;
        [NonSerialized]
        public Axis AxisY;
        [NonSerialized]
        public Axis AxisZ;
        [NonSerialized]
        public Axis AxisA;
        /// <summary>
        /// 点位名称
        /// </summary>
        public string Name;
        public PointPosType Type;
        /// <summary>
        /// 点位描述
        /// </summary>
        public string Description;
        /// <summary>
        /// 移动速度
        /// </summary>
        public double MoveVel;
        /// <summary>
        /// 移动加速度
        /// </summary>
        public double MoveAcc;
        /// <summary>
        /// 移动减速度
        /// </summary>
        public double MoveDec;

        /// <summary>
        /// 绑定轴
        /// </summary>
        /// <returns></returns>
        public abstract bool BindAxis();
        /// <summary>
        /// 示教点位
        /// </summary>
        /// <returns></returns>
        public abstract bool TeachPos();
        public abstract string GetPos();
        public abstract bool SetPos(string str);
        public abstract bool AbsMoveL();
        public abstract bool AbsMoveP();
        public abstract bool RelMoveL();
        public abstract bool RelMoveP();
    }
}
