namespace ReminderApp;

public partial class App : Application
{
	static ReminderDatabase database;

	public static ReminderDatabase Database
	{
		get
		{
			if(database == null)
			{
				string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Reminders.db3");
				database = new ReminderDatabase(dbPath);
			}
			return database;
		}
	}

	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}
}

