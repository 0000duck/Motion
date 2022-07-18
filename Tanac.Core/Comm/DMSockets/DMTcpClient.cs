using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Tanac.Comm.Tool;
using Tanac.Log4Net;

namespace DMSkin.Sockets
{
	public class DMTcpClient : Component
	{
		public delegate void ReceviceByteEventHandler(byte[] date);

		public delegate void ErrorMsgEventHandler(string msg);

		public delegate void StateInfoEventHandler(string msg, SocketState state);

		private string _ServerIp;

		private int _ServerPort;

		private TcpClient _Tcpclient;

		private Thread _Tcpthread;

		private bool _IsStartTcpthreading;

		private bool _Isclosed;

		private int _ReConnectionTime;

		private string _Receivestr;

		private int _ReConectedCount;

		private ReceviceByteEventHandler _ReceviceByteEventHandler;

		private ErrorMsgEventHandler _ErrorMsgEventHandler;

		private StateInfoEventHandler _StateInfoEventHandler;

		private IContainer _Container;

		public bool IsReceivedByHex { get; set; } = false;


		public byte EndSymbol { get; set; }

		[Description("服务端IP")]
		[Category("TcpClient属性")]
		public string ServerIp
		{
			get
			{
				return _ServerIp;
			}
			set
			{
				_ServerIp = value;
			}
		}

		[Description("服务端监听端口")]
		[Category("TcpClient属性")]
		public int ServerPort
		{
			get
			{
				return _ServerPort;
			}
			set
			{
				_ServerPort = value;
			}
		}

		[Description("TcpClient操作类")]
		[Browsable(false)]
		[Category("TcpClient隐藏属性")]
		public TcpClient Tcpclient
		{
			get
			{
				return _Tcpclient;
			}
			set
			{
				_Tcpclient = value;
			}
		}

		[Description("TcpClient连接服务端线程")]
		[Category("TcpClient隐藏属性")]
		[Browsable(false)]
		public Thread Tcpthread
		{
			get
			{
				return _Tcpthread;
			}
			set
			{
				_Tcpthread = value;
			}
		}

		[Browsable(false)]
		[Category("TcpClient隐藏属性")]
		[Description("是否启动Tcp连接线程")]
		public bool IsStartTcpthreading
		{
			get
			{
				return _IsStartTcpthreading;
			}
			set
			{
				_IsStartTcpthreading = value;
			}
		}

		[Description("连接是否关闭（用来断开重连）")]
		[Category("TcpClient属性")]
		public bool Isclosed
		{
			get
			{
				return _Isclosed;
			}
			set
			{
				_Isclosed = value;
			}
		}

		[Category("TcpClient属性")]
		[Description("设置断开重连时间间隔单位（毫秒）（默认3000毫秒）")]
		public int ReConnectionTime
		{
			get
			{
				return _ReConnectionTime;
			}
			set
			{
				_ReConnectionTime = value;
			}
		}

		[Description("接收Socket数据包 缓存字符串")]
		[Category("TcpClient隐藏属性")]
		[Browsable(false)]
		public string Receivestr
		{
			get
			{
				return _Receivestr;
			}
			set
			{
				_Receivestr = value;
			}
		}

		[Description("重连次数")]
		[Category("TcpClient隐藏属性")]
		[Browsable(false)]
		public int ReConectedCount
		{
			get
			{
				return _ReConectedCount;
			}
			set
			{
				_ReConectedCount = value;
			}
		}

		[Category("TcpClient事件")]
		[Description("接收Byte数据事件")]
		public event ReceviceByteEventHandler OnReceviceByte
		{
			add
			{
				ReceviceByteEventHandler receviceByteEventHandler = _ReceviceByteEventHandler;
				ReceviceByteEventHandler receviceByteEventHandler2;
				do
				{
					receviceByteEventHandler2 = receviceByteEventHandler;
					ReceviceByteEventHandler value2 = (ReceviceByteEventHandler)Delegate.Combine(receviceByteEventHandler2, value);
					receviceByteEventHandler = Interlocked.CompareExchange(ref _ReceviceByteEventHandler, value2, receviceByteEventHandler2);
				}
				while (receviceByteEventHandler != receviceByteEventHandler2);
			}
			remove
			{
				ReceviceByteEventHandler receviceByteEventHandler = _ReceviceByteEventHandler;
				ReceviceByteEventHandler receviceByteEventHandler2;
				do
				{
					receviceByteEventHandler2 = receviceByteEventHandler;
					ReceviceByteEventHandler value2 = (ReceviceByteEventHandler)Delegate.Remove(receviceByteEventHandler2, value);
					receviceByteEventHandler = Interlocked.CompareExchange(ref _ReceviceByteEventHandler, value2, receviceByteEventHandler2);
				}
				while (receviceByteEventHandler != receviceByteEventHandler2);
			}
		}

