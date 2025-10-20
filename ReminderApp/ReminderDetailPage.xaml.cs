namespace ReminderApp;

public partial class ReminderDetailPage : ContentPage
{
    public ReminderDetailPage()
    {
        InitializeComponent();

        // Инициализация Picker с русскими названиями
        var urgencyOptions = new List<string> { "Низкий", "Средний", "Высокий" };
        UrgencyPicker.ItemsSource = urgencyOptions;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext == null || ((Reminder)BindingContext).Id != 0 && Shell.Current.CurrentItem.Title == "Добавить")
        {
            BindingContext = new Reminder
            {
                ReminderDate = DateTime.Now,
                IsDone = false
            };
        }

        var reminder = (Reminder)BindingContext;

        // Устанавливаем выбранный уровень срочности
        SetUrgencyPicker(reminder.Urgency);

        // Устанавливаем дату и время
        ReminderDatePicker.Date = reminder.ReminderDate.Date;
        ReminderTimePicker.Time = reminder.ReminderDate.TimeOfDay;

        // Обновляем заголовок в зависимости от режима (добавление/редактирование)
        Title = reminder.Id == 0 ? "Добавить задачу" : "Редактировать задачу";
    }

    private void SetUrgencyPicker(Urgency urgency)
    {
        string urgencyText = urgency switch
        {
            Urgency.Low => "Низкий",
            Urgency.Medium => "Средний",
            Urgency.High => "Высокий",
            _ => "Средний"
        };

        UrgencyPicker.SelectedItem = urgencyText;
    }

    private Urgency GetUrgencyFromString(string urgencyText)
    {
        return urgencyText switch
        {
            "Низкий" => Urgency.Low,
            "Средний" => Urgency.Medium,
            "Высокий" => Urgency.High,
            _ => Urgency.Medium
        };
    }

    async void OnSaveClicked(object sender, EventArgs e)
    {
        if (BindingContext is not Reminder reminder) return;

        // Валидация - проверяем, что название задачи заполнено
        if (string.IsNullOrWhiteSpace(reminder.Name))
        {
            await DisplayAlert("Ошибка", "Пожалуйста, введите название задачи", "OK");
            return;
        }

        // Получаем уровень срочности из Picker
        if (UrgencyPicker.SelectedItem is string selectedUrgency)
        {
            reminder.Urgency = GetUrgencyFromString(selectedUrgency);
        }
        else
        {
            reminder.Urgency = Urgency.Medium;
        }

        // Комбинируем дату и время
        var date = ReminderDatePicker.Date;
        var time = ReminderTimePicker.Time;
        reminder.ReminderDate = date + time;

        // Сохраняем задачу
        await App.Database.SaveReminderAsync(reminder);

        // Показываем сообщение об успехе
        await DisplayAlert("Успех", "Задача успешно сохранена", "OK");

        // Возвращаемся на список задач
        await Shell.Current.GoToAsync("//Reminders");
    }

    async void OnCancelClicked(object sender, EventArgs e)
    {
        // Подтверждение отмены
        bool confirm = await DisplayAlert("Подтверждение",
            "Отменить создание задачи? Все несохраненные данные будут потеряны.",
            "Да", "Нет");

        if (confirm)
        {
            await Shell.Current.GoToAsync("//Reminders");
        }
    }

    async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (BindingContext is not Reminder reminder) return;

        bool confirm = await DisplayAlert("Удалить", "Удалить эту задачу?", "Да", "Нет");
        if (!confirm) return;

        await App.Database.DeleteReminderAsync(reminder);
        await Shell.Current.GoToAsync("//Reminders");
    }
}





//namespace ReminderApp;

//public partial class ReminderDetailPage : ContentPage
//{
//    public ReminderDetailPage()
//    {
//        InitializeComponent();

//        // Инициализация Picker с русскими названиями
//        var urgencyOptions = new List<string> { "Низкий", "Средний", "Высокий" };
//        UrgencyPicker.ItemsSource = urgencyOptions;
//    }

//    protected override void OnAppearing()
//    {
//        base.OnAppearing();

//        if (BindingContext == null || ((Reminder)BindingContext).Id != 0 && Shell.Current.CurrentItem.Title == "Добавить")
//        {
//            BindingContext = new Reminder
//            {
//                ReminderDate = DateTime.Now,
//                IsDone = false
//            };
//        }

//        var reminder = (Reminder)BindingContext;

