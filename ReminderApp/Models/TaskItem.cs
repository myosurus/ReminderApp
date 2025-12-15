namespace ReminderApp.Models;

// Вспомогательный класс для отображения задач
public class TaskItem
{
    public int Id { get; set; }   //добавлено
    public string Name { get; set; }
	public string Time { get; set; }
	public Color Color { get; set; }
}
