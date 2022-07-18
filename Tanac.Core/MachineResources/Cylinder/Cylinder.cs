using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
	[Serializable]
	public abstract class Cylinder
    {
        public string Name { get; set; }
		[NonSerialized]
		public InputSetting _input_Home;
		[NonSerialized]
		public InputSetting _input_Work;
		[NonSerialized]
		public OutputSetting _output_Home;
		[NonSerialized]
		public OutputSetting _output_Work;

		public string _input_Home_Name;

		public string _input_Work_Name;

		public string _output_Home_Name;

		public string _output_Work_Name;

		public abstract bool BindIoSetting();
		public abstract bool? Stop();
		public abstract bool Home();
		public abstract bool Work();
		public abstract bool WaitHome(int timeout = int.MaxValue);
		public abstract bool WaitWork(int timeout = int.MaxValue);
		public abstract bool WorkAndWaitDone(int timeout = int.MaxValue);
		public abstract bool HomeAndWaitDone(int timeout = int.MaxValue);
		public abstract bool? isHomed();
		public abstract bool? isWorked();
		protected void AlarmPause(int intError, bool bShowDialog = true)
        {

        }

	}
}
