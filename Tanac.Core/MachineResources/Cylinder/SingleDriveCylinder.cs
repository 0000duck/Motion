using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tanac.Core.MachineResources
{
	[Serializable]
	public class SingleDriveCylinder : Cylinder
	{
		public SingleDriveCylinder(string name, OutputSetting output, InputSetting input_Home, InputSetting input_Work)
		{
			Name = name;
			_output_Work = output;
			_input_Home = input_Home;
			_input_Work = input_Work;
			if (_input_Home == null && _input_Work == null)
			{
				throw new Exception("[" +Name + "] Work Sensor and Home sensor are null !");
			}
			if (_output_Work == null)
			{
				throw new Exception("[" +Name + "] output is null !");
			}
			_output_Work_Name = output.OutputName;
			_input_Home_Name = input_Home.InputName;
			_input_Work_Name = input_Work.InputName;
		}

		public override bool Home()
		{
			try
			{
				return _output_Work.OFF();
			}
			catch (Exception ex)
			{
				throw new Exception("SingleDriveCylinder [" + Name + "] Home Exception:" + ex.StackTrace);
			}
		}

		public override bool WaitHome(int timeout = int.MaxValue)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			bool result = false;
			try
			{
				if (_input_Home != null)
				{
					while (!_input_Home.GetStatus())
					{
						if (stopwatch.ElapsedMilliseconds >= timeout)
						{
							return true;
						}
						Thread.Sleep(1);
					}
				}
				if (_input_Work != null)
				{
					while (_input_Work.GetStatus())
					{
						if (stopwatch.ElapsedMilliseconds >= timeout)
						{
							return true;
						}
						Thread.Sleep(1);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("SingleDriveCylinder [" + Name + "] WaitHome Exception!" + ex.StackTrace);
			}
			return result;
		}

		public override bool Work()
		{
			try
			{
				return _output_Work.ON();
			}
			catch (Exception ex)
			{
				throw new Exception("SingleDriveCylinder [" + Name + "] Work Exception:" + ex.StackTrace);
			}
		}

		public override bool WaitWork(int timeout = int.MaxValue)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			bool result = false;
			try
			{
				if (_input_Home != null)
				{
					while (_input_Home.GetStatus())
					{
						if (stopwatch.ElapsedMilliseconds >= timeout)
						{
							return true;
						}
						Thread.Sleep(1);
					}
				}
				if (_input_Work != null)
				{
					while (!_input_Work.GetStatus())
					{
						if (stopwatch.ElapsedMilliseconds >= timeout)
						{
							return true;
						}
						Thread.Sleep(1);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("SingleDriveCylinder [" + Name + "] WaitWork Exception!" + ex.StackTrace);
			}
			return result;
		}

		public override bool WorkAndWaitDone(int timeout = int.MaxValue)
		{
			bool flag = false;
			try
			{
				Work();
				return WaitWork(timeout);
			}
			catch (Exception ex)
			{
				throw new Exception("[" + Name + "] WorkAndWaitDone Exception:" + ex.StackTrace);
			}
		}

		public override bool HomeAndWaitDone(int timeout = int.MaxValue)
		{
			bool flag = false;
			try
			{
				Home();
				return WaitHome(timeout);
			}
			catch (Exception ex)
			{
				throw new Exception("[" + Name + "] HomeAndWaitDone Exception:" + ex.StackTrace);
			}
		}

		public override bool? isHomed()
		{
			return _input_Home?.GetStatus();
		}

		public override bool? isWorked()
		{
			return _input_Work?.GetStatus();
		}

		public override bool? Stop()
		{
			return null;
		}

        public override bool BindIoSetting()
        {
			_output_Work = OutputManager.Get(_output_Work_Name);
			_input_Home = InputManager.Get(_input_Home_Name);
			_input_Work = InputManager.Get(_input_Work_Name);
			return true;
		}
    }
}
