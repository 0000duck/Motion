using System;
using System.Windows;

namespace Tanac.Log4Net
{
	[Serializable]
	public class LogData : DependencyObject
	{
		public static bool s_IsOnlyShowCommunacation = false;

		public static readonly DependencyProperty IsShowProperty = DependencyProperty.Register("IsShow", typeof(bool), typeof(LogData), new PropertyMetadata(true));

		public string DateTime { get; set; }

		public string LogLevel { get; set; }

		public string LogText { get; set; }

		public string TextColor { get; set; }

		public bool IsShow
		{
			get
			{
				return (bool)GetValue(IsShowProperty);
			}
			set
			{
				SetValue(IsShowProperty, value);
			}
		}

		public LogData(string _DateTime, string _LogLevel, string _LogText)
		{
			RefushData(_DateTime, _LogLevel, _LogText);
		}

		public void RefushData(string _DateTime, string _LogLevel, string _LogText)
		{
			DateTime = _DateTime;
			LogLevel = _LogLevel.Trim();
			LogText = _LogText.Trim();
			if (!LogText.Contains("接收数据") && !LogText.Contains("发送数据"))
			{
				IsShow = !s_IsOnlyShowCommunacation;
			}
			switch (LogLevel)
			{
				case "WARN":
					TextColor = "#8B4513";
					return;
				case "ERROR":
					TextColor = "#FF4D27";
					return;
				case "FATAL":
					TextColor = "#FF4D27";
					return;
			}
			if (LogText.Contains("接收数据") || LogText.Contains("发送数据"))
			{
				TextColor = "#0000FF";
			}
			else
			{
				TextColor = "#000000";
			}
		}
	}
}
