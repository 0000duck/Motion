using gts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Tanac.Core.MachineResources;

namespace PlugIOCard.GTS
{
    [Category("固高")]
    [DisplayName("GTS")]
    [Serializable]
    public class GtsIOCard : IOCard
    {
        public GtsIOCard()
        {
            base.InputCount = 16;
            base.OutputCount = 16;
            base.CardType = "GTS";
        }
        [HandleProcessCorruptedStateExceptions]
        public override bool CardInit()
        {
            try
            {
                short nRtn = mc.GT_Open((short)CardID, 0, 1);
                if (nRtn == 0) return true;
            }
            catch (Exception ex)
            {
                throw new Exception("GtsIOCard CardInit Exception:" + ex.StackTrace);
            }
            return false;
        }
        [HandleProcessCorruptedStateExceptions]
        public override bool GetDI(int port, int group = 0)
        {
            int nData = 0;

            try
            {
                if (0 == mc.GT_GetDi((short)CardID, mc.MC_GPI, out nData))
                {
                    nData = ~nData;
                    return 0 != (nData & (1 << (port - 1)));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GtsIOCard GetDI Exception:" + ex.StackTrace);
            }
            return false;
        }
        [HandleProcessCorruptedStateExceptions]
        public override bool GetDO(int port, int group = 0)
        {
            int nData = 0;
            try
            {
                if (0 == mc.GT_GetDo((short)CardID, mc.MC_GPO, out nData))
                {
                    nData = ~nData;
                    return 0 != (nData & (1 << (port-1)));
                }

            }
            catch (Exception ex)
            {
                throw new Exception("GtsIOCard GetDO Exception:" + ex.StackTrace);
            }
            return false;
        }
        [HandleProcessCorruptedStateExceptions]
        public override bool SetDO(int port, int ONOrOFF, int group = 0)
        {
            int ret = 0;
            try
            {
                ret=mc.GT_SetDoBit((short)CardID, mc.MC_GPO, (short)port, (short)ONOrOFF);
                if (ret == 0) return true;
            }
            catch(Exception ex)
            {
                throw new Exception("GtsIOCard SetDO Exception:" + ex.StackTrace);
            }
            return false;
        }

    }
}
