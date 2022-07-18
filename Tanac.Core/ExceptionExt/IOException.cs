using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.ExceptionExt
{
	public class IOException : UserException
	{
		public IOException()
		{
		}

		public IOException(string message)
			: base(message)
		{
		}

		public IOException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
