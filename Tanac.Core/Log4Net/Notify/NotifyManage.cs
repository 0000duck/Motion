using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Tanac.Log4Net.Notify
{
	public class NotifyManage : IMainEventCommnuicationHandler, IEventNotify
	{
		public static bool s_IsAutoClose = true;

		public static List<NotificationWindow> _dialogs = new List<NotificationWindow>();

		public static NotifyManage Instance = new NotifyManage();

		private void ShowNotify(NotifyData data)
		{
			int num = (int)(SystemParameters.WorkArea.Height / 110.0);
			if (_dialogs.Count > 4)
			{
				NotificationWindow notificationWindow = _dialogs.Last();
				notificationWindow.SetNewNotifyData(data);
				return;
			}
			NotificationWindow notificationWindow2 = new NotificationWindow();
			notificationWindow2.Closed += Dialog_Closed;
			notificationWindow2.TopFrom = GetTopFrom(notificationWindow2);
			notificationWindow2.HideAllNotificationWindow = HideAllNotificationWindow;
			notificationWindow2.DataContext = data;
			notificationWindow2.Show();
			_dialogs.Add(notificationWindow2);
		}

		private void HideAllNotificationWindow()
		{
			List<NotificationWindow> dialogs = _dialogs;
			foreach (NotificationWindow item in dialogs)
			{
				item.HideWindow();
			}
		}

		private void Dialog_Closed(object sender, EventArgs e)
		{
			NotificationWindow item = sender as NotificationWindow;
			_dialogs.Remove(item);
		}

		private double GetTopFrom(NotificationWindow nWindow)
		{
			double topFrom = SystemParameters.WorkArea.Bottom - 10.0;
			bool flag = _dialogs.Any((NotificationWindow o) => o.TopFrom == topFrom);
			while (flag)
			{
				topFrom = topFrom - nWindow.Height - 10.0;
				flag = _dialogs.Any((NotificationWindow o) => o.TopFrom == topFrom);
			}
			if (topFrom <= 0.0)
			{
				topFrom = SystemParameters.WorkArea.Bottom - 10.0;
			}
			return topFrom;
		}

		public void EventNotify(EventData eventData)
		{
			if (eventData.EventNotify == EventNotifyType.New)
			{
				NotifyData data = eventData.Data as NotifyData;
				SendMessage(data);
			}
		}

		public void SendMessage(NotifyData data)
		{
			Application.Current.Dispatcher.BeginInvoke((Action)delegate
			{
				ShowNotify(data);
			});
		}
	}
}
