using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace ReminderApp.Models;

public static class NotificationService
{
    public static void ScheduleReminder(Reminder reminder, int notificationId)
    {
        var request = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = reminder.Name,
            Description = reminder.Description,
            Schedule =
            {
                NotifyTime = reminder.StartReminding,
                RepeatType = NotificationRepeat.TimeInterval,
                NotifyRepeatInterval = reminder.RemindFrequency,
                Android =
                {
                    AlarmType = AndroidAlarmType.RtcWakeup
                }
            }
        };

        LocalNotificationCenter.Current.Show(request);
    }

    public static void CancelNotification(int notificationId)
    {
        LocalNotificationCenter.Current.Cancel(notificationId);
    }
}
