using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;

namespace TaskManager
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                ExceptionHandler.HandleException((Exception)e.ExceptionObject,
                    "Необработанное исключение домена",
                    LogEventLevel.Fatal);
            };

            try
            {
                var logger = new Logger();
                var taskManager = new TaskManager();

                Log.Information("=== TaskManager ЗАПУЩЕН ===");
                Log.Information("Директория логов: {LogDirectory}", Logger.GetLogDirectory());

                Console.WriteLine("=== TASK MANAGER ===");
                Console.WriteLine($"Логи сохраняются в: {Logger.GetLogDirectory()}");
                Console.WriteLine("Логи пишутся в JSON формате (структурированные)");

                bool isRunning = true;
                while (isRunning)
                {
                    try
                    {
                        ShowMenu();
                        Console.Write("Выберите действие (1-4): ");

                        string input = Console.ReadLine()?.Trim() ?? "";
                        Log.Debug("Пользовательский ввод: {UserInput}", input);

                        switch (input)
                        {
                            case "1":
                                ExceptionHandler.ExecuteWithHandling(
                                    "Добавление задачи",
                                    () => AddTask(taskManager)
                                );
                                break;

                            case "2":
                                ExceptionHandler.ExecuteWithHandling(
                                    "Удаление задачи",
                                    () => RemoveTask(taskManager)
                                );
                                break;

                            case "3":
                                ExceptionHandler.ExecuteWithHandling(
                                    "Просмотр списка задач",
                                    () => ListTasks(taskManager)
                                );
                                break;

                            case "4":
                                Log.Information("Программа завершается по запросу пользователя");
                                Console.WriteLine("Выход...");
                                isRunning = false;
                                break;

                            default:
                                Console.WriteLine("Неверный выбор. Введите число от 1 до 4.");
                                Log.Warning("Неверный ввод в меню: {UserInput}", input);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.HandleException(ex, "Обработка команды меню", LogEventLevel.Error);
                        Console.WriteLine("\nПроизошла ошибка. Подробнее см. логи.");
                        Console.WriteLine("Нажмите Enter для продолжения...");
                        Console.ReadLine();
                    }

                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, "Инициализация приложения", LogEventLevel.Fatal);
                Console.WriteLine("\nКритическая ошибка при запуске. Подробнее см. логи.");
                Console.WriteLine("Нажмите Enter для выхода...");
                Console.ReadLine();
            }
            finally
            {
                Log.Information("=== TaskManager ЗАВЕРШЕН ===");
                Log.CloseAndFlush();
                
                Console.WriteLine("\nНажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\n═══════════════════════════════");
            Console.WriteLine("           МЕНЮ");
            Console.WriteLine("═══════════════════════════════");
            Console.WriteLine("1. Добавить задачу");
            Console.WriteLine("2. Удалить задачу");
            Console.WriteLine("3. Показать список задач");
            Console.WriteLine("4. Выход");
            Console.WriteLine("═══════════════════════════════");
        }

        static void AddTask(TaskManager taskManager)
        {
            Console.Write("Введите название задачи: ");
            string title = Console.ReadLine()?.Trim() ?? "";

            taskManager.AddTask(title);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Задача '{title}' успешно добавлена.");
            Console.ResetColor();
        }

        static void RemoveTask(TaskManager taskManager)
        {
            Console.Write("Введите название задачи для удаления: ");
            string title = Console.ReadLine()?.Trim() ?? "";

            if (taskManager.RemoveTask(title))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Задача '{title}' удалена.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Задача '{title}' не найдена.");
                Console.ResetColor();
            }
        }

        static void ListTasks(TaskManager taskManager)
        {
            var tasks = taskManager.GetTasks();

            if (tasks.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
            }
            else
            {
                Console.WriteLine("\nСПИСОК ЗАДАЧ:");
                Console.WriteLine("═══════════════════════════════");
                for (int i = 0; i < tasks.Count; i++)
                {
                    Console.WriteLine($"{i + 1,3}. {tasks[i].Title}");
                }
                Console.WriteLine("═══════════════════════════════");
                Console.WriteLine($"Всего задач: {tasks.Count}");
            }
        }
    }
}