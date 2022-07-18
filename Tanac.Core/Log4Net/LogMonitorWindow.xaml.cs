using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tanac.Log4Net
{
    public delegate void ClearLogWindowEvent(object sender, RoutedEventArgs e);
	/// <summary>
	/// LogMonitorWindow.xaml 的交互逻辑
	/// </summary>
	public partial class LogMonitorWindow : Window
	{
		public Dictionary<string, LogViewControl> s_Dic = new Dictionary<string, LogViewControl>();

		private static LogMonitorWindow instance;
		public static LogMonitorWindow Instance => instance;

		public event ClearLogWindowEvent ClearLogWindowEvent;

		static LogMonitorWindow()
		{
			instance = null;
			if (instance == null)
			{
				instance = new LogMonitorWindow();
			}
		}

		public void ClearLogMonitorWindows()
		{
			s_Dic.Clear();
			base.Dispatcher.Invoke(delegate
			{
				wrapPanel.Children.Clear();
			});
		}

		public void ClearAllLogWindowShow()
		{
			this.ClearLogWindowEvent?.Invoke(null, null);
		}

		private LogMonitorWindow()
		{
			InitializeComponent();
		}

		public void AddNewLog(string section, string log, string logLevel)
		{
			if (!string.IsNullOrEmpty(section))
			{
				if (!s_Dic.ContainsKey(section))
				{
					LogViewControl logViewControl = new LogViewControl();
					logViewControl.HeaderName = section;
					s_Dic[section] = logViewControl;
					wrapPanel.Children.Add(logViewControl);
				}
				s_Dic[section].AddNewLine(log, logLevel);
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

		public void Remove(string sectioName)
		{
			s_Dic.Remove(sectioName);
			foreach (LogViewControl child in wrapPanel.Children)
			{
				if (child.HeaderName == sectioName)
				{
					wrapPanel.Children.Remove(child);
					break;
				}
			}
		}
	}
}