		[Category("TcpClient事件")]
		[Description("返回错误消息事件")]
		public event ErrorMsgEventHandler OnErrorMsg
		{
			add
			{
				ErrorMsgEventHandler errorMsgEventHandler = _ErrorMsgEventHandler;
				ErrorMsgEventHandler errorMsgEventHandler2;
				do
				{
					errorMsgEventHandler2 = errorMsgEventHandler;
					ErrorMsgEventHandler value2 = (ErrorMsgEventHandler)Delegate.Combine(errorMsgEventHandler2, value);
					errorMsgEventHandler = Interlocked.CompareExchange(ref _ErrorMsgEventHandler, value2, errorMsgEventHandler2);
				}
				while (errorMsgEventHandler != errorMsgEventHandler2);
			}
			remove
			{
				ErrorMsgEventHandler errorMsgEventHandler = _ErrorMsgEventHandler;
				ErrorMsgEventHandler errorMsgEventHandler2;
				do
				{
					errorMsgEventHandler2 = errorMsgEventHandler;
					ErrorMsgEventHandler value2 = (ErrorMsgEventHandler)Delegate.Remove(errorMsgEventHandler2, value);
					errorMsgEventHandler = Interlocked.CompareExchange(ref _ErrorMsgEventHandler, value2, errorMsgEventHandler2);
				}
				while (errorMsgEventHandler != errorMsgEventHandler2);
			}
		}

		[Description("连接状态改变时返回连接状态事件")]
		[Category("TcpClient事件")]
		public event StateInfoEventHandler OnStateInfo
		{
			add
			{
				StateInfoEventHandler stateInfoEventHandler = _StateInfoEventHandler;
				StateInfoEventHandler stateInfoEventHandler2;
				do
				{
					stateInfoEventHandler2 = stateInfoEventHandler;
					StateInfoEventHandler value2 = (StateInfoEventHandler)Delegate.Combine(stateInfoEventHandler2, value);
					stateInfoEventHandler = Interlocked.CompareExchange(ref _StateInfoEventHandler, value2, stateInfoEventHandler2);
				}
				while (stateInfoEventHandler != stateInfoEventHandler2);
			}
			remove
			{
				StateInfoEventHandler stateInfoEventHandler = _StateInfoEventHandler;
				StateInfoEventHandler stateInfoEventHandler2;
				do
				{
					stateInfoEventHandler2 = stateInfoEventHandler;
					StateInfoEventHandler value2 = (StateInfoEventHandler)Delegate.Remove(stateInfoEventHandler2, value);
					stateInfoEventHandler = Interlocked.CompareExchange(ref _StateInfoEventHandler, value2, stateInfoEventHandler2);
				}
				while (stateInfoEventHandler != stateInfoEventHandler2);
			}
		}

		public DMTcpClient()
		{
			_ReConnectionTime = 3000;
			CreateContainer();
		}

		public DMTcpClient(IContainer container)
		{
			_ReConnectionTime = 3000;
			container.Add(this);
			CreateContainer();
		}

		public void StartConnection()
		{
			try
			{
				CreateConnection();
			}
			catch (Exception ex)
			{
				OnTcpClientErrorMsgEnterHead("错误信息：" + ex.Message);
			}
		}

		private void CreateConnection(bool delayFlag = false)
		{
			if (!Isclosed)
			{
				if (delayFlag)
				{
					Thread.Sleep(ReConnectionTime);
				}
				Isclosed = true;
				Tcpclient = new TcpClient();
				Tcpthread = new Thread(MinitorConnection);
				IsStartTcpthreading = true;
				Tcpthread.Start();
			}
		}

		public void StopConnection()
		{
			IsStartTcpthreading = false;
			Isclosed = false;
			if (Tcpclient != null)
			{
				Tcpclient.Close();
				Tcpclient = null;
			}
			if (Tcpthread != null)
			{
				Tcpthread.Interrupt();
				Tcpthread.Abort();
			}
			OnTcpClientStateInfoEnterHead("断开连接", SocketState.Disconnect);
		}

