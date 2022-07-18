using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.ExceptionExt
{
	public class CylinderException : UserException
	{
		public CylinderException()
		{
		}

		public CylinderException(string message)
			: base(message)
		{
		}

		public CylinderException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
