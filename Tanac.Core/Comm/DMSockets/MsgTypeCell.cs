using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSkin.Sockets
{
	[Serializable]
	public class MsgTypeCell
	{
		private MsgType _Msgtype;

		private string _ImageSuffix;

		private byte[] _BufferBytes;

		public MsgType Msgtype
		{
			get
			{
				return _Msgtype;
			}
			set
			{
				_Msgtype = value;
			}
		}

		public string ImageSuffix
		{
			get
			{
				return _ImageSuffix;
			}
			set
			{
				_ImageSuffix = value;
			}
		}

		public byte[] BufferBytes
		{
			get
			{
				return _BufferBytes;
			}
			set
			{
				_BufferBytes = value;
			}
		}

		public MsgTypeCell(MsgType msgType, byte[] buffer)
		{
			_ImageSuffix = "";
			Msgtype = msgType;
			BufferBytes = buffer;
		}
	}
}
