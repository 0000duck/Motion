using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tanac.Log4Net;
using Tanac.Utils;

namespace Tanac.Core.MachineResources
{
    /// <summary>
    /// /IO卡管理器
    /// </summary>
    public class IOCardManager
    {
		/// <summary>
		/// 配置文件路径
		/// </summary>
		private static string pathVar = Application.StartupPath + "\\Config\\IOCardParam.gra";
		/// <summary>
		/// IO卡插件目录路径
		/// </summary>
		public static readonly string PlugInsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IOCards\\");
		/// <summary>
		/// IO卡插件字典
		/// </summary>
		public static Dictionary<string, PluginsInfo> mPluginDic = new Dictionary<string, PluginsInfo>();

		public static List<IOCard> IOCardList = new List<IOCard>();

		/// <summary>
		/// 初始化插件
		/// </summary>
		public static void InitPlugin()
		{
			if (!Directory.Exists(PlugInsDir))
			{
				Directory.CreateDirectory(PlugInsDir);
			}
			string[] files = Directory.GetFiles(PlugInsDir);
			foreach (string fileName in files)
            {
				try
                {
					FileInfo fileInfo = new FileInfo(fileName);
					//判断是否是PlugIOCard.xxxxxxx.dll
					if (!fileInfo.Name.StartsWith("PlugIOCard.") || !fileInfo.Name.EndsWith(".dll")) continue;
					// 该方法会占用文件 但可以调试
					Assembly assemPlugIn = AppDomain.CurrentDomain.Load(Assembly.LoadFile(fileInfo.FullName).GetName());
					foreach (Type type in assemPlugIn.GetTypes())
					{
						//是IOCard的子类
						if (typeof(IOCard).IsAssignableFrom(type))
						{
							PluginsInfo info = new PluginsInfo();
							//获取插件名称
							if (GetPluginInfo(assemPlugIn, type, ref info))
							{
								mPluginDic[info.Name] = info;
							}
							break;
						}
					}
				}
				catch (Exception ex)
                {
					Log.Error("加载IO卡插件失败,"+ex.ToString());
				}
			}
		}
		/// <summary>
		/// 获取插件的信息
		/// </summary>
		/// <param name="type"></param>
		/// <param name="info"></param>
		/// <returns></returns>
		public static bool GetPluginInfo(Assembly assemPlugIn, Type type, ref PluginsInfo info)
		{
			try
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(CategoryAttribute), true);
				object[] customAttributes2 = type.GetCustomAttributes(typeof(DisplayNameAttribute), true);
				info.Category = ((CategoryAttribute)customAttributes[0]).Category;
				info.CardObjType = type;
				string displayName = ((DisplayNameAttribute)customAttributes2[0]).DisplayName;
				if (displayName.Contains("."))
				{
					try
					{
						string[] array = displayName.Split('.');
						info.SortNO = int.Parse(array[0]);
						info.Name = array[1];
					}
					catch (Exception ex)
					{
						Log.Error("插件名称分割 '.' 字符失败" + ex.ToString());
						return false;
					}
				}
				else
				{
					info.Name = displayName;
				}
				if (!string.IsNullOrWhiteSpace(info.Category) && !string.IsNullOrWhiteSpace(info.Name))
				{
					return true;
				}
			}
			catch (Exception ex2)
			{
				Log.Error("获取插件类别名称错误" + ex2.ToString());
			}
			return false;
		}

		/// <summary>
		/// 获取轴卡插件列表
		/// </summary>
		/// <param name="cameraPluginList"></param>
		public static List<string> GetCameraPluginList()
		{
			List<string> list = new List<string>();
			try
			{
				foreach (var mPligin in mPluginDic)
				{
					list.Add(mPligin.Key);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
			}
			return list;
		}

		/// <summary>
		/// 添加轴卡
		/// </summary>
		/// <param name="CardType"></param>
		public static void CreateIOCard(string cardType, int cardID)
		{
			try
			{
				PluginsInfo mPluginsInfo = mPluginDic[cardType];
				IOCard ioCard = (IOCard)mPluginsInfo.CardObjType.Assembly.CreateInstance(mPluginsInfo.CardObjType.FullName);
				string text = "IOCard";
				bool flag = false;
				int num = 0;
				while (true)
				{
					flag = true;
					foreach (IOCard value in IOCardList)
					{

						if (value.CardID == cardID & value.CardType == cardType)
						{
							Log.Error($"IO卡型号为:{cardType},卡号:{cardID}已经添加,无法重复添加");
							return;
						}

						if (value.Encode == num)
						{
							num++;
							flag = false;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				ioCard.CardName = text + num;
				ioCard.Encode = num;//卡编号
				ioCard.CardID = cardID;//卡号
				ioCard.CardRemarks = cardType + "_" + cardID;
				IOCardList.Add(ioCard);
			}
			catch (Exception ex)
			{
				Log.Error("轴卡添加失败" + ex.Message);
			}
		}

		public static IOCard Get(string name)
		{
			return IOCardList.SingleOrDefault((IOCard p) => p.CardName == name);
		}
		public static bool Add(IOCard cly)
		{
			IOCard cylinder = IOCardList.SingleOrDefault((IOCard p) => p == cly);
			if (cylinder == null)
			{
				IOCardList.Add(cly);
				return true;
			}
			return false;
		}
		public static bool Delete(string name)
		{
			IOCard variable = IOCardList.SingleOrDefault((IOCard p) => p.CardName == name);
			if (variable != null)
			{
				IOCardList.Remove(variable);
				return true;
			}
			return false;
		}
		public static void Clear()
		{
			IOCardList.Clear();
		}
		public static void SaveConfig()
		{
			SerializeUtils.BinarySerialize(pathVar, IOCardList);
		}
		public static void LoadConfig()
		{
			IOCardList = SerializeUtils.BinaryDeserialize<IOCard>(pathVar);
		}
		/// <summary>
		/// 所有卡初始化
		/// </summary>
		/// <returns></returns>
		public static bool AllCardInit()
		{
			IOCardList.ForEach(s =>
			{
				s.CardInit();
			});
			return true;
		}
	}
}
