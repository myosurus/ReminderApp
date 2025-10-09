using SQLite;

namespace ReminderApp;

public class ReminderDatabase
{
	private readonly SQLiteAsyncConnection _database;

	public ReminderDatabase(string dbPath)
	{
		_database = new SQLiteAsyncConnection(dbPath);
		_database.CreateTableAsync<Reminder>().Wait();
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

	public Task<int> DeleteReminderAsync(Reminder reminder) =>
		_database.DeleteAsync(reminder);
}
