using SQLite;

namespace ReminderApp.Models;

public class ReminderDatabase
{
	private readonly SQLiteAsyncConnection _database;

	public ReminderDatabase(string dbPath)
	{
		_database = new SQLiteAsyncConnection(dbPath);
		_database.CreateTableAsync<Reminder>().Wait(); 
		_database.CreateTableAsync<Notification>().Wait();
    }

    public Task<List<Reminder>> GetRemindersAsync() =>
		_database.Table<Reminder>().ToListAsync();

	public Task<Reminder> GetReminderAsync(int id) =>
		_database.Table<Reminder>().Where(r => r.Id == id).FirstOrDefaultAsync();

	public Task<int> SaveReminderAsync(Reminder reminder)
	{
		if(reminder.Id != 0)
			return _database.UpdateAsync(reminder);
		else
			return _database.InsertAsync(reminder);
	}

    public Task<int> CreateReminderAsync(Reminder reminder)
    {
        return _database.InsertAsync(reminder);
    }

    public Task<int> DeleteReminderAsync(Reminder reminder) =>
		_database.DeleteAsync(reminder);

    public Task<int> CreateNotificationAsync(Notification notification)
    {
        return _database.InsertAsync(notification);
    }
    public Task<int> SaveNotificationAsync(Notification notification)
    {
        if (notification.Id != 0)
            return _database.UpdateAsync(notification);
        else
            return _database.InsertAsync(notification);
    }

    public Task<int> DeleteNotificationAsync(Notification notification) =>
    _database.DeleteAsync(notification);

    public Task<List<Notification>> GetNotificationsByReminderIdAsync(int reminderId) =>
    _database.Table<Notification>()
             .Where(n => n.ReminderId == reminderId)
             .ToListAsync();
}
