using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Log4Net.Notify
{
	public interface IEventNotify
	{
		void EventNotify(EventData eventData);
	}
}
