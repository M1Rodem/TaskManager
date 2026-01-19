using System;

namespace TaskManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger();
            var taskManager = new TaskManager();

            logger.Info("TaskManager запущен");

            Console.WriteLine("=== TASK MANAGER ===");

            bool isRunning = true;
            while (isRunning)
            {
                ShowMenu();
                Console.Write("Выберите действие (1-4): ");

                string input = Console.ReadLine()?.Trim() ?? "";
                logger.Trace($"Пользователь ввел: {input}");

                switch (input)
                {
                    case "1":
                        AddTask(taskManager, logger);
                        break;
                    case "2":
                        RemoveTask(taskManager, logger);
                        break;
                    case "3":
                        ListTasks(taskManager, logger);
                        break;
                    case "4":
                        logger.Info("Приложение завершает работу");
                        Console.WriteLine("Выход...");
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Введите число от 1 до 4.");
                        logger.Warning($"Неверный ввод: {input}");
                        break;
                }
            }

            logger.Close();
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Добавить задачу");
            Console.WriteLine("2. Удалить задачу");
            Console.WriteLine("3. Показать список задач");
            Console.WriteLine("4. Выход");
        }

        static void AddTask(TaskManager taskManager, Logger logger)
        {
            logger.Trace("Начало операции AddTask");

            Console.Write("Введите название задачи: ");
            string title = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Ошибка: название задачи не может быть пустым!");
                logger.Warning("Попытка добавить задачу с пустым названием");
                logger.Trace("Конец операции AddTask (неудача)");
                return;
            }

            taskManager.AddTask(title);
            Console.WriteLine($"Задача '{title}' добавлена.");
            logger.Info($"Задача '{title}' добавлена");
            logger.Trace("Конец операции AddTask (успех)");
        }

        static void RemoveTask(TaskManager taskManager, Logger logger)
        {
            logger.Trace("Начало операции RemoveTask");

            Console.Write("Введите название задачи для удаления: ");
            string title = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Ошибка: название не может быть пустым!");
                logger.Warning("Попытка удалить задачу с пустым названием");
                logger.Trace("Конец операции RemoveTask (неудача)");
                return;
            }

            if (taskManager.RemoveTask(title))
            {
                Console.WriteLine($"Задача '{title}' удалена.");
                logger.Info($"Задача '{title}' удалена");
                logger.Trace("Конец операции RemoveTask (успех)");
            }
            else
            {
                Console.WriteLine($"Задача '{title}' не найдена.");
                logger.Error($"Задача '{title}' не найдена для удаления");
                logger.Trace("Конец операции RemoveTask (неудача)");
            }
        }

        static void ListTasks(TaskManager taskManager, Logger logger)
        {
            logger.Trace("Начало операции ListTasks");

            var tasks = taskManager.GetTasks();

            if (tasks.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                logger.Info("Список задач пуст");
            }
            else
            {
                Console.WriteLine("\nСписок задач:");
                for (int i = 0; i < tasks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {tasks[i].Title}");
                }
                Console.WriteLine($"Всего задач: {tasks.Count}");
                logger.Info($"Выведено {tasks.Count} задач");
            }

            logger.Trace("Конец операции ListTasks");
        }
    }
}