using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.ExceptionExt
{
	public class TimeoutException : UserException
	{
		public TimeoutException()
		{
		}

		public TimeoutException(string message)
			: base(message)
		{
		}

		public TimeoutException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
