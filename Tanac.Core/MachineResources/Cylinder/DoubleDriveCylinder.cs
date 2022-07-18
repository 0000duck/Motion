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
	public class DoubleDriveCylinder : Cylinder
	{

		public DoubleDriveCylinder(string name, OutputSetting output_Home, OutputSetting output_Work, InputSetting input_Home, InputSetting input_Work)
		{
		     Name = name;
			_output_Home = output_Home;
			_output_Work = output_Work;
			_input_Home = input_Home;
			_input_Work = input_Work;
			if (_input_Home == null && _input_Work == null)
			{
				throw new Exception("DoubleDriveCylinder [" + name + "] Work input and Home input are all null !");
			}
			if (_output_Home == null || _input_Work == null)
			{
				throw new Exception("DoubleDriveCylinder[" + name + "] Home or Work output is null !");
			}
			_output_Home_Name = output_Home.OutputName;
			_output_Work_Name = output_Work.OutputName;
			_input_Home_Name = input_Home.InputName;
			_input_Work_Name = input_Work.InputName;
		}

		public override bool Home()
		{
			bool flag = false;
			try
			{
				flag = _output_Home.ON(500);
				return flag & _output_Work.OFF(500);
			}
			catch (Exception ex)
			{
				throw new Exception("DoubleDriveCylinder[" + Name + "] Home Exception:" + ex.StackTrace);
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
							string title = Name + "报警";
							string excMsg ="回动作位超时";
							string exSolution ="检查动作位传感器";
							CoreFunction.AlarmPause(title, excMsg, exSolution, "", "");
							return false;
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
				throw new Exception("DoubleDriveCylinder[" + Name + "] WaitHome Exception!" + ex.StackTrace);
			}
			return result;
		}

		public override bool Work()
		{
			bool flag = false;
			try
			{
				flag = _output_Home.OFF();
				return flag & _output_Work.ON();
			}
			catch (Exception ex)
			{
				throw new Exception("DoubleDriveCylinder [" + Name + "] Work Exception:" + ex.StackTrace);
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
				throw new Exception("DoubleDriveCylinder [" + Name + "] WaitWork Exception!" + ex.StackTrace);
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
				throw new Exception("DoubleDriveCylinder [" + Name + "] WorkAndWaitDone Exception:" + ex.StackTrace);
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
				throw new Exception("DoubleDriveCylinder [" + Name + "] HomeAndWaitDone  Exception:" + ex.StackTrace);
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
			bool flag = false;
			try
			{
				flag = _output_Home.OFF();
				return flag & _output_Work.OFF();
			}
			catch (Exception ex)
			{
				throw new Exception("DoubleDriveCylinder [" + Name + "] Stop Exception:" + ex.StackTrace);
			}
		}

        public override bool BindIoSetting()
        {
			_output_Home = OutputManager.Get(_output_Home_Name);
			_output_Work = OutputManager.Get(_output_Work_Name);
			_input_Home = InputManager.Get(_input_Home_Name);
			_input_Work = InputManager.Get(_input_Work_Name);
			return true;
		}
    }
}
