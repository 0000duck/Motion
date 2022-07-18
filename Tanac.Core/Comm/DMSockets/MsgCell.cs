using System;

namespace DMSkin.Sockets
{
	[Serializable]
	public class MsgCell : IDataCell
	{
		private int _MessageId;

		private object _Data;

		public int MessageId
		{
			get
			{
				return _MessageId;
			}
			set
			{
				_MessageId = value;
			}
		}

		public object Data
		{
			get
			{
				return _Data;
			}
			set
			{
				_Data = value;
			}
		}

		public MsgCell()
		{
		}

		public MsgCell(int messageId, object data)
		{
			_MessageId = messageId;
			_Data = data;
		}

		public byte[] ToBuffer()
		{
			byte[] array = SerHelper.Serialize(_Data);
			byte[] bytes = BitConverter.GetBytes(MessageId);
			byte[] array2 = new byte[array.Length + bytes.Length];
			Buffer.BlockCopy(bytes, 0, array2, 0, bytes.Length);
			Buffer.BlockCopy(array, 0, array2, bytes.Length, array.Length);
			return array2;
		}

		public void FromBuffer(byte[] buffer)
		{
			_MessageId = BitConverter.ToInt32(buffer, 0);
			_Data = SerHelper.Deserialize(buffer, 4);
		}
	}
}
