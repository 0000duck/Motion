using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace Tanac.Log4Net
{
	public class TextBoxBaseAppender : AppenderSkeleton
	{
		private RichTextBox m_RichTextBox;

		private FlowDocument m_FlowDocument = new FlowDocument();

		private Paragraph m_Paragraph = new Paragraph();

		private Regex m_SectionRegex = new Regex(".+日志监听\\[(?<key1>.*?)\\].+");

		private bool m_Flag = false;

		private List<string> m_LogStrList = new List<string>();

		private object m_LockObj = new object();

		public RichTextBox RichTextBox
		{
			get
			{
				return m_RichTextBox;
			}
			set
			{
				m_RichTextBox = value;
				m_FlowDocument.Blocks.Add(m_Paragraph);
				m_RichTextBox.Document = m_FlowDocument;
			}
		}

		public static event Action<string> s_LogString;

		public void ClearLog()
		{
			m_Paragraph.Inlines.Clear();
		}

		static TextBoxBaseAppender()
		{
		}

		protected override void Append(LoggingEvent loggingEvent)
		{
			if (RichTextBox == null || Log.IsFastModle)
			{
				return;
			}
			PatternLayout patternLayout = (PatternLayout)Layout;
			string empty = string.Empty;
			if (patternLayout != null)
			{
				empty = patternLayout.Format(loggingEvent);
				if (loggingEvent.ExceptionObject != null)
				{
					empty = empty + loggingEvent.ExceptionObject.ToString() + Environment.NewLine;
				}
			}
			else
			{
				empty = loggingEvent.LoggerName + "-" + loggingEvent.RenderedMessage + Environment.NewLine;
			}
			printf(empty);
		}

		public Run AddNewLine(string log, string logLevel)
		{
			Run run = new Run(log);
			string value;
			switch (logLevel.Trim())
			{
				case "WARN":
					value = "#FF7F50";
					break;
				case "ERROR":
					value = "#FF4D27";
					break;
				case "FATAL":
					value = "#FF4D27";
					break;
				default:
					value = ((!log.Contains("接收数据") && !log.Contains("发送数据")) ? "#BEBEBE" : "#8FC9CB");
					break;
			}
			run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
			if (m_Paragraph.Inlines.Count > 200)
			{
				m_Paragraph.Inlines.Remove(m_Paragraph.Inlines.FirstInline);
			}
			return run;
		}

		private void printf(string str)
		{
			lock (m_LockObj)
			{
				if (!str.Contains("--DEBUG--"))
				{
					m_LogStrList.Add(str);
				}
			}
			if (m_Flag)
			{
				return;
			}
			m_Flag = true;
			Task.Run(delegate
			{
				while (true)
				{
					Thread.Sleep(300);
					if (m_LogStrList.Count != 0)
					{
						List<string> tempList = new List<string>();
						lock (m_LockObj)
						{
							for (int i = 0; i < m_LogStrList.Count; i++)
							{
								TextBoxBaseAppender.s_LogString?.Invoke(m_LogStrList[i]);
								tempList.Add(m_LogStrList[i]);
							}
							m_LogStrList.Clear();
						}
						if (tempList.Count > 0)
						{
							try
							{
								m_RichTextBox.Dispatcher.Invoke(delegate
								{
									try
									{
										List<Run> list = new List<Run>();
										foreach (string item in tempList)
										{
											string[] array = Regex.Split(item, "--", RegexOptions.IgnoreCase);
											if (m_Paragraph.Inlines.Count < 100)
											{
												list.Add(AddNewLine(item, array[1]));
											}
											else
											{
												m_Paragraph.Inlines.Clear();
												list.Add(AddNewLine(item, array[1]));
											}
											if (item.Contains("-日志监听["))
											{
												Match match = m_SectionRegex.Match(item);
												string value = match.Groups["key1"].Value;
												LogMonitorWindow.Instance.AddNewLog(value, item, array[1]);
											}
										}
										if (m_Paragraph.Inlines.FirstInline == null)
										{
											list.Reverse();
											m_Paragraph.Inlines.AddRange(list);
										}
										else
										{
											foreach (Run item2 in list)
											{
												m_Paragraph.Inlines.InsertBefore(m_Paragraph.Inlines.FirstInline, item2);
											}
										}
										if (Log.s_IsAutoScroll)
										{
											m_RichTextBox.ScrollToEnd();
										}
									}
									catch (Exception)
									{
										throw;
									}
								});
							}
							catch (TaskCanceledException)
							{
							}
							catch (Exception)
							{
								throw;
							}
						}
					}
				}
			});
		}
	}
}
