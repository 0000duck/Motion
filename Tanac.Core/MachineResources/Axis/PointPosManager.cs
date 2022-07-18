using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tanac.Utils;

namespace Tanac.Core.MachineResources
{
    /// <summary>
    /// 点位管理器
    /// </summary>
    public class PointPosManager
    {
		private static string pathVar = Application.StartupPath + "\\Config\\PointParam.xml";
		public static List<PointPos> PointPosList = new List<PointPos>();
		public static PointPos Get(string name)
		{
			return PointPosList.SingleOrDefault((PointPos p) => p.Name == name);
		}
		public static bool Add(PointPos pos)
		{
			PointPos variable = PointPosList.SingleOrDefault((PointPos p) => p.Name == pos.Name);
			if (variable == null)
			{
				PointPosList.Add(variable);
				return true;
			}
			return false;
		}
		public static bool Delete(string name)
		{
			PointPos variable = PointPosList.SingleOrDefault((PointPos p) => p.Name == name);
			if (variable != null)
			{
				PointPosList.Remove(variable);
				return true;
			}
			return false;
		}
		public static void Clear()
		{
			PointPosList.Clear();
		}
		public static void BindAxis()
		{
			PointPosList.ForEach(s =>
			{
				s.BindAxis();
			});
		}
		public static void SaveConfig()
		{
			SerializeUtils.BinarySerialize(pathVar, PointPosList);
		}
		public static void LoadConfig()
		{
			PointPosList = SerializeUtils.BinaryDeserialize<PointPos>(pathVar);
		}
	}
}
