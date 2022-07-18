using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.ExceptionExt
{
	public class UserException : ApplicationException
	{
		public UserException()
		{
		}

		public UserException(string message)
			: base(message)
		{
		}

		public UserException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
