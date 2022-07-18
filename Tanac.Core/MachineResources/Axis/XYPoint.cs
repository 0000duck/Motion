using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
    [Serializable]
    public class XYPoint:PointPos
    {
        public XYPoint()
        {
        }

        public XYPoint(string name,Axis axisX, Axis axisY)
        {
            Name = name;
            AxisX = axisX;
            AxisY = axisY;
            AxisX_Name = axisX.Name;
            AxisY_Name = axisY.Name;
        }
        public override bool AbsMoveL()
        {
            //判断轴是否同一个板卡

            short[] nAixsArray = new short[2] { AxisX.AxisID, AxisY.AxisID};
            double[] fPosArray = new double[2] { XPos * AxisX.Rate, YPos * AxisY.Rate};
            bool res = AxisX.Card.AbsLineMove(nAixsArray, fPosArray, MoveVel * AxisX.Rate, MoveAcc * AxisX.Rate, MoveDec * AxisX.Rate);
            return res;
        }

        public override bool AbsMoveP()
        {
            throw new NotImplementedException();
        }

        public override bool BindAxis()
        {
            AxisX = AxisManager.Get(AxisX_Name);
            AxisY = AxisManager.Get(AxisY_Name);
            return true;
        }

        public override string GetPos()
        {
            string pos = $"{XPos},{YPos}";
            return pos;
        }

        public override bool SetPos(string str)
        {
            string[] array = str.Split(',');
            if(array.Length == 2)
            { 
               bool res1=double.TryParse(array[0], out double xpos);
               bool res2=double.TryParse(array[1], out double ypos);
               if(res1&&res2)
               {
                    XPos=xpos;
                    YPos=ypos;
                    return true;
               }
            }
            return false;
        }


        public override bool RelMoveL()
        {
            throw new NotImplementedException();
        }

        public override bool RelMoveP()
        {
            throw new NotImplementedException();
        }

        public override bool TeachPos()
        {
           AxisX.GetFeedbackPosition(ref XPos);
           AxisY.GetFeedbackPosition(ref YPos);
           return false;
        }
    }
}