//        // Скрываем кнопку удаления для новой задачи
//        // DeleteButton.IsVisible = reminder.Id != 0;

//        // Устанавливаем выбранный уровень срочности
//        SetUrgencyPicker(reminder.Urgency);

//        ReminderDatePicker.Date = reminder.ReminderDate.Date;
//        // ReminderTimePicker.Time = reminder.ReminderDate.TimeOfDay;
//    }

//    private void SetUrgencyPicker(Urgency urgency)
//    {
//        string urgencyText = urgency switch
//        {
//            Urgency.Low => "Низкий",
//            Urgency.Medium => "Средний",
//            Urgency.High => "Высокий",
//            _ => "Средний"
//        };

//        UrgencyPicker.SelectedItem = urgencyText;
//    }

//    private Urgency GetUrgencyFromString(string urgencyText)
//    {
//        return urgencyText switch
//        {
//            "Низкий" => Urgency.Low,
//            "Средний" => Urgency.Medium,
//            "Высокий" => Urgency.High,
//            _ => Urgency.Medium
//        };
//    }

//    async void OnSaveClicked(object sender, EventArgs e)
//    {
//        if (BindingContext is not Reminder reminder) return;

//        // Получаем уровень срочности из Picker
//        if (UrgencyPicker.SelectedItem is string selectedUrgency)
//        {
//            reminder.Urgency = GetUrgencyFromString(selectedUrgency);
//        }
//        else
//        {
//            reminder.Urgency = Urgency.Medium;
//        }

//        var date = ReminderDatePicker.Date;
//        // var time = ReminderTimePicker.Time;
//        reminder.ReminderDate = date; // + time;

//        await App.Database.SaveReminderAsync(reminder);
//        await Shell.Current.GoToAsync("//Reminders");
//    }

//    async void OnCancelClicked(object sender, EventArgs e)
//    {
//        await Shell.Current.GoToAsync("//Reminders");
//    }

//    async void OnDeleteClicked(object sender, EventArgs e)
//    {
//        if (BindingContext is not Reminder reminder) return;

//        bool confirm = await DisplayAlert("Удалить", "Удалить эту задачу?", "Да", "Нет");
//        if (!confirm) return;

//        await App.Database.DeleteReminderAsync(reminder);
//        await Shell.Current.GoToAsync("//Reminders");
//    }
//}


//namespace ReminderApp;

//public partial class ReminderDetailPage : ContentPage
//{
//	public ReminderDetailPage()
//	{
//		InitializeComponent();
//		UrgencyPicker.ItemsSource = Enum.GetValues(typeof(Urgency)).Cast<Urgency>().ToList();
//	}

//	protected override void OnAppearing()
//	{
//		base.OnAppearing();

//		if(BindingContext == null || ((Reminder)BindingContext).Id != 0 && Shell.Current.CurrentItem.Title == "Add")
//		{
//			BindingContext = new Reminder
//			{
//				ReminderDate = DateTime.Now,
//				IsDone = false
//			};
//		}

//		var reminder = (Reminder)BindingContext;

//		DeleteButton.IsVisible = reminder.Id != 0;

//		UrgencyPicker.SelectedItem = reminder.Urgency;
//		ReminderDatePicker.Date = reminder.ReminderDate.Date;
//		ReminderTimePicker.Time = reminder.ReminderDate.TimeOfDay;

//		Title = reminder.Id == 0 ? "Add Reminder" : "Edit Reminder";
//	}


//	async void OnSaveClicked(object sender, EventArgs e)
//	{
//		if(BindingContext is not Reminder reminder) return;

//		reminder.Urgency = (Urgency)(UrgencyPicker.SelectedItem ?? Urgency.Medium);

//		var date = ReminderDatePicker.Date;
//		var time = ReminderTimePicker.Time;
//		reminder.ReminderDate = date + time;

//		await App.Database.SaveReminderAsync(reminder);
//		await Shell.Current.GoToAsync("//Reminders");
//	}

//	async void OnDeleteClicked(object sender, EventArgs e)
//	{
//		if(BindingContext is not Reminder reminder) return;

//		bool confirm = await DisplayAlert("Delete", "Delete this reminder?", "Yes", "No");
//		if(!confirm) return;

//		await App.Database.DeleteReminderAsync(reminder);

//		await Shell.Current.GoToAsync("//Reminders");
//	}
//}

