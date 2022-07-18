using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
    [Serializable]
    public class XYZPoint:PointPos
    {
        public XYZPoint(string name,Axis axisX, Axis axisY, Axis axisZ)
        {
            Name = name;
            AxisX = axisX;
            AxisY = axisY;
            AxisZ = axisZ;
            AxisX_Name = axisX.Name;
            AxisY_Name = axisY.Name;
            AxisZ_Name = axisZ.Name;
        }
        public override bool AbsMoveL()
        {
            short[] nAixsArray = new short[3] { AxisX.AxisID, AxisY.AxisID, AxisZ.AxisID};
            double[] fPosArray = new double[3] { XPos * AxisX.Rate, YPos * AxisY.Rate, ZPos * AxisZ.Rate};
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
            AxisZ = AxisManager.Get(AxisZ_Name);
            return true;
        }

        public override string GetPos()
        {
            string pos = $"{XPos},{YPos},{ZPos}";
            return pos;
        }
        public override bool SetPos(string str)
        {
            string[] array = str.Split(',');
            if (array.Length == 3)
            {
                bool res1 = double.TryParse(array[0], out double xpos);
                bool res2 = double.TryParse(array[1], out double ypos);
                bool res3 = double.TryParse(array[2], out double zpos);
                if (res1 && res2&&res3)
                {
                    XPos = xpos;
                    YPos = ypos;
                    ZPos = zpos;
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
            AxisZ.GetFeedbackPosition(ref ZPos);
            return false;
        }
    }
}
