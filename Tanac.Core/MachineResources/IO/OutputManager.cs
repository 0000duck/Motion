using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tanac.Utils;

namespace Tanac.Core.MachineResources
{
	/// <summary>
	/// IO输出配置管理器
	/// </summary>
    public class OutputManager
    {
		private static string pathVar = Application.StartupPath + "\\Config\\OutputParam.gra";
		public static List<OutputSetting> OutputSettingList = new List<OutputSetting>();
		public static OutputSetting Get(string name)
		{
			return OutputSettingList.SingleOrDefault((OutputSetting p) => p.OutputName == name);
		}
		public static bool Add(OutputSetting cly)
		{
			OutputSetting cylinder = OutputSettingList.SingleOrDefault((OutputSetting p) => p == cly);
			if (cylinder == null)
			{
				OutputSettingList.Add(cly);
				return true;
			}
			return false;
		}
		public static bool Delete(string name)
		{
			OutputSetting variable = OutputSettingList.SingleOrDefault((OutputSetting p) => p.OutputName == name);
			if (variable != null)
			{
				OutputSettingList.Remove(variable);
				return true;
			}
			return false;
		}
		public static void BindCard()
        {
			OutputSettingList.ForEach(s =>
			{
				s.BindCard();
			});
		}
		public static void Clear()
		{
			OutputSettingList.Clear();
		}
		public static void SaveConfig()
		{
			SerializeUtils.BinarySerialize(pathVar, OutputSettingList);
		}
		public static void LoadConfig()
		{
			OutputSettingList = SerializeUtils.BinaryDeserialize<OutputSetting>(pathVar);
		}
	}
}
