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
	/// 轴管理器
	/// </summary>
    public class AxisManager
    {
		private static string pathVar = Application.StartupPath + "\\Config\\AxisParam.gra";
		public static List<Axis> AxisList = new List<Axis>();
		public static Axis Get(string name)
		{
			return AxisList.SingleOrDefault((Axis p) => p.Name == name);
		}
		public static bool Add(Axis axis)
		{
			Axis variable = AxisList.SingleOrDefault((Axis p) => p.Name == axis.Name);
			if (variable == null)
			{
				AxisList.Add(axis);
				return true;
			}
			return false;
		}
		public static bool Delete(string name)
		{
			Axis variable = AxisList.SingleOrDefault((Axis p) => p.Name == name);
			if (variable != null)
			{
				AxisList.Remove(variable);
				return true;
			}
			return false;
		}
		public static void Clear()
		{
			AxisList.Clear();
		}
		public static void BindCard()
		{
			AxisList.ForEach(s =>
			{
				s.BindCard();
			});
		}

		public static void AllStopEmg()
		{
			AxisList.ForEach(s =>
			{
				s.StopEmg();
			});
		}
		public static void AllStopMove()
		{
			AxisList.ForEach(s =>
			{
				s.StopMove();
			});
		}

		public static void SaveConfig()
		{
			SerializeUtils.BinarySerialize(pathVar,AxisList);
		}
		public static void LoadConfig()
		{
			AxisList = SerializeUtils.BinaryDeserialize<Axis>(pathVar);
		}
	}
}