		private void MinitorConnection()
		{
			byte[] array = new byte[5024];
			try
			{
				while (IsStartTcpthreading)
				{
					if (!Tcpclient.Connected)
					{
						try
						{
							if (ReConectedCount != 0)
							{
								OnTcpClientStateInfoEnterHead($"正在第{ReConectedCount}次重新连接服务器... ...", SocketState.Reconnection);
							}
							else
							{
								OnTcpClientStateInfoEnterHead("正在连接服务器... ...", SocketState.Connecting);
							}
							Tcpclient.Connect(IPAddress.Parse(ServerIp), ServerPort);
							OnTcpClientStateInfoEnterHead("已连接服务器", SocketState.Connected);
						}
						catch
						{
							ReConectedCount++;
							Isclosed = false;
							IsStartTcpthreading = false;
							continue;
						}
					}
					int num = 0;
					if (EndSymbol != 0)
					{
						List<byte> list = new List<byte>();
						byte[] array2;
						do
						{
							array2 = new byte[1];
							int num2 = Tcpclient.Client.Receive(array2, 0, 1, SocketFlags.None);
							if (num2 == 1)
							{
								list.Add(array2[0]);
								num = list.Count;
							}
							else
							{
								num = 0;
							}
						}
						while (array2[0] != EndSymbol && num != 0);
						array = list.ToArray();
					}
					else
					{
						num = Tcpclient.Client.Receive(array);
					}
					if (num == 0)
					{
						OnTcpClientStateInfoEnterHead("与服务器断开连接... ...", SocketState.Disconnect);
						Isclosed = false;
						ReConectedCount = 1;
						IsStartTcpthreading = false;
					}
					else if (!IsReceivedByHex)
					{
						Receivestr = Encoding.Default.GetString(array, 0, num);
						if (Receivestr.Trim() != "")
						{
							byte[] array3 = new byte[num];
							Array.Copy(array, 0, array3, 0, num);
							OnTcpClientReceviceByte(array3);
						}
					}
					else
					{
						byte[] array4 = new byte[num];
						Array.Copy(array, 0, array4, 0, num);
						OnTcpClientReceviceByte(array4);
					}
				}
				CreateConnection(delayFlag: true);
			}
			catch (Exception ex)
			{
				if (Tcpclient != null)
				{
					OnTcpClientErrorMsgEnterHead("错误信息：" + ex.Message);
					if (!ex.Message.Contains("远程主机强迫关闭了一个现有的连接"))
					{
						Log.Error($"{ServerIp}:{ServerPort} 发生异常, 异常信息为: {ex.Message} ");
					}
					Isclosed = false;
					ReConectedCount = 1;
					CreateConnection(delayFlag: true);
				}
			}
		}

		public bool SendCommand(string cmdstr, bool isSendByHex)
		{
			try
			{
				byte[] buffer = ((!isSendByHex) ? Encoding.Default.GetBytes(cmdstr) : HexTool.HexToByte(cmdstr));
				List<System.Net.Sockets.Socket> list = new List<System.Net.Sockets.Socket>();
				list.Add(Tcpclient.Client);
				System.Net.Sockets.Socket.Select(null, list, null, 1000);
				foreach (System.Net.Sockets.Socket item in list)
				{
					item.Send(buffer);
				}
				return true;
			}
			catch (Exception ex)
			{
				OnTcpClientErrorMsgEnterHead(ex.Message);
				return false;
			}
		}

		public void SendFile(string filename)
		{
			Tcpclient.Client.BeginSendFile(filename, SendFile, Tcpclient);
		}

		private void SendFile(IAsyncResult input)
		{
			try
			{
				TcpClient tcpClient = (TcpClient)input.AsyncState;
				tcpClient.Client.EndSendFile(input);
			}
			catch (SocketException)
			{
			}
		}

		public void SendCommand(byte[] byteMsg)
		{
			try
			{
				Tcpclient.Client.Send(byteMsg);
			}
			catch (Exception ex)
			{
				OnTcpClientErrorMsgEnterHead("错误信息：" + ex.Message);
			}
		}

		protected virtual void OnTcpClientReceviceByte(byte[] date)
		{
			if (_ReceviceByteEventHandler != null)
			{
				_ReceviceByteEventHandler(date);
			}
		}

		protected virtual void OnTcpClientErrorMsgEnterHead(string msg)
		{
			if (_ErrorMsgEventHandler != null)
			{
				_ErrorMsgEventHandler(msg);
			}
		}

		protected virtual void OnTcpClientStateInfoEnterHead(string msg, SocketState state)
		{
			if (_StateInfoEventHandler != null)
			{
				_StateInfoEventHandler(msg, state);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _Container != null)
			{
				_Container.Dispose();
			}
			base.Dispose(disposing);
		}

		private void CreateContainer()
		{
			_Container = new Container();
		}
	}
}
