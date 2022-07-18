using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
	/// <summary>
	/// LogViewControl.xaml 的交互逻辑
	/// </summary>
	public partial class LogViewControl : UserControl
	{
		private FlowDocument m_FlowDocument = new FlowDocument();

		private Paragraph m_Paragraph = new Paragraph();
		public string HeaderName { get; set; }

		public LogViewControl()
		{
			InitializeComponent();
			base.DataContext = this;
			m_FlowDocument.Blocks.Add(m_Paragraph);
			myRichTextBox.Document = m_FlowDocument;
			LogMonitorWindow.Instance.ClearLogWindowEvent += Clear_Click;
		}

		public void AddNewLine(string log, string logLevel)
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
			m_Paragraph.Inlines.Add(run);
			if (m_Paragraph.Inlines.Count > 200)
			{
				m_Paragraph.Inlines.Clear();
			}
		}

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			m_Paragraph.Inlines.Clear();
		}

		private void ClearAll_Click(object sender, RoutedEventArgs e)
		{
			LogMonitorWindow.Instance.ClearAllLogWindowShow();
		}

		private void remove_Click(object sender, RoutedEventArgs e)
		{
			LogMonitorWindow.Instance.Remove(HeaderName);
		}
	}
}
