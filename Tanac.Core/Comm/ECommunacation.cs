using DMSkin.Sockets;
using PCComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Tanac.Log4Net;

namespace Tanac.Comm
{
	[Serializable]
	public class ECommunacation
	{
		[NonSerialized]
		private AutoResetEvent m_RecStrSignal = new AutoResetEvent(false);

		private Queue<string> m_RecStrQueue = new Queue<string>();

		private bool m_IsStartRec = false;

		private EndSymbol m_EndSymbol;

		private bool m_IsReceivedByHex;

		private bool m_IsPlcDisconnect = false;

		private List<string> m_SocketIpPortList = new List<string>();
		[NonSerialized]
		private DMTcpServer m_DMTcpServer;
		[NonSerialized]
		private DMTcpClient m_DMTcpClient;
		[NonSerialized]
		private DMUdpClient m_DMUdpClient;
		[NonSerialized]
		private MySerialPort m_MySerialPort;

		private int m_ObjectConnectedCount = 0;

		public int TimeOut { get; set; }

		public string OrderStrAfterConnected { get; set; } = "";


		public EndSymbol EndSymbol
		{
			get
			{
				return m_EndSymbol;
			}
			set
			{
				m_EndSymbol = value;
				byte endSymbol = 0;
				switch (m_EndSymbol)
				{
					case EndSymbol.None:
						endSymbol = 0;
						break;
					case EndSymbol.Return:
						endSymbol = 13;
						break;
					case EndSymbol.NewLine:
						endSymbol = 10;
						break;
					case EndSymbol.Return_NewLine:
						endSymbol = 13;
						break;
				}
				switch (CommunicationModel)
				{
					case CommunicationModel.TcpClient:
						if (m_DMTcpClient != null)
						{
							m_DMTcpClient.EndSymbol = endSymbol;
						}
						else
						{
							Connect();
						}
						break;
					case CommunicationModel.TcpServer:
						if (m_DMTcpServer != null)
						{
							m_DMTcpServer.EndSymbol = endSymbol;
						}
						else
						{
							Connect();
						}
						break;
					case CommunicationModel.UDP:
						if (m_DMUdpClient != null)
						{
							m_DMUdpClient.EndSymbol = endSymbol;
						}
						else
						{
							Connect();
						}
						break;
					case CommunicationModel.COM:
						if (m_MySerialPort != null)
						{
							m_MySerialPort.EndSymbol = endSymbol;
						}
						else
						{
							Connect();
						}
						break;
				}
			}
		}

		public string Key { get; set; }

		public int Encode { get; set; }

		public CommunicationModel CommunicationModel { get; set; } = CommunicationModel.TcpServer;


		public bool IsConnected { get; set; }

		public bool IsSendByHex { get; set; }

		public bool IsReceivedByHex
		{
			get
			{
				return m_IsReceivedByHex;
			}
			set
			{
				if (m_DMTcpServer != null)
				{
					m_DMTcpServer.IsReceivedByHex = value;
				}
				if (m_DMTcpClient != null)
				{
					m_DMTcpClient.IsReceivedByHex = value;
				}
				if (m_DMUdpClient != null)
				{
					m_DMUdpClient.IsReceivedByHex = value;
				}
				if (m_MySerialPort != null)
				{
					m_MySerialPort.IsReceivedByHex = value;
				}
				m_IsReceivedByHex = value;
			}
		}

		public string RemoteIP { get; set; } = "127.0.0.1";


		public int RemotePort { get; set; } = 9000;


		public int LocalPort { get; set; } = 8000;


		public string PortName { get; set; } = "COM1";


		public string BaudRate { get; set; } = "9600";


		public string Parity { get; set; } = "None";


		public string DataBits { get; set; } = "8";


		public string StopBits { get; set; } = "One";


		public string Remarks { get; set; }

		public bool IsHasObjectConnected
		{
			get
			{
				return (m_ObjectConnectedCount > 0) ? true : false;
			}
			set
			{
				if (value)
				{
					if (m_ObjectConnectedCount < 0)
					{
						m_ObjectConnectedCount = 1;
					}
					else
					{
						m_ObjectConnectedCount++;
					}
					if (!string.IsNullOrEmpty(OrderStrAfterConnected))
					{
						SendStr(OrderStrAfterConnected);
					}
				}
				else
				{
					m_ObjectConnectedCount--;
				}
			}
		}

		public event ReceiveString ReceiveString;

		public DMTcpClient DMTcpClient()
		{
			return m_DMTcpClient;
		}

		public void SetObjectConnected(bool flag)
		{
			m_ObjectConnectedCount = (flag ? 1 : 0);
		}

		public ECommunacation()
		{
			ReceiveString += VmCommunacation_ReceiveString;
		}

