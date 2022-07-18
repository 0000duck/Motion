using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System.IO;
using Tanac.Log4Net.Notify;

namespace Tanac.Log4Net
{
	public class Log
	{
		public static bool s_IsStarting = false;

		public static bool s_IsAutoScroll = false;

		public static List<string> s_ErrMsg = new List<string>();

		private static ILog log4Net = LogManager.GetLogger("logLogger");

		private static readonly ILog LogModify = LogManager.GetLogger("modifyLogger");

		private static bool s_ShowTip=false;

		public static bool IsFastModle = false;

		public static void RegisterLog()
		{
			s_IsStarting = true;
			XmlConfigurator.ConfigureAndWatch(new FileInfo(Application.StartupPath + "\\log4net.config"));
		}

		public static void Debug(string str)
		{
			log4Net.Debug(str);
		}

		public static void Info(string str)
		{
			log4Net.Info(str);
		}

		public static void Warn(string str)
		{
			if (s_IsStarting && !s_ErrMsg.Contains(str))
			{
				s_ErrMsg.Add(str);
			}
			log4Net.Warn(str);
			ShowTip(str, LogLevel.Warn);
		}

		public static void Error(string str)
		{
			if (s_IsStarting && !s_ErrMsg.Contains(str))
			{
				s_ErrMsg.Add(str);
			}
			log4Net.Error(str);
			ShowTip(str, LogLevel.Error);
		}

		public static void Fatal(string str)
		{
			if (s_IsStarting && !s_ErrMsg.Contains(str))
			{
				s_ErrMsg.Add(str);
			}
			log4Net.Fatal(str);
			ShowTip(str, LogLevel.Fatal);
		}

		public static void ModullParamModify(string str)
		{
			LogModify.Info(str);
		}

		public static void Tip(string str)
		{
			log4Net.Info(str);
			ShowTip(str, LogLevel.Info);
		}

		public static void ForceTip(string str)
		{
			log4Net.Info(str);
			ShowTip(str, LogLevel.Info, IsForce: true);
		}

		public static void ShowTip(string str, LogLevel LogLevel, bool IsForce = false)
		{
			if ((!IsFastModle || IsForce) && s_ShowTip)
			{
				NotifyManage.Instance.SendMessage(new NotifyData
				{
					Content = str,
					LogLevel = LogLevel
				});
			}
		}

		public static void Print(LogLevel LogLevel, string str)
		{
			switch (LogLevel)
			{
				case LogLevel.Debug:
					Debug(str);
					break;
				case LogLevel.Info:
					Info(str);
					break;
				case LogLevel.Tip:
					Tip(str);
					break;
				case LogLevel.Warn:
					Warn(str);
					break;
				case LogLevel.Error:
					Error(str);
					break;
				case LogLevel.Fatal:
					Fatal(str);
					break;
				default:
					Debug(str);
					break;
			}
		}

		public static void InitializeDataGrid(System.Windows.Controls.RichTextBox richTextBox)
		{
			if (richTextBox != null)
			{
				string pattern = "%d{yyyy-MM-dd HH:mm:ss} --%-5p-- %m%n";
				TextBoxBaseAppender textBoxBaseAppender = new TextBoxBaseAppender();
				textBoxBaseAppender.RichTextBox = richTextBox;
				textBoxBaseAppender.Layout = new PatternLayout(pattern);
				Logger logger = log4Net.Logger as Logger;
				logger.AddAppender(textBoxBaseAppender);
			}
		}

		public static void SetTipWindowVisibility(bool flag)
		{
			s_ShowTip = flag;
		}

		private static string getFolder()
		{
			try
			{
				Logger logger = log4Net.Logger as Logger;
				RollingFileAppender rollingFileAppender = logger.GetAppender("LogFile") as RollingFileAppender;
				if (rollingFileAppender == null)
				{
					return "";
				}
				return Path.GetDirectoryName(rollingFileAppender.File);
			}
			catch (Exception)
			{
				return "";
			}
		}

		public static void DeleteLog(int dayNum)
		{
			Task.Run(delegate
			{
				try
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(getFolder());
					FileInfo[] files = directoryInfo.GetFiles();
					FileInfo[] array = files;
					foreach (FileInfo fileInfo in array)
					{
						DateTime lastWriteTime = fileInfo.LastWriteTime;
						int days = (DateTime.Now - lastWriteTime).Days;
						if (days > dayNum)
						{
							File.Delete(fileInfo.FullName);
						}
					}
				}
				catch (Exception)
				{
				}
			});
		}

		public static void UpdateLogLevel(string str)
		{
			try
			{
				LogLevel logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), str);
				Level debug = Level.Debug;
				switch (logLevel)
				{
					case LogLevel.Debug:
						debug = Level.Debug;
						break;
					case LogLevel.Info:
						debug = Level.Info;
						break;
					case LogLevel.Warn:
						debug = Level.Warn;
						break;
					case LogLevel.Error:
						debug = Level.Error;
						break;
					case LogLevel.Fatal:
						debug = Level.Fatal;
						break;
					default:
						debug = Level.Debug;
						break;
				}
				Logger logger = log4Net.Logger as Logger;
				logger.Level = debug;
			}
			catch (Exception ex)
			{
				Error(ex.ToString());
				throw;
			}
		}
	}
}
