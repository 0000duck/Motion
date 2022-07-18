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
	public class OutputSetting
	{
		private int _Id;

		private string _CardName;

		private string _OutputName;

		private int _Group;

		private int _Port;

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

		public string OutputName
		{
			get
			{
				return _OutputName;
			}
			set
			{
				_OutputName = value;
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
		[NonSerialized]
		public IOCard Card;

        public OutputSetting(string name,IOCard card,int port,int group=0)
        {
			Card = card;
			CardName = card.CardName;
			OutputName = name;
			Port = port;
			Group = group;
        }

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
		public bool BindCard()
        {
		   IOCard ioCard=IOCardManager.Get(CardName);
		   if(ioCard==null)
           {
				return false;
           }
		   Card=ioCard;
		   return true;
		}
		public bool ON(int timeout = 2000)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			bool flag = false;
			int num = 1;
			try
			{
				if (GetStatus())
				{
					return true;
				}
				flag = Card.SetDO(_Port,0,_Group);
				if (flag && GetStatus())
				{
					return true;
				}
				while (!GetStatus() || !flag)
				{
					if (stopwatch.ElapsedMilliseconds > timeout)
					{
						return false;
					}
					flag = Card.SetDO(_Port,0, _Group);
					Thread.Sleep(1);
				}
				return true;
			}
			catch (Exception ex)
			{
				throw new IOException("设置Output[:" + OutputName + "]时发生异常！" + ex.Message);
			}
		}

		public bool OFF(int timeout = 2000)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			bool flag = false;
			int num = 0;
			try
			{
				if (!GetStatus())
				{
					return true;
				}
				flag = Card.SetDO(_Port,1,_Group);
				if (flag && !GetStatus())
				{
					return flag;
				}
				while (GetStatus() || !flag)
				{
					if (stopwatch.ElapsedMilliseconds > timeout)
					{
						return false;
					}
					flag = Card.SetDO(_Port,1,_Group);
					Thread.Sleep(1);
				}
				return true;
			}
			catch (Exception ex)
			{
				throw new IOException("设置Output[:" + OutputName + "]时发生异常！" + ex.Message);
			}
		}

		public bool Toggle(int timeout = 2000)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			bool result = false;
			try
			{
				if (!GetStatus())
				{
					return ON(timeout);
				}
				if (GetStatus())
				{
					return OFF(timeout);
				}
			}
			catch (Exception ex)
			{
				throw new IOException("设置Output[:" + OutputName + "]时发生异常！" + ex.Message);
			}
			return result;
		}

		public bool GetStatus()
		{
			bool flag = false;
			try
			{
				return Card.GetDO(_Port, _Group);
			}
			catch (Exception ex)
			{
				throw new IOException("获取Output[:" + OutputName + "]状态时发生异常！" + ex.Message);
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
				throw new IOException("OutputModel WaitON Exception:" + ex.Message);
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
				throw new IOException("OutputModel WaitOFF Exception:" + ex.Message);
			}
			return result;
		}
	}
}
