namespace ReminderApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

#if ANDROID
    await RequestNotificationPermissionAsync();
#endif
		}

		private async Task RequestNotificationPermissionAsync()
		{
			var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

			if(status != PermissionStatus.Granted)
			{
				status = await Permissions.RequestAsync<Permissions.PostNotifications>();
			}
		}

	}
}
