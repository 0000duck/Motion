using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tanac.Log4Net;

namespace Tanac.Comm
{
	public class EComMgr
	{
		private static Dictionary<string, ECommunacation> s_ECommunacationDic;

		private static object s_Obj;

		public static int s_SendIntervalTime;

		public static DateTime s_LastSendTime;

		static EComMgr()
		{
			s_ECommunacationDic = new Dictionary<string, ECommunacation>();
			s_Obj = new object();
			s_SendIntervalTime = 0;
		}

		public static List<ECommunacation> GetEcomList()
		{
			return s_ECommunacationDic.Values.ToList();
		}
		/// <summary>
		/// 反序列化后刷新的作用
		/// </summary>
		/// <param name="eComList"></param>
		public static void SetEcomList(List<ECommunacation> eComList)
		{
			foreach (string key in s_ECommunacationDic.Keys)
			{
				s_ECommunacationDic[key].DisConnect();
			}
			s_ECommunacationDic.Clear();
			if (eComList == null)
			{
				return;
			}
			foreach (ECommunacation eCom in eComList)
			{
				s_ECommunacationDic[eCom.Key] = eCom;
				eCom.Connect();
			}
		}

		public static List<EComInfo> GetKeyList()
		{
			List<EComInfo> list = new List<EComInfo>();
			foreach (string item2 in s_ECommunacationDic.Keys.ToList())
			{
				EComInfo item = new EComInfo(item2, s_ECommunacationDic[item2].IsConnected, s_ECommunacationDic[item2].CommunicationModel);
				list.Add(item);
			}
			return list;
		}
		public static List<string> GetKeyStrList()
		{
			List<string> list = new List<string>();
			foreach (string item2 in s_ECommunacationDic.Keys.ToList())
			{
				EComInfo item = new EComInfo(item2, s_ECommunacationDic[item2].IsConnected, s_ECommunacationDic[item2].CommunicationModel);
				list.Add(item.Key);
			}
			return list;
		}
		public static string GetRemarks(string key)
		{
			ECommunacation ECom = s_ECommunacationDic.Values.FirstOrDefault((ECommunacation c) => c.Key == key);
			if (ECom != null)
			{
				return ECom.Remarks;
			}
			return "";
		}

		public static ECommunacation GetECommunacation(string key)
		{
			if (s_ECommunacationDic.ContainsKey(key))
			{
				return s_ECommunacationDic[key];
			}
			return null;
		}

		public static string CreateECom(CommunicationModel communicationModel)
		{
			ECommunacation ECom = new ECommunacation();
			ECom.CommunicationModel = communicationModel;
			string text = "";
			switch (communicationModel)
			{
				case CommunicationModel.TcpClient:
					text = "TCP客户端";
					break;
				case CommunicationModel.TcpServer:
					text = "TCP服务端";
					break;
				case CommunicationModel.UDP:
					text = "UDP";
					break;
				case CommunicationModel.COM:
					text = "串口";
					break;
			}
			bool flag = false;
			int num = 0;
			while (true)
			{
				flag = true;
				foreach (ECommunacation value in s_ECommunacationDic.Values)
				{
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
				bool flag2 = true;
			}
			text = (ECom.Key = text + num);
			ECom.Encode = num;
			s_ECommunacationDic[text] = ECom;
			return text;
		}

		public static void DeleteECom(string key)
		{
			if (s_ECommunacationDic.ContainsKey(key))
			{
				ECommunacation ECom = s_ECommunacationDic[key];
				ECom.DisConnect();
				s_ECommunacationDic.Remove(key);
			}
		}

		public static bool Connect(string key, int intEndSymbol)
		{
			try
			{
				if (!s_ECommunacationDic.ContainsKey(key))
				{
					return false;
				}
				ECommunacation ECom = s_ECommunacationDic[key];
				if (intEndSymbol != -1)
				{
					ECom.EndSymbol = (EndSymbol)intEndSymbol;
				}
				if (ECom.CommunicationModel == CommunicationModel.UDP && ECom.EndSymbol > EndSymbol.None)
				{
					Log.Warn("[" + key + "] 接收数据 不支持读取截止符");
				}
				return ECom.Connect();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
		}

		public static void DisConnect(string key)
		{
			try
			{
				if (s_ECommunacationDic.ContainsKey(key))
				{
					ECommunacation ECom = s_ECommunacationDic[key];
					ECom.DisConnect();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
		}

		public static void DisConnectAll()
		{
			foreach (ECommunacation value in s_ECommunacationDic.Values)
			{
				value.DisConnect();
			}
		}

		public static bool SendStr(string key, string str)
		{
			try
			{
				if (!s_ECommunacationDic.ContainsKey(key))
				{
					return false;
				}
				ECommunacation ECom = s_ECommunacationDic[key];
				bool result = false;
				if (s_SendIntervalTime > 0)
				{
					lock (s_Obj)
					{
						_ = s_LastSendTime;
						if (true)
						{
							double totalMilliseconds = DateTime.Now.Subtract(s_LastSendTime).TotalMilliseconds;
							if (totalMilliseconds < (double)s_SendIntervalTime)
							{
								int millisecondsTimeout = (int)((double)s_SendIntervalTime - totalMilliseconds);
								Thread.Sleep(millisecondsTimeout);
							}
						}
						result = ECom.SendStr(str);
						s_LastSendTime = DateTime.Now;
					}
				}
				else
				{
					result = ECom.SendStr(str);
				}
				return result;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
		}

		public static bool GetEcomRecStr(string key, out string pReturnStr, int timeOut)
		{
			pReturnStr = "";
			try
			{
				if (!s_ECommunacationDic.ContainsKey(key))
				{
					return false;
				}
				ECommunacation ECom = s_ECommunacationDic[key];
				ECom.TimeOut = timeOut;
				ECom.GetStr(out pReturnStr);
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
		}
		/// <summary>
		/// 停止接收堵塞
		/// </summary>
		/// <param name="key"></param>
		public static void StopRecStrSignal(string key)
		{
			try
			{
				if (s_ECommunacationDic.ContainsKey(key))
				{
					ECommunacation ECom = s_ECommunacationDic[key];
					ECom.StopRecStrSignal();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
		}
	}
}
