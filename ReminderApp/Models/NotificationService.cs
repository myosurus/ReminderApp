using Plugin.LocalNotification;

namespace ReminderApp.Models;

public static class NotificationService
{
	public static void ScheduleReminder(Reminder reminder)
	{
		var request = new NotificationRequest
		{
			NotificationId = reminder.Id,
			Title = reminder.Name,
			Description = reminder.Description,
			Schedule =
			{
				NotifyTime = reminder.StartReminding,
				RepeatType = NotificationRepeat.TimeInterval,
				NotifyRepeatInterval = reminder.RemindFrequency,
				Android =
				{
					AlarmType = Plugin.LocalNotification.AndroidOption.AndroidAlarmType.RtcWakeup
				}
			}
		};

		LocalNotificationCenter.Current.Show(request);
	}

	public static void CancelReminder(int id)
	{
		LocalNotificationCenter.Current.Cancel(id);
	}
}

