using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
    [Serializable]
    public class XPoint : PointPos
    {
        public XPoint(string name,Axis axisX)
        {
            AxisX = axisX;
            AxisX_Name = axisX.Name;
            Name = name;
        }

        public XPoint()
        {
        }

        public override bool BindAxis()
        {
            AxisX = AxisManager.Get(AxisX_Name);
            return true;
        }
        /// <summary>
        /// 获取点位信息
        /// </summary>
        /// <returns></returns>
        public override string GetPos()
        {
            string pos = $"{XPos}";
            return pos;
        }
        public override bool SetPos(string str)
        {
            string[] array = str.Split(',');
            if (array.Length == 1)
            {
                bool res1 = double.TryParse(array[0], out double xpos);
                if (res1)
                {
                    XPos = xpos;
                    return true;
                }
            }
            return false;
        }
        public override bool TeachPos()
        {
            AxisX.GetFeedbackPosition(ref XPos);
            return true;
        }
        public override bool AbsMoveL()
        {
            return false;
        }
        public override bool AbsMoveP()
        {
            bool res=AxisX.AbsMove(XPos);
            return res;
        }
        public override bool RelMoveL()
        {
            throw new NotImplementedException();
        }
        public override bool RelMoveP()
        {
            throw new NotImplementedException();
        }
    }
}
