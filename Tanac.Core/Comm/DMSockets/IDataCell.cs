using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSkin.Sockets
{
	public interface IDataCell
	{
		byte[] ToBuffer();

		void FromBuffer(byte[] buffer);
	}
}
