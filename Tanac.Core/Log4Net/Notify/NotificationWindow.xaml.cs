using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Tanac.Log4Net.Notify
{
	/// <summary>
	/// NotificationWindow.xaml 的交互逻辑
	/// </summary>
	public partial class NotificationWindow : Window
	{
		public Action HideAllNotificationWindow;
		public double TopFrom { get; set; }

		public NotificationWindow()
		{
			InitializeComponent();
			base.Loaded += NotificationWindow_Loaded;
		}

		public void SetNewNotifyData(NotifyData data)
		{
			base.Visibility = Visibility.Visible;
			tbContent.Text = data.Content;
			switch (data.LogLevel)
			{
				case LogLevel.Debug:
					tbContent.Foreground = Brushes.Black;
					break;
				case LogLevel.Info:
					tbContent.Foreground = Brushes.Black;
					break;
				case LogLevel.Warn:
					tbContent.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D27"));
					break;
				case LogLevel.Error:
					tbContent.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D27"));
					break;
				default:
					tbContent.Foreground = Brushes.Red;
					break;
			}
		}

		private void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
		{
			NotifyData notifyData = base.DataContext as NotifyData;
			if (notifyData != null)
			{
				SetNewNotifyData(notifyData);
			}
			NotificationWindow self = sender as NotificationWindow;
			if (self == null)
			{
				return;
			}
			self.UpdateLayout();
			double right = SystemParameters.WorkArea.Right;
			self.Top = self.TopFrom - self.ActualHeight;
			DoubleAnimation animation = new DoubleAnimation();
			animation.Duration = new Duration(TimeSpan.FromMilliseconds(200.0));
			animation.From = right;
			animation.To = right - self.ActualWidth;
			self.BeginAnimation(Window.LeftProperty, animation);
			Task.Factory.StartNew(delegate
			{
				do
				{
					int num = 5;
					Thread.Sleep(TimeSpan.FromSeconds(num));
				}
				while (!NotifyManage.s_IsAutoClose);
				base.Dispatcher.Invoke(delegate
				{
					animation = new DoubleAnimation();
					animation.Duration = new Duration(TimeSpan.FromMilliseconds(500.0));
					animation.Completed += delegate
					{
						self.Close();
					};
					animation.From = right - self.ActualWidth;
					animation.To = right;
					self.BeginAnimation(Window.LeftProperty, animation);
				});
			});
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			double right = SystemParameters.WorkArea.Right;
			DoubleAnimation doubleAnimation = new DoubleAnimation();
			doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500.0));
			doubleAnimation.Completed += delegate
			{
				Close();
			};
			doubleAnimation.From = right - base.ActualWidth;
			doubleAnimation.To = right;
			BeginAnimation(Window.LeftProperty, doubleAnimation);
		}

		public void HideWindow()
		{
			double right = SystemParameters.WorkArea.Right;
			DoubleAnimation doubleAnimation = new DoubleAnimation();
			doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500.0));
			doubleAnimation.Completed += delegate
			{
				Close();
			};
			doubleAnimation.From = right - base.ActualWidth;
			doubleAnimation.To = right;
			BeginAnimation(Window.LeftProperty, doubleAnimation);
		}

		private void tbContent_MouseDown(object sender, MouseButtonEventArgs e)
		{
			HideAllNotificationWindow?.Invoke();
		}

		private void Window_MouseEnter(object sender, MouseEventArgs e)
		{
			NotifyManage.s_IsAutoClose = false;
		}

		private void Window_MouseLeave(object sender, MouseEventArgs e)
		{
			NotifyManage.s_IsAutoClose = true;
		}
	}
}
