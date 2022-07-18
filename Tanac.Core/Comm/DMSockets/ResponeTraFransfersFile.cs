using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSkin.Sockets
{
	[Serializable]
	public class ResponeTraFransfersFile
	{
		private string _MD5;

		private int _Size;

		private int _Index;

		public string MD5
		{
			get
			{
				return _MD5;
			}
			set
			{
				_MD5 = value;
			}
		}

		public int Size
		{
			get
			{
				return _Size;
			}
			set
			{
				_Size = value;
			}
		}

		public int Index
		{
			get
			{
				return _Index;
			}
			set
			{
				_Index = value;
			}
		}

		public ResponeTraFransfersFile()
		{
		}

		public ResponeTraFransfersFile(string md5, int size, int index)
		{
			_MD5 = md5;
			_Size = size;
			_Index = index;
		}
	}
}
