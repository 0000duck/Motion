using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.ExceptionExt
{
	public class AxisException : UserException
	{
		public AxisException()
		{
		}

		public AxisException(string message)
			: base(message)
		{
		}

		public AxisException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
