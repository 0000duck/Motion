using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Tanac.Core.MachineResources
{
	[Serializable]
	public class InputSetting
	{
		private int _Id;
		private string _CardName;

		private string _InputName;

		private int _Group;

		private int _Port;

		private bool _Status;

		public int ID
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
			}
		}
		public string InputName
		{
			get
			{
				return _InputName;
			}
			set
			{
				_InputName = value;
			}
		}

		public int Group
		{
			get
			{
				return _Group;
			}
			set
			{
				_Group = value;
			}
		}

		public int Port
		{
			get
			{
				return _Port;
			}
			set
			{
				_Port = value;
			}
		}

		public bool Status
		{
			get
			{
				return _Status;
			}
			set
			{
				_Status = value;
			}
		}
		[NonSerialized]
		public IOCard Card;
		/// <summary>
		/// 绑定卡的名称
		/// </summary>
		public string CardName
		{
			get
			{
				return _CardName;
			}
			set
			{
				_CardName = value;
			}
		}
		public InputSetting(string name, IOCard card, int port, int group = 0)
		{
			Card = card;
			CardName = card.CardName;
			InputName = name;
			Port = port;
			Group = group;
		}
		public bool BindCard()
		{
			IOCard ioCard = IOCardManager.Get(CardName);
			if (ioCard == null)
			{
				return false;
			}
			Card = ioCard;
			return true;
		}
		public bool GetStatus()
		{
			bool flag = false;
			try
			{
				return Card.GetDI(_Port,_Group);
			}
			catch (Exception ex)
			{
				throw new IOException("获取Output[:" + _InputName + "]状态时发生异常！" + ex.Message);
			}
		}

		public bool WaitON(int timeout = 2000)
		{
			bool result = false;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			try
			{
				while (!GetStatus())
				{
					if (stopwatch.ElapsedMilliseconds >= timeout)
					{
						return false;
					}
					Thread.Sleep(1);
				}
			}
			catch (Exception ex)
			{
				throw new IOException("InputSetting WaitON Exception:" + ex.Message);
			}
			return result;
		}

		public bool WaitOFF(int timeout = 2000)
		{
			bool result = false;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			try
			{
				while (GetStatus())
				{
					if (stopwatch.ElapsedMilliseconds >= timeout)
					{
						return false;
					}
					Thread.Sleep(1);
				}
			}
			catch (Exception ex)
			{
				throw new IOException("InputSetting WaitOFF Exception:" + ex.Message);
			}
			return result;
		}

		public bool KeepON(int keepTime = 10000)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			try
			{
				while (GetStatus())
				{
					if (stopwatch.ElapsedMilliseconds >= keepTime)
					{
						return true;
					}
					Thread.Sleep(10);
				}
			}
			catch (Exception ex)
			{
				throw new IOException("InputSetting KeepON Exception:" + ex.Message);
			}
			return false;
		}

		public bool KeepOFF(int keepTime = 10000)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			try
			{
				while (!GetStatus())
				{
					if (stopwatch.ElapsedMilliseconds >= keepTime)
					{
						return true;
					}
					Thread.Sleep(10);
				}
			}
			catch (Exception ex)
			{
				throw new IOException("InputSetting KeepOFF Exception:" + ex.Message);
			}
			return false;
		}
	}
}
