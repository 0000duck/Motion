using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tanac.Utils;

namespace Tanac.Core.MachineResources
{
	public class InputManager
	{
		private static string pathVar = Application.StartupPath + "\\Config\\InputParam.gra";
		public static List<InputSetting> InputSettingList = new List<InputSetting>();
		public static InputSetting Get(string name)
		{
			return  InputSettingList.SingleOrDefault((InputSetting p) => p.InputName == name);
		}
		public static bool Add(InputSetting cly)
		{
			InputSetting cylinder = InputSettingList.SingleOrDefault((InputSetting p) => p.InputName == cly.InputName);
			if (cylinder == null)
			{
				InputSettingList.Add(cly);
				return true;
			}
			return false;
		}
		public static bool Delete(string name)
		{
			InputSetting variable = InputSettingList.SingleOrDefault((InputSetting p) => p.InputName == name);
			if (variable != null)
			{
				InputSettingList.Remove(variable);
				return true;
			}
			return false;
		}
		public static void Clear()
		{
			InputSettingList.Clear();
		}
		public static void BindCard()
		{
			InputSettingList.ForEach(s =>
			{
				s.BindCard();
			});
		}
		public static void SaveConfig()
		{
			SerializeUtils.BinarySerialize(pathVar, InputSettingList);
		}
		public static void LoadConfig()
		{
			InputSettingList = SerializeUtils.BinaryDeserialize<InputSetting>(pathVar);
		}
	}
}
