using System;
using System.Net;

namespace DMSkin.Sockets
{
	public delegate void ReceiveDataEventHandler(object sender, ReceiveDataEventArgs e);
	public class ReceiveDataEventArgs : EventArgs
	{
		private byte[] _Buffer;

		private IPEndPoint _RemoteIP;

		public byte[] Buffer
		{
			get
			{
				return _Buffer;
			}
			set
			{
				_Buffer = value;
			}
		}

		public IPEndPoint RemoteIP
		{
			get
			{
				return _RemoteIP;
			}
			set
			{
				_RemoteIP = value;
			}
		}

		public ReceiveDataEventArgs()
		{
		}

		public ReceiveDataEventArgs(byte[] buffer, IPEndPoint remoteIP)
		{
			_Buffer = buffer;
			_RemoteIP = remoteIP;
		}
	}
}
