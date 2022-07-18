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
	/// 气缸管理器
	/// </summary>
    public class CylinderManager
    {
        private static string pathVar = Application.StartupPath + "\\Config\\CylinderParam.gra";
		public static List<Cylinder> CylinderList = new List<Cylinder>();
		public static Cylinder Get(string name)
		{
			return CylinderList.SingleOrDefault((Cylinder p) => p.Name == name);
		}
		public static bool Add(Cylinder cly)
		{
			Cylinder cylinder = CylinderList.SingleOrDefault((Cylinder p) => p.Name == cly.Name);
			if (cylinder == null)
			{
				CylinderList.Add(cly);
				return true;
			}
			return false;
		}
		public static bool Delete(string name)
		{
			Cylinder variable = CylinderList.SingleOrDefault((Cylinder p) => p.Name == name);
			if (variable != null)
			{
				CylinderList.Remove(variable);
				return true;
			}
			return false;
		}
		public static void Clear()
		{
			CylinderList.Clear();
		}
		public static void BindIoSetting()
        {
			CylinderList.ForEach(s =>
			{
				s.BindIoSetting();
			});
		}
		public static void SaveConfig()
		{
			SerializeUtils.BinarySerialize(pathVar, CylinderList);
		}
		public static void LoadConfig()
		{
			CylinderList = SerializeUtils.BinaryDeserialize<Cylinder>(pathVar);
		}
	}
}
