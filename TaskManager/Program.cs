using Serilog;
using System;
using System.Diagnostics;

namespace TaskManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger();

            var taskManager = new TaskManager();

            Log.Information("TaskManager запущен");

            Console.WriteLine("=== TASK MANAGER ===");

            bool isRunning = true;
            while (isRunning)
            {
                ShowMenu();
                Console.Write("Выберите действие (1-4): ");

                string input = Console.ReadLine()?.Trim() ?? "";
                Log.Debug($"Пользователь ввел: {input}");

                switch (input)
                {
                    case "1":
                        AddTask(taskManager);
                        break;
                    case "2":
                        RemoveTask(taskManager);
                        break;
                    case "3":
                        ListTasks(taskManager);
                        break;
                    case "4":
                        Log.Information("Приложение завершает работу");
                        Console.WriteLine("Выход...");
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Введите число от 1 до 4.");
                        Log.Warning($"Неверный ввод: {input}");
                        break;
                }
            }

            Log.CloseAndFlush();
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Добавить задачу");
            Console.WriteLine("2. Удалить задачу");
            Console.WriteLine("3. Показать список задач");
            Console.WriteLine("4. Выход");
        }

        static void AddTask(TaskManager taskManager)
        {
            Log.Debug("Начало операции AddTask");

            Console.Write("Введите название задачи: ");
            string title = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Ошибка: название задачи не может быть пустым!");
                Log.Warning("Попытка добавить задачу с пустым названием");
                Log.Debug("Конец операции AddTask (неудача)");
                return;
            }

            taskManager.AddTask(title);
            Console.WriteLine($"Задача '{title}' добавлена.");
            Log.Information($"Задача '{title}' добавлена");
            Log.Debug("Конец операции AddTask (успех)");
        }

        static void RemoveTask(TaskManager taskManager)
        {
            Log.Debug("Начало операции RemoveTask");

            Console.Write("Введите название задачи для удаления: ");
            string title = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Ошибка: название не может быть пустым!");
                Log.Warning("Конец операции RemoveTask (неудача)");
                return;
            }

            if (taskManager.RemoveTask(title))
            {
                Console.WriteLine($"Задача '{title}' удалена.");
                Log.Information($"Задача '{title}' удалена");
                Log.Debug("Конец операции RemoveTask (успех)");
            }
            else
            {
                Console.WriteLine($"Задача '{title}' не найдена.");
                Log.Error($"Задача '{title}' не найдена для удаления");
                Log.Debug("Конец операции RemoveTask (неудача)");
            }
        }

        static void ListTasks(TaskManager taskManager)
        {
            Log.Debug("Начало операции ListTasks");

            var tasks = taskManager.GetTasks();

            if (tasks.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                Log.Information("Список задач пуст");
            }
            else
            {
                Console.WriteLine("\nСписок задач:");
                for (int i = 0; i < tasks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {tasks[i].Title}");
                }
                Console.WriteLine($"Всего задач: {tasks.Count}");
                Log.Information($"Выведено {tasks.Count} задач");
            }
            Log.Debug("Конец операции ListTasks");
        }
    }
}