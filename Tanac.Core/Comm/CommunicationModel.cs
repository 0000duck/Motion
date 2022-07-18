using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Comm
{
	/// <summary>
	/// 通讯模式
	/// </summary>
	public enum CommunicationModel
	{
		TcpClient,
		TcpServer,
		UDP,
		COM
	}
}
