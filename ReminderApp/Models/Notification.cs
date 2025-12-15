using SQLite;

namespace ReminderApp.Models;

public class Notification
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int ReminderId { get; set; }
}

