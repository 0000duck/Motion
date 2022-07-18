using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading;
using Tanac.Comm.Tool;

namespace DMSkin.Sockets
{
	public class DMUdpClient : Component
	{
		public delegate void ReceiveByteEventHandler(ReceiveDataEventArgs e);

		public delegate void ReceiveTextMsgEventHandler(MsgTypeCell msgTypeCell);

		private UdpLibrary _UdpLibrary;

		private string _RemoteIp;

		private int _RemotePort;

		private int _LocalPort;

		private ReceiveByteEventHandler _ReceiveByteEventHandler;

		private ReceiveTextMsgEventHandler _ReceiveTextMsgEventHandler;

		private IContainer _IContainer;

		public bool IsReceivedByHex { get; set; } = false;


		public byte EndSymbol { get; set; }

		[Description("UDP客户端基类")]
		[Category("UDP客户端属性")]
		public UdpLibrary UdpLibrary
		{
			get
			{
				if (_UdpLibrary == null)
				{
					_UdpLibrary = new UdpLibrary(_LocalPort);
					_UdpLibrary.ReceiveData += _UdpLibrary_ReceiveData;
				}
				return _UdpLibrary;
			}
		}

		[Description("远程监听IP")]
		[Category("UDP客户端属性")]
		public string RemoteIp
		{
			get
			{
				return _RemoteIp;
			}
			set
			{
				_RemoteIp = value;
			}
		}

		[Category("UDP客户端属性")]
		[Description("远程监听端口")]
		public int RemotePort
		{
			get
			{
				return _RemotePort;
			}
			set
			{
				_RemotePort = value;
			}
		}

		[Description("本地监听IP")]
		[Category("UDP客户端属性")]
		public int LocalPort
		{
			get
			{
				return _LocalPort;
			}
			set
			{
				_LocalPort = value;
			}
		}

		[Category("UDP客户端属性")]
		[Description("远程主机网络端点")]
		public IPEndPoint RemoteEp => new IPEndPoint(IPAddress.Parse(_RemoteIp), _RemotePort);

		[Description("接收文本数据事件")]
		public event ReceiveByteEventHandler ReceiveByte
		{
			add
			{
				ReceiveByteEventHandler receiveByteEventHandler = _ReceiveByteEventHandler;
				ReceiveByteEventHandler receiveByteEventHandler2;
				do
				{
					receiveByteEventHandler2 = receiveByteEventHandler;
					ReceiveByteEventHandler value2 = (ReceiveByteEventHandler)Delegate.Combine(receiveByteEventHandler2, value);
					receiveByteEventHandler = Interlocked.CompareExchange(ref _ReceiveByteEventHandler, value2, receiveByteEventHandler2);
				}
				while (receiveByteEventHandler != receiveByteEventHandler2);
			}
			remove
			{
				ReceiveByteEventHandler receiveByteEventHandler = _ReceiveByteEventHandler;
				ReceiveByteEventHandler receiveByteEventHandler2;
				do
				{
					receiveByteEventHandler2 = receiveByteEventHandler;
					ReceiveByteEventHandler value2 = (ReceiveByteEventHandler)Delegate.Remove(receiveByteEventHandler2, value);
					receiveByteEventHandler = Interlocked.CompareExchange(ref _ReceiveByteEventHandler, value2, receiveByteEventHandler2);
				}
				while (receiveByteEventHandler != receiveByteEventHandler2);
			}
		}

		[Description("接收文本数据事件")]
		public event ReceiveTextMsgEventHandler ReceiveTextMsg
		{
			add
			{
				ReceiveTextMsgEventHandler receiveTextMsgEventHandler = _ReceiveTextMsgEventHandler;
				ReceiveTextMsgEventHandler receiveTextMsgEventHandler2;
				do
				{
					receiveTextMsgEventHandler2 = receiveTextMsgEventHandler;
					ReceiveTextMsgEventHandler value2 = (ReceiveTextMsgEventHandler)Delegate.Combine(receiveTextMsgEventHandler2, value);
					receiveTextMsgEventHandler = Interlocked.CompareExchange(ref _ReceiveTextMsgEventHandler, value2, receiveTextMsgEventHandler2);
				}
				while (receiveTextMsgEventHandler != receiveTextMsgEventHandler2);
			}
			remove
			{
				ReceiveTextMsgEventHandler receiveTextMsgEventHandler = _ReceiveTextMsgEventHandler;
				ReceiveTextMsgEventHandler receiveTextMsgEventHandler2;
				do
				{
					receiveTextMsgEventHandler2 = receiveTextMsgEventHandler;
					ReceiveTextMsgEventHandler value2 = (ReceiveTextMsgEventHandler)Delegate.Remove(receiveTextMsgEventHandler2, value);
					receiveTextMsgEventHandler = Interlocked.CompareExchange(ref _ReceiveTextMsgEventHandler, value2, receiveTextMsgEventHandler2);
				}
				while (receiveTextMsgEventHandler != receiveTextMsgEventHandler2);
			}
		}

		public DMUdpClient()
		{
			_RemoteIp = "127.0.0.1";
			_RemotePort = 8900;
			_LocalPort = 8899;
			CreateIContainer();
		}

		public DMUdpClient(IContainer container)
		{
			_RemoteIp = "127.0.0.1";
			_RemotePort = 8900;
			_LocalPort = 8899;
			container.Add(this);
			CreateIContainer();
		}

		public bool Start()
		{
			UdpLibrary.Port = LocalPort;
			return UdpLibrary.Start();
		}

		public void Stop()
		{
			UdpLibrary.Stop();
		}

		private void _UdpLibrary_ReceiveData(object sender, ReceiveDataEventArgs e)
		{
			OnReceiveByte(e);
		}

		public void Send(int messageId, object data)
		{
			Send(messageId, data, RemoteEp);
		}

		public void SendText(string strmsg, bool isSendByHex)
		{
			if (isSendByHex)
			{
				byte[] buffer = HexTool.HexToByte(strmsg);
				UdpLibrary.Send(buffer, RemoteEp);
			}
			else
			{
				byte[] bytes = Encoding.Default.GetBytes(strmsg);
				UdpLibrary.Send(bytes, RemoteEp);
			}
		}

		public void SendText(byte[] strmsgBytes)
		{
			UdpLibrary.Send(strmsgBytes, RemoteEp);
		}

		public void Send(int messageId, object data, IPEndPoint remoteIp)
		{
			MsgCell cell = new MsgCell(messageId, data);
			UdpLibrary.Send(cell, remoteIp);
		}

		protected virtual void OnReceiveByte(ReceiveDataEventArgs e)
		{
			if (_ReceiveByteEventHandler != null)
			{
				_ReceiveByteEventHandler(e);
			}
		}

		protected virtual void OnReceiveTextMsg(MsgTypeCell msgTypeCell)
		{
			if (_ReceiveTextMsgEventHandler != null)
			{
				_ReceiveTextMsgEventHandler(msgTypeCell);
			}
		}

		private void CreateIContainer()
		{
			_IContainer = new Container();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _IContainer != null)
			{
				_IContainer.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
