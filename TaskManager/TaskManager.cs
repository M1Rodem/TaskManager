using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Serilog;

namespace TaskManager
{
    public class TaskManager
    {
        private List<Task> tasks = new List<Task>();

        public void AddTask(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Log.Error("Попытка добавить задачу с пустым названием");
                throw new ArgumentException("Название задачи не может быть пустым или состоять только из пробелов", nameof(title));
            }

            if (title.Length > 100)
            {
                Log.Error("Название задачи превышает 100 символов: {TitleLength}", title.Length);
                throw new ArgumentException("Название задачи не может превышать 100 символов", nameof(title));
            }

            if (tasks.Any(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            {
                Log.Warning("Попытка добавить дубликат задачи: {TaskTitle}", title);
                throw new InvalidOperationException($"Задача с названием '{title}' уже существует");
            }
            
            Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Start, 0, $"AddTask: '{title}'");
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                var task = new Task(title);
                tasks.Add(task);
                
                Log.Information("Задача добавлена: {TaskTitle}, {@Task}", title, task);
                
                sw.Stop();
                Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Stop, 1, 
                    $"AddTask завершена. Задача: '{title}'. Время: {sw.ElapsedMilliseconds} мс");
            }
            catch (Exception ex)
            {
                sw.Stop();
                Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Error, 1, 
                    $"AddTask ошибка: {ex.Message}. Время: {sw.ElapsedMilliseconds} мс");
                Log.Error(ex, "Ошибка при добавлении задачи: {TaskTitle}", title);
                throw;
            }
        }

        public bool RemoveTask(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Log.Error("Попытка удалить задачу с пустым названием");
                throw new ArgumentException("Название задачи не может быть пустым или состоять только из пробелов", nameof(title));
            }

            Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Start, 0, $"RemoveTask: '{title}'");
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                var task = tasks.FirstOrDefault(t => t.Title == title);
                if (task != null)
                {
                    tasks.Remove(task);
                    sw.Stop();
                    
                    Log.Information("Задача удалена: {TaskTitle}", title);
                    Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Stop, 1, 
                        $"RemoveTask успешна. Задача: '{title}'. Время: {sw.ElapsedMilliseconds} мс");
                    return true;
                }

                sw.Stop();
                Log.Warning("Задача не найдена для удаления: {TaskTitle}", title);
                Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Stop, 1, 
                    $"RemoveTask: задача не найдена. Время: {sw.ElapsedMilliseconds} мс");
                return false;
            }
            catch (Exception ex)
            {
                sw.Stop();
                Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Error, 1, 
                    $"RemoveTask ошибка: {ex.Message}. Время: {sw.ElapsedMilliseconds} мс");
                Log.Error(ex, "Ошибка при удалении задачи: {TaskTitle}", title);
                throw;
            }
        }

        public List<Task> GetTasks()
        {
            Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Start, 0, "GetTasks");
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                var result = new List<Task>(tasks);
                sw.Stop();
                
                Log.Information("Показан список задач: {Count} задач", result.Count);
                Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Stop, 1, 
                    $"GetTasks завершена. Найдено задач: {result.Count}. Время: {sw.ElapsedMilliseconds} мс");
                
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Error, 1, 
                    $"GetTasks ошибка: {ex.Message}. Время: {sw.ElapsedMilliseconds} мс");
                Log.Error(ex, "Ошибка при получении списка задач");
                throw;
            }
        }
    }
}