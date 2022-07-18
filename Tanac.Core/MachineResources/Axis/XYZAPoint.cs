using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
    [Serializable]
    public class XYZAPoint:PointPos
    {
        public XYZAPoint(string name,Axis axisX,Axis axisY,Axis axisZ,Axis axisA)
        {
            Name = name;
            AxisX = axisX;
            AxisY = axisY;
            AxisZ = axisZ;
            AxisA = axisA;
            AxisX_Name = axisX.Name;
            AxisY_Name = axisY.Name;
            AxisZ_Name = axisZ.Name;
            AxisA_Name = axisA.Name;
        }

        public override bool AbsMoveL()
        {
            short[] nAixsArray = new short[4] { AxisX.AxisID, AxisY.AxisID, AxisZ.AxisID,AxisA.AxisID };
            double[] fPosArray = new double[4] { XPos*AxisX.Rate,YPos * AxisY.Rate, ZPos*AxisZ.Rate, APos*AxisA.Rate };
            bool res = AxisX.Card.AbsLineMove(nAixsArray, fPosArray, MoveVel*AxisX.Rate, MoveAcc*AxisX.Rate, MoveDec*AxisX.Rate);
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
            AxisA = AxisManager.Get(AxisA_Name);
            return true;
        }

        public override string GetPos()
        {
            
            string pos = $"{XPos},{YPos},{ZPos},{APos}";
            
            return pos;
        }
        public override bool SetPos(string str)
        {
            string[] array = str.Split(',');
            if (array.Length == 4)
            {
                bool res1 = double.TryParse(array[0], out double xpos);
                bool res2 = double.TryParse(array[1], out double ypos);
                bool res3 = double.TryParse(array[2], out double zpos);
                bool res4 = double.TryParse(array[3], out double apos);
                if (res1 && res2&&res3&&res4)
                {
                    XPos = xpos;
                    YPos = ypos;
                    ZPos = zpos;
                    APos = apos;
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
            AxisA.GetFeedbackPosition(ref APos);
            return true;
        }
    }
}
