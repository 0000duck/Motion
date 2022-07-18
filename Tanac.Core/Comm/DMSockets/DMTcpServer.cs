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
	public class DMTcpServer : Component
	{
		public delegate void ReceviceByteEventHandler(Socket temp, byte[] dataBytes);

		public delegate void ErrorMsgEventHandler(string msg);

		public delegate void ReturnClientCountEventHandler(int count);

		public delegate void StateInfoEventHandler(string msg, SocketState state);

		public delegate void AddClientEventHandler(Socket temp);

		public delegate void OfflineClientEventHandler(Socket temp);

		public Socket ServerSocket;

		public Thread StartSockst;

		private string _ServerIp;

		private int _ServerPort;

		public bool IsStartListening;

		private List<Socket> m_ClientSocketList;

		private IContainer _IContainer;

		private ReceviceByteEventHandler _ReceviceByteEventHandler;

		private ErrorMsgEventHandler _ErrorMsgEventHandler;

		private ReturnClientCountEventHandler _ReturnClientCountEventHandler;

		private StateInfoEventHandler _StateInfoEventHandler;

		private AddClientEventHandler _AddClientEventHandler1;

		private AddClientEventHandler _AddClientEventHandler2;

		public bool IsReceivedByHex { get; set; } = false;


		public byte EndSymbol { get; set; }

		[Description("本机监听IP,默认是本地IP")]
		[Category("TCP服务端")]
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

		[Category("TCP服务端")]
		[Description("本机监听端口,默认是8000")]
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

		public List<Socket> ClientSocketList
		{
			get
			{
				return m_ClientSocketList;
			}
			set
			{
				m_ClientSocketList = value;
			}
		}

		[Description("接收原始Byte数组数据事件")]
		[Category("TcpServer事件")]
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

		[Category("TcpServer事件")]
		[Description("错误消息")]
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

		[Description("用户上线下线时更新客户端在线数量事件")]
		[Category("TcpServer事件")]
		public event ReturnClientCountEventHandler OnReturnClientCount
		{
			add
			{
				ReturnClientCountEventHandler returnClientCountEventHandler = _ReturnClientCountEventHandler;
				ReturnClientCountEventHandler returnClientCountEventHandler2;
				do
				{
					returnClientCountEventHandler2 = returnClientCountEventHandler;
					ReturnClientCountEventHandler value2 = (ReturnClientCountEventHandler)Delegate.Combine(returnClientCountEventHandler2, value);
					returnClientCountEventHandler = Interlocked.CompareExchange(ref _ReturnClientCountEventHandler, value2, returnClientCountEventHandler2);
				}
				while (returnClientCountEventHandler != returnClientCountEventHandler2);
			}
			remove
			{
				ReturnClientCountEventHandler returnClientCountEventHandler = _ReturnClientCountEventHandler;
				ReturnClientCountEventHandler returnClientCountEventHandler2;
				do
				{
					returnClientCountEventHandler2 = returnClientCountEventHandler;
					ReturnClientCountEventHandler value2 = (ReturnClientCountEventHandler)Delegate.Remove(returnClientCountEventHandler2, value);
					returnClientCountEventHandler = Interlocked.CompareExchange(ref _ReturnClientCountEventHandler, value2, returnClientCountEventHandler2);
				}
				while (returnClientCountEventHandler != returnClientCountEventHandler2);
			}
		}

		[Description("监听状态改变时返回监听状态事件")]
		[Category("TcpServer事件")]
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

		[Category("TcpServer事件")]
		[Description("新客户端上线时返回客户端事件")]
		public event AddClientEventHandler OnOnlineClient
		{
			add
			{
				AddClientEventHandler addClientEventHandler = _AddClientEventHandler1;
				AddClientEventHandler addClientEventHandler2;
				do
				{
					addClientEventHandler2 = addClientEventHandler;
					AddClientEventHandler value2 = (AddClientEventHandler)Delegate.Combine(addClientEventHandler2, value);
					addClientEventHandler = Interlocked.CompareExchange(ref _AddClientEventHandler1, value2, addClientEventHandler2);
				}
				while (addClientEventHandler != addClientEventHandler2);
			}
			remove
			{
				AddClientEventHandler addClientEventHandler = _AddClientEventHandler1;
				AddClientEventHandler addClientEventHandler2;
				do
				{
					addClientEventHandler2 = addClientEventHandler;
					AddClientEventHandler value2 = (AddClientEventHandler)Delegate.Remove(addClientEventHandler2, value);
					addClientEventHandler = Interlocked.CompareExchange(ref _AddClientEventHandler1, value2, addClientEventHandler2);
				}
				while (addClientEventHandler != addClientEventHandler2);
			}
		}

		[Description("客户端下线时返回客户端事件")]
		[Category("TcpServer事件")]
		public event AddClientEventHandler OnOfflineClient
		{
			add
			{
				AddClientEventHandler addClientEventHandler = _AddClientEventHandler2;
				AddClientEventHandler addClientEventHandler2;
				do
				{
					addClientEventHandler2 = addClientEventHandler;
					AddClientEventHandler value2 = (AddClientEventHandler)Delegate.Combine(addClientEventHandler2, value);
					addClientEventHandler = Interlocked.CompareExchange(ref _AddClientEventHandler2, value2, addClientEventHandler2);
				}
				while (addClientEventHandler != addClientEventHandler2);
			}
			remove
			{
				AddClientEventHandler addClientEventHandler = _AddClientEventHandler2;
				AddClientEventHandler addClientEventHandler2;
				do
				{
					addClientEventHandler2 = addClientEventHandler;
					AddClientEventHandler value2 = (AddClientEventHandler)Delegate.Remove(addClientEventHandler2, value);
					addClientEventHandler = Interlocked.CompareExchange(ref _AddClientEventHandler2, value2, addClientEventHandler2);
				}
				while (addClientEventHandler != addClientEventHandler2);
			}
		}

		public DMTcpServer()
		{
			_ServerIp = "0.0.0.0";
			_ServerPort = 8000;
			ClientSocketList = new List<Socket>();
			CreateIContainer();
			ClientSocketList = new List<Socket>();
			ClientSocketList.Clear();
		}

		public DMTcpServer(IContainer container)
		{
			_ServerIp = "0.0.0.0";
			_ServerPort = 8000;
			ClientSocketList = new List<Socket>();
			container.Add(this);
			CreateIContainer();
			ClientSocketList = new List<Socket>();
			ClientSocketList.Clear();
		}

		public bool Start()
		{
			try
			{
				if (!IsStartListening)
				{
					ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					ServerSocket.Bind(new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort));
					ServerSocket.Listen(10000);
					IsStartListening = true;
					OnTcpServerStateInfoEnterHead($"服务端Ip:{ServerIp},端口:{ServerPort}已启动监听", SocketState.StartListening);
					StartSockst = new Thread(StartSocketListening);
					StartSockst.IsBackground = true;
					StartSockst.Start();
					return true;
				}
				return true;
			}
			catch (SocketException ex)
			{
				Log.Error(ex.Message);
				OnTcpServerErrorMsgEnterHead(ex.Message);
				return false;
			}
		}

		public void Stop()
		{
			try
			{
				IsStartListening = false;
				if (StartSockst != null)
				{
					StartSockst.Interrupt();
					StartSockst.Abort();
				}
				if (ServerSocket != null)
				{
					ServerSocket.Close();
				}
				OnTcpServerStateInfoEnterHead($"服务端Ip:{ServerIp},端口:{ServerPort}已停止监听", SocketState.StopListening);
				for (int i = 0; i < ClientSocketList.Count; i++)
				{
					OnTcpServerOfflineClientEnterHead(ClientSocketList[i]);
					ClientSocketList[i].Shutdown(SocketShutdown.Both);
				}
			}
			catch (SocketException)
			{
			}
		}

		public void StartSocketListening()
		{
			try
			{
				while (IsStartListening)
				{
					Socket socket = ServerSocket.Accept();
					try
					{
						Thread.Sleep(10);
						lock (this)
						{
							ClientSocketList.Add(socket);
						}
						string text = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
						string text2 = ((IPEndPoint)socket.RemoteEndPoint).Port.ToString();
						OnTcpServerStateInfoEnterHead("<" + text + "：" + text2 + ">---上线", SocketState.ClientOnline);
						OnTcpServerOnlineClientEnterHead(socket);
						OnTcpServerReturnClientCountEnterHead(ClientSocketList.Count);
						ThreadPool.QueueUserWorkItem(ClientSocketCallBack, socket);
					}
					catch (Exception)
					{
						socket.Shutdown(SocketShutdown.Both);
						OnTcpServerOfflineClientEnterHead(socket);
						ClientSocketList.Remove(socket);
					}
				}
			}
			catch (Exception ex2)
			{
				OnTcpServerErrorMsgEnterHead(ex2.Message);
			}
		}

		public void ClientSocketCallBack(object obj)
		{
			Socket socket = (Socket)obj;
			while (IsStartListening)
			{
				try
				{
					byte[] array = new byte[1024];
					int num = 0;
					if (EndSymbol != 0)
					{
						List<byte> list = new List<byte>();
						byte[] array2;
						do
						{
							array2 = new byte[1];
							int num2 = socket.Receive(array2, 0, 1, SocketFlags.None);
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
						num = socket.Receive(array);
					}
					if (num > 0)
					{
						byte[] array3 = new byte[num];
						Array.Copy(array, 0, array3, 0, num);
						OnTcpServerReceviceByte(socket, array3);
					}
					else if (num == 0)
					{
						SocketExitMethod(socket);
						return;
					}
				}
				catch (Exception ex)
				{
					string text = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
					string text2 = ((IPEndPoint)socket.RemoteEndPoint).Port.ToString();
					if (!ex.Message.Contains("远程主机强迫关闭了一个现有的连接"))
					{
						Log.Error(" " + text + ":" + text2 + " 异常信息为:" + ex.Message);
					}
					SocketExitMethod(socket);
					return;
				}
			}
		}

		private void SocketExitMethod(Socket socket)
		{
			try
			{
				lock (this)
				{
					ClientSocketList.Remove(socket);
				}
				string text = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
				string text2 = ((IPEndPoint)socket.RemoteEndPoint).Port.ToString();
				OnTcpServerStateInfoEnterHead("<" + text + "：" + text2 + ">---下线", SocketState.ClientOnOff);
				OnTcpServerOfflineClientEnterHead(socket);
				OnTcpServerReturnClientCountEnterHead(ClientSocketList.Count);
				socket.Shutdown(SocketShutdown.Both);
			}
			catch
			{
			}
		}

		public void SendData(string ip, int port, string strData, bool isSendByHex)
		{
			Socket socket = ResoultSocket(ip, port);
			try
			{
				if (socket == null)
				{
					return;
				}
				byte[] array = ((!isSendByHex) ? Encoding.Default.GetBytes(strData) : HexTool.HexToByte(strData));
				List<Socket> list = new List<Socket>();
				list.Add(socket);
				Socket.Select(null, list, null, 1000);
				foreach (Socket item in list)
				{
					item.Send(array, array.Length, SocketFlags.None);
				}
			}
			catch (SocketException ex)
			{
				socket?.Shutdown(SocketShutdown.Both);
				OnTcpServerErrorMsgEnterHead(ex.Message);
			}
		}

		public void SendData(string ip, int port, byte[] dataBytes)
		{
			Socket socket = ResoultSocket(ip, port);
			try
			{
				socket?.Send(dataBytes);
			}
			catch (SocketException ex)
			{
				socket?.Shutdown(SocketShutdown.Both);
				OnTcpServerErrorMsgEnterHead(ex.Message);
			}
		}

		public Socket ResoultSocket(string ip, int port)
		{
			Socket result = null;
			try
			{
				foreach (Socket clientSocket in ClientSocketList)
				{
					if (((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString().Equals(ip) && port == ((IPEndPoint)clientSocket.RemoteEndPoint).Port)
					{
						result = clientSocket;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				OnTcpServerErrorMsgEnterHead(ex.Message);
			}
			return result;
		}

		protected virtual void OnTcpServerReceviceByte(Socket temp, byte[] dataBytes)
		{
			if (_ReceviceByteEventHandler != null)
			{
				_ReceviceByteEventHandler(temp, dataBytes);
			}
		}

		protected virtual void OnTcpServerErrorMsgEnterHead(string msg)
		{
			if (_ErrorMsgEventHandler != null)
			{
				_ErrorMsgEventHandler(msg);
			}
		}

		protected virtual void OnTcpServerReturnClientCountEnterHead(int count)
		{
			if (_ReturnClientCountEventHandler != null)
			{
				_ReturnClientCountEventHandler(count);
			}
		}

		protected virtual void OnTcpServerStateInfoEnterHead(string msg, SocketState state)
		{
			if (_StateInfoEventHandler != null)
			{
				_StateInfoEventHandler(msg, state);
			}
		}

		protected virtual void OnTcpServerOnlineClientEnterHead(Socket temp)
		{
			if (_AddClientEventHandler1 != null)
			{
				_AddClientEventHandler1(temp);
			}
		}

		protected virtual void OnTcpServerOfflineClientEnterHead(Socket temp)
		{
			if (_AddClientEventHandler2 != null)
			{
				_AddClientEventHandler2(temp);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _IContainer != null)
			{
				_IContainer.Dispose();
			}
			base.Dispose(disposing);
		}

		private void CreateIContainer()
		{
			_IContainer = new Container();
		}
	}
}
