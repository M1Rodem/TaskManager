using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TaskManager
{
    public class TaskManager
    {
        private List<Task> tasks = new List<Task>();

        public void AddTask(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название задачи не может быть пустым или состоять только из пробелов", nameof(title));

            if (title.Length > 100)
                throw new ArgumentException("Название задачи не может превышать 100 символов", nameof(title));

            if (tasks.Any(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Задача с названием '{title}' уже существует");

            Stopwatch sw = Stopwatch.StartNew();

            Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Information, 0, $"Начало AddTask: '{title}'");
            Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Verbose, 0, $"Добавляется задача: {title}");

            tasks.Add(new Task(title));

            sw.Stop();
            Tracer.TaskManagerTrace.TraceEvent(
                TraceEventType.Information,
                1,
                $"Завершение AddTask. Время: {sw.ElapsedMilliseconds} мс"
            );
        }

        public bool RemoveTask(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название задачи не может быть пустым или состоять только из пробелов", nameof(title));

            Stopwatch sw = Stopwatch.StartNew();
            Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Information, 0, $"Начало RemoveTask: '{title}'");

            var task = tasks.FirstOrDefault(t => t.Title == title);
            if (task != null)
            {
                tasks.Remove(task);
                sw.Stop();
                Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Information, 1, $"Завершение RemoveTask (успех). Время: {sw.ElapsedMilliseconds} мс");
                return true;
            }

            sw.Stop();
            Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Information, 1, $"Завершение RemoveTask (не найдено). Время: {sw.ElapsedMilliseconds} мс");
            return false;
        }

        public List<Task> GetTasks()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Information, 0, "Начало GetTasks");

            var result = new List<Task>(tasks);

            sw.Stop();
            Tracer.TaskManagerTrace.TraceEvent(
                TraceEventType.Information,
                1,
                $"Завершение GetTasks. Найдено задач: {result.Count}. Время: {sw.ElapsedMilliseconds} мс"
            );

            return result;
        }
    }
}