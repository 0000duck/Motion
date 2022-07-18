using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Log4Net.Notify
{
	public class EventData
	{
		public EventNotifyType EventNotify { get; set; }

		public object Data { get; set; }
	}
}
