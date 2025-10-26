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
        DeleteButton.IsVisible = reminder.Id != 0; // 🔹 Показываем кнопку только для существующих задач
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
        //await App.Database.SaveReminderAsync(reminder);
        await App.Database.CreateReminderAsync(reminder);

        // Показываем сообщение об успехе
        await DisplayAlert("Успех", "Задача успешно сохранена", "OK");

        // 🔹 Очищаем все поля после сохранения (add)
        ClearForm();

        // Возвращаемся на список задач
        await Shell.Current.GoToAsync("//Reminders");     
    }

    private void ClearForm() //add
    {
        // Создаём новый объект и привязываем к контексту
        BindingContext = new Reminder
        {
            ReminderDate = DateTime.Now,
            IsDone = false
        };

        // Сбрасываем визуальные элементы
        UrgencyPicker.SelectedItem = null;
        ReminderDatePicker.Date = DateTime.Now;
        ReminderTimePicker.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
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