		public void AddRecString(string recStr)
		{
			if (recStr != null && m_IsStartRec)
			{
				m_RecStrQueue.Enqueue(recStr);
				if (m_RecStrQueue.Count == 1)
				{
					m_RecStrSignal.Set();
				}
			}
		}

		private void VmCommunacation_ReceiveString(string str)
		{
			Log.Info("[" + Key + "]接收数据:" + str);
		}

		public bool Connect()
		{
			if (IsConnected)
			{
				return true;
			}
			if (m_RecStrSignal == null)
			{ 
				m_RecStrSignal = new AutoResetEvent(false); 
			}
			switch (CommunicationModel)
			{
				case CommunicationModel.TcpClient:
					if (m_DMTcpClient == null)
					{
						m_DMTcpClient = new DMTcpClient();
						m_DMTcpClient.OnReceviceByte += M_DMTcpClient_OnReceviceByte;
						m_DMTcpClient.OnStateInfo += M_DMTcpClient_OnStateInfo;
						m_DMTcpClient.OnErrorMsg += M_DMTcpClient_OnErrorMsg;
					}
					m_DMTcpClient.IsReceivedByHex = IsReceivedByHex;
					m_DMTcpClient.ServerIp = RemoteIP;
					m_DMTcpClient.ServerPort = RemotePort;
					m_DMTcpClient.StartConnection();
					IsConnected = true;
					break;
				case CommunicationModel.TcpServer:
					if (m_DMTcpServer == null)
					{
						m_DMTcpServer = new DMTcpServer();
						m_DMTcpServer.OnReceviceByte += M_DMTcpServer_OnReceviceByte;
						m_DMTcpServer.OnOnlineClient += M_DMTcpServer_OnOnlineClient;
						m_DMTcpServer.OnOfflineClient += M_DMTcpServer_OnOfflineClient;
					}
					m_DMTcpServer.IsReceivedByHex = IsReceivedByHex;
					m_DMTcpServer.ServerIp = "0.0.0.0";
					m_DMTcpServer.ServerPort = LocalPort;
					IsConnected = m_DMTcpServer.Start();
					break;
				case CommunicationModel.UDP:
					if (m_DMUdpClient == null)
					{
						m_DMUdpClient = new DMUdpClient();
						m_DMUdpClient.ReceiveByte += M_DMUdpClient_ReceiveByte;
					}
					m_DMUdpClient.IsReceivedByHex = IsReceivedByHex;
					m_DMUdpClient.RemoteIp = RemoteIP;
					m_DMUdpClient.RemotePort = RemotePort;
					m_DMUdpClient.LocalPort = LocalPort;
					IsConnected = m_DMUdpClient.Start();
					IsHasObjectConnected = IsConnected;
					break;
				case CommunicationModel.COM:
					if (m_MySerialPort == null)
					{
						m_MySerialPort = new MySerialPort();
						m_MySerialPort.OnReceiveString += M_MySerialPort_OnReceiveString;
					}
					m_MySerialPort.IsReceivedByHex = IsReceivedByHex;
					m_MySerialPort.PortName = PortName;
					m_MySerialPort.BaudRate = BaudRate;
					m_MySerialPort.DataBits = DataBits;
					m_MySerialPort.StopBits = StopBits;
					m_MySerialPort.Parity = Parity;
					IsConnected = m_MySerialPort.OpenPort();
					IsHasObjectConnected = IsConnected;
					break;
			}
			return IsConnected;
		}

		private void M_DMTcpClient_OnErrorMsg(string msg)
		{
			if (IsHasObjectConnected)
			{
				IsHasObjectConnected = false;
				Log.Warn(string.Format("与 服务器的连接断开  {0}:{1}", RemoteIP, RemotePort));
			}
		}

		private void M_DMTcpClient_OnStateInfo(string msg, SocketState state)
		{
			if (m_IsPlcDisconnect)
			{
				return;
			}
			switch (state)
			{
				case SocketState.Connecting:
					break;
				case SocketState.Connected:
					IsHasObjectConnected = true;
					Log.Tip(string.Format("已成功连接服务器  {0}:{1}", RemoteIP, RemotePort));
					break;
				case SocketState.Reconnection:
					break;
				case SocketState.Disconnect:
					if (IsHasObjectConnected)
					{
						IsHasObjectConnected = false;
						Log.Warn(string.Format("与服务器的连接断开  {0}:{1}", RemoteIP, RemotePort));
					}
					break;
				case SocketState.StartListening:
					break;
				case SocketState.StopListening:
					break;
				case SocketState.ClientOnline:
					break;
				case SocketState.ClientOnOff:
					break;
			}
		}

