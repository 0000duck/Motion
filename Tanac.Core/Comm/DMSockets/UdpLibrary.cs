using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Tanac.Comm.Tool;
using Tanac.Log4Net;

namespace DMSkin.Sockets
{
	public class UdpLibrary : IDisposable
	{
		private UdpClient _UdpClient;

		private int PortNum;

		private bool IsConnected;

		private ReceiveDataEventHandler _ReceiveDataEventHandler;

		[Category("UDP服务端")]
		[Description("UDP监听端口")]
		public int Port
		{
			get
			{
				return PortNum;
			}
			set
			{
				PortNum = value;
			}
		}

		[Category("UDP服务端")]
		[Description("UDP客户端")]
		internal UdpClient UdpClientObj => _UdpClient;

		public event ReceiveDataEventHandler ReceiveData
		{
			add
			{
				ReceiveDataEventHandler receiveDataEventHandler = _ReceiveDataEventHandler;
				ReceiveDataEventHandler receiveDataEventHandler2;
				do
				{
					receiveDataEventHandler2 = receiveDataEventHandler;
					ReceiveDataEventHandler value2 = (ReceiveDataEventHandler)Delegate.Combine(receiveDataEventHandler2, value);
					receiveDataEventHandler = Interlocked.CompareExchange(ref _ReceiveDataEventHandler, value2, receiveDataEventHandler2);
				}
				while (receiveDataEventHandler != receiveDataEventHandler2);
			}
			remove
			{
				ReceiveDataEventHandler receiveDataEventHandler = _ReceiveDataEventHandler;
				ReceiveDataEventHandler receiveDataEventHandler2;
				do
				{
					receiveDataEventHandler2 = receiveDataEventHandler;
					ReceiveDataEventHandler value2 = (ReceiveDataEventHandler)Delegate.Remove(receiveDataEventHandler2, value);
					receiveDataEventHandler = Interlocked.CompareExchange(ref _ReceiveDataEventHandler, value2, receiveDataEventHandler2);
				}
				while (receiveDataEventHandler != receiveDataEventHandler2);
			}
		}

		public UdpLibrary()
		{
			PortNum = 1234;
		}

		public UdpLibrary(int port)
		{
			PortNum = port;
		}

		public bool Start()
		{
			if (!IsConnected)
			{
				try
				{
					_UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, Port));
					IsConnected = true;
					ReceiveInternal();
				}
				catch (Exception ex)
				{
					Log.Error(ex.Message);
					IsConnected = false;
				}
				return IsConnected;
			}
			return false;
		}

		public void Stop()
		{
			try
			{
				IsConnected = false;
				UdpClientObj?.Close();
				_UdpClient = null;
			}
			catch
			{
			}
		}

		public void Send(IDataCell cell, IPEndPoint remoteIP)
		{
			byte[] buffer = cell.ToBuffer();
			SendInternal(buffer, remoteIP);
		}

		public void Send(byte[] buffer, IPEndPoint remoteIP)
		{
			SendInternal(buffer, remoteIP);
		}

		protected void SendInternal(byte[] buffer, IPEndPoint remoteIP)
		{
			if (!IsConnected)
			{
				throw new ApplicationException("UDP Closed.");
			}
			try
			{
				UdpClientObj.BeginSend(buffer, buffer.Length, remoteIP, SendCallBack, null);
			}
			catch (SocketException ex)
			{
				throw ex;
			}
		}

		protected void ReceiveInternal()
		{
			if (!IsConnected)
			{
				return;
			}
			try
			{
				if (UdpClientObj != null)
				{
					UdpClientObj.BeginReceive(ReceiveCallBack, null);
				}
			}
			catch (SocketException ex)
			{
				throw ex;
			}
		}

		private void SendCallBack(IAsyncResult input)
		{
			try
			{
				UdpClientObj.EndSend(input);
			}
			catch (SocketException ex)
			{
				throw ex;
			}
		}

		private void ReceiveCallBack(IAsyncResult input)
		{
			if (!IsConnected)
			{
				return;
			}
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
			byte[] buffer = null;
			try
			{
				buffer = UdpClientObj.EndReceive(input, ref remoteEP);
			}
			catch (SocketException)
			{
			}
			finally
			{
				ReceiveInternal();
			}
			OnReceiveData(new ReceiveDataEventArgs(buffer, remoteEP));
		}

		public void Dispose()
		{
			IsConnected = false;
			if (_UdpClient != null)
			{
				_UdpClient.Close();
				_UdpClient = null;
			}
		}

		[Description("UDP服务端接收数据事件")]
		[Category("UDPServer事件")]
		protected virtual void OnReceiveData(ReceiveDataEventArgs e)
		{
			if (_ReceiveDataEventHandler != null)
			{
				_ReceiveDataEventHandler(this, e);
			}
		}
	}
}