		private void M_DMTcpServer_OnOfflineClient(Socket temp)
		{
			IsHasObjectConnected = false;
			m_SocketIpPortList.Remove(temp.RemoteEndPoint.ToString());
			Log.Warn(temp.RemoteEndPoint.ToString() + "客户端已断开连接");
			if (IsHasObjectConnected)
			{
				Log.Warn(string.Format("当前连接的客户端数量为 {0} , \r\n{1}", m_ObjectConnectedCount, string.Join("\r\n", m_SocketIpPortList)));
			}
		}

		private void M_DMTcpServer_OnOnlineClient(Socket temp)
		{
			IsHasObjectConnected = true;
			m_SocketIpPortList.Add(temp.RemoteEndPoint.ToString());
			Log.Tip(temp.RemoteEndPoint.ToString() + " 客户端已经连接");
			if (m_ObjectConnectedCount > 1)
			{
				Log.Warn(string.Format("当前连接的客户端数量为 {0} , \r\n{1}", m_ObjectConnectedCount, string.Join("\r\n", m_SocketIpPortList)));
			}
		}

		public void DisConnect()
		{
			m_IsPlcDisconnect = false;
			IsConnected = false;
			StopRecStrSignal();
			switch (CommunicationModel)
			{
				case CommunicationModel.TcpClient:
					if (m_DMTcpClient != null)
					{
						m_DMTcpClient.StopConnection();
					}
					break;
				case CommunicationModel.TcpServer:
					if (m_DMTcpServer != null)
					{
						m_DMTcpServer.Stop();
					}
					break;
				case CommunicationModel.UDP:
					if (m_DMUdpClient != null)
					{
						m_DMUdpClient.Stop();
					}
					IsHasObjectConnected = false;
					break;
				case CommunicationModel.COM:
					if (m_MySerialPort != null)
					{
						m_MySerialPort.ClosePort();
					}
					IsHasObjectConnected = false;
					break;
			}
		}
		public void PlcDisConnect()
		{
			m_IsPlcDisconnect = true;
			StopRecStrSignal();
			switch (CommunicationModel)
			{
				case CommunicationModel.TcpClient:
					if (m_DMTcpClient != null)
					{
						m_DMTcpClient.StopConnection();
					}
					break;
				case CommunicationModel.COM:
					if (m_MySerialPort != null)
					{
						m_MySerialPort.ClosePort();
					}
					break;
			}
		}
		public bool SendStr(string str)
		{
			lock (this)
			{
				bool flag = false;
				if (!IsConnected)
				{
					return false;
				}
				switch (CommunicationModel)
				{
					case CommunicationModel.TcpClient:
						if (m_DMTcpClient != null)
						{
							DMTcpClient dMTcpClient = m_DMTcpClient;
							flag = ((dMTcpClient != null) ? new bool?(dMTcpClient.SendCommand(str, IsSendByHex)) : null).Value;
						}
						break;
					case CommunicationModel.TcpServer:
						if (m_DMTcpServer == null)
						{
							break;
						}
						try
						{
							foreach (Socket clientSocket in m_DMTcpServer.ClientSocketList)
							{
								if (clientSocket.Connected)
								{
									DMTcpServer dMTcpServer = m_DMTcpServer;
									if (dMTcpServer != null)
									{
										dMTcpServer.SendData(((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString(), ((IPEndPoint)clientSocket.RemoteEndPoint).Port, str, IsSendByHex);
									}
									flag = true;
								}
							}
						}
						catch (Exception)
						{
						}
						break;
					case CommunicationModel.UDP:
						if (m_DMUdpClient != null)
						{
							DMUdpClient dMUdpClient = m_DMUdpClient;
							if (dMUdpClient != null)
							{
								dMUdpClient.SendText(str, IsSendByHex);
							}
							flag = true;
						}
						break;
					case CommunicationModel.COM:
						if (m_MySerialPort != null)
						{
							MySerialPort mySerialPort = m_MySerialPort;
							flag = ((mySerialPort != null) ? new bool?(mySerialPort.WriteData(str, IsSendByHex)) : null).Value;
						}
						break;
				}
				if (!Key.Contains("Host服务端"))
				{
					if (flag)
					{
						if (IsSendByHex)
						{
							Log.Info("[" + Key + "]发送数据:" + str);
						}
						else
						{
							Log.Info("[" + Key + "]发送数据:" + str);
						}
					}
					else
					{
						Log.Info("[" + Key + "]发送数据失败");
					}
				}
				return flag;
			}
		}

		public void GetStr(out string pReturnStr)
		{
			string text = "";
			m_IsStartRec = true;//开始监听
			m_RecStrSignal.Reset();//需要加这一句,因为断开连接的时候会执行 m_RecStrSignal.Set()
			if (m_RecStrQueue.Count > 0)
			{
				text = m_RecStrQueue.Dequeue();
			}
			else
			{
				m_RecStrSignal.WaitOne(TimeOut);
				text = ((m_RecStrQueue.Count <= 0) ? "" : m_RecStrQueue.Dequeue());
			}
             pReturnStr=text.Trim();
		}

		public void StopRecStrSignal()
		{
			lock (this)
			{
				m_IsStartRec = false;
				m_RecStrQueue.Clear();
				m_RecStrSignal.Set();
			}
		}

		private void M_DMTcpServer_OnReceviceByte(Socket temp, byte[] dataBytes)
		{
			lock (this)
			{
				string @string = Encoding.Default.GetString(dataBytes);
				if (!IsReceivedByHex)
				{
					@string = @string.Trim().Trim(default(char));
				}
				else
				{
					@string = "";
					if (dataBytes.Length != 0)
					{
						foreach (byte b in dataBytes)
						{
							@string += string.Format("{0:X2} ", b);
						}
					}
				}
				if (!string.IsNullOrWhiteSpace(@string))
				{
					ReceiveString receiveString = this.ReceiveString;
					if (receiveString != null)
					{
						receiveString(@string);
					}
					AddRecString(@string);
				}
			}
		}

		private void M_DMTcpClient_OnReceviceByte(byte[] dataBytes)
		{
			lock (this)
			{
				string @string = Encoding.Default.GetString(dataBytes);
				if (!IsReceivedByHex)
				{
					@string = @string.Trim().Trim(default(char));
				}
				else
				{
					@string = "";
					if (dataBytes.Length != 0)
					{
						foreach (byte b in dataBytes)
						{
							@string += string.Format("{0:X2} ", b);
						}
					}
				}
				if (!string.IsNullOrWhiteSpace(@string))
				{
					ReceiveString receiveString = this.ReceiveString;
					if (receiveString != null)
					{
						receiveString(@string);
					}
					AddRecString(@string);
				}
			}
		}

		private void M_MySerialPort_OnReceiveString(string str)
		{
			lock (this)
			{
				if (!IsReceivedByHex)
				{
					str = str.Trim().Trim(default(char));
				}
				if (!string.IsNullOrWhiteSpace(str))
				{
					ReceiveString receiveString = this.ReceiveString;
					if (receiveString != null)
					{
						receiveString(str);
					}
					AddRecString(str);
				}
			}
		}

		private void M_DMUdpClient_ReceiveByte(ReceiveDataEventArgs e)
		{
			lock (this)
			{
				string @string = Encoding.Default.GetString(e.Buffer);
				if (!IsReceivedByHex)
				{
					@string = @string.Trim().Trim(default(char));
				}
				else
				{
					@string = "";
					if (e.Buffer.Length != 0)
					{
						byte[] buffer = e.Buffer;
						foreach (byte b in buffer)
						{
							@string += string.Format("{0:X2} ", b);
						}
					}
				}
				if (!string.IsNullOrWhiteSpace(@string))
				{
					ReceiveString receiveString = this.ReceiveString;
					if (receiveString != null)
					{
						receiveString(@string);
					}
					AddRecString(@string);
				}
			}
		}

		public Socket GetSocket()
		{
			switch (CommunicationModel)
			{
				case CommunicationModel.TcpClient:
					if (m_DMTcpClient != null)
					{
						TcpClient tcpclient = m_DMTcpClient.Tcpclient;
						return (tcpclient != null) ? tcpclient.Client : null;
					}
					break;
				case CommunicationModel.TcpServer:
					Log.Warn(" TcpServer暂不支持获取 socket");
					break;
				case CommunicationModel.UDP:
					Log.Warn(" UDP暂不支持获取 socket");
					break;
				case CommunicationModel.COM:
					Log.Warn(" COM不支持获取 socket");
					break;
			}
			return null;
		}

		public string GetInfoStr()
		{
			string result = "";
			switch (CommunicationModel)
			{
				case CommunicationModel.TcpClient:
					result = string.Format("远程主机: {0}:{1}", RemoteIP, RemotePort);
					break;
				case CommunicationModel.TcpServer:
					result = string.Format("本地主机: 0.0.0.0:{0}\r\n客户端连接数量: {1}\r\n客户端信息:\r\n{2}", LocalPort, m_ObjectConnectedCount, string.Join("\r\n", m_SocketIpPortList));
					break;
				case CommunicationModel.UDP:
					result = string.Format("本地主机: 0.0.0.0:{0}\r\n远程主机: {1}:{2}", LocalPort, RemoteIP, RemotePort);
					break;
				case CommunicationModel.COM:
					result = "串口号: " + PortName + "\r\n波特率: " + BaudRate + "\r\n校验位: " + Parity + "\r\n数据位: " + DataBits + "\r\n停止位: " + StopBits;
					break;
			}
			return result;
		}

		public void SetSerialPortDataReceivedFunction(SerialPortDataReceivedFunction function)
		{
			if (CommunicationModel == CommunicationModel.COM)
			{
				m_MySerialPort.DataReceivedFunction = function;
			}
		}
	}
}
