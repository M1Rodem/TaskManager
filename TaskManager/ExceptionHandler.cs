using Serilog;
using Serilog.Events;  
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace TaskManager
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Главный метод обработки исключений
        /// </summary>
        /// <param name="ex">Пойманное исключение</param>
        /// <param name="operationContext">Название операции, где произошла ошибка</param>
        /// <param name="level">Уровень критичности (Error или Fatal)</param>
        public static void HandleException(Exception ex, string operationContext, LogEventLevel level = LogEventLevel.Error)
        {
            bool isCritical = (level == LogEventLevel.Fatal) || IsCriticalException(ex);

            string errorMessage = $"ОШИБКА в операции '{operationContext}': {ex.Message}";

            if (isCritical)
            {
                Log.Fatal(ex, "КРИТИЧЕСКАЯ ОШИБКА в {Operation}: {ErrorMessage}", operationContext, ex.Message);
            }
            else
            {
                Log.Error(ex, "Ошибка в {Operation}: {ErrorMessage}", operationContext, ex.Message);
            }

            Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Error, 3,
                $"Операция: {operationContext}\n" +
                $"Ошибка: {ex.Message}\n" +
                $"Стек вызовов:\n{ex.StackTrace}");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n!!! ОШИБКА !!!");
            Console.WriteLine($"Операция: {operationContext}");
            Console.WriteLine($"Описание: {ex.Message}");
            Console.ResetColor();

            if (isCritical)
            {
                NotifyCriticalError(ex, operationContext);
            }
        }

        /// <summary>
        /// Определяем, является ли исключение критическим (системным)
        /// </summary>
        private static bool IsCriticalException(Exception ex)
        {
            return ex is OutOfMemoryException ||
                   ex is StackOverflowException ||
                   ex is AccessViolationException ||
                   ex is NullReferenceException ||
                   ex is IOException;
        }

        /// <summary>
        /// Отправка уведомлений о критических ошибках
        /// </summary>
        private static void NotifyCriticalError(Exception ex, string operationContext)
        {
            Log.Warning("Отправка оповещения о критической ошибке...");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"\n[КРИТИЧЕСКАЯ ОШИБКА] Отправлено оповещение администратору!");
            Console.ResetColor();

            // 6.3 Получаем путь к директории логов
            string logDir = Logger.GetLogDirectory();
            if (!string.IsNullOrEmpty(logDir))
            {
                // 6.4 Записываем в специальный файл критических ошибок для аудита
                string criticalLogPath = Path.Combine(logDir, "CRITICAL_ERRORS.log");
                try
                {
                    File.AppendAllText(criticalLogPath,
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] КРИТИЧЕСКАЯ ОШИБКА\n" +
                        $"Операция: {operationContext}\n" +
                        $"Ошибка: {ex.Message}\n" +
                        $"Стек: {ex.StackTrace}\n" +
                        $"{new string('-', 80)}\n");
                }
                catch (Exception fileEx)
                {
                    Log.Error(fileEx, "Не удалось записать критическую ошибку в файл");
                }
            }
            // SendEmailNotification(ex, operationContext);
        }
        /// <summary>
        /// Метод для отправки email о критической ошибке
        /// </summary>
        private static void SendEmailNotification(Exception ex, string operationContext)
        {
            try
            {
                string fromEmail = "your-email@gmail.com";

                string toEmail = "admin@yourcompany.com";

                string smtpServer = "smtp.gmail.com";

                int smtpPort = 587;

                string password = "your-app-password";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail);
                    mail.To.Add(toEmail);
                    mail.Subject = $"[КРИТИЧЕСКАЯ ОШИБКА] TaskManager - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    mail.Body = $"Произошла критическая ошибка в TaskManager:\n\n" +
                               $"Время: {DateTime.Now}\n" +
                               $"Операция: {operationContext}\n" +
                               $"Ошибка: {ex.Message}\n\n" +
                               $"Стек вызовов:\n{ex.StackTrace}\n\n" +
                               $"Тип исключения: {ex.GetType().Name}";
                    mail.Body += $"\n\nДополнительная информация:\n" +
                                $"OS: {Environment.OSVersion}\n" +
                                $".NET Version: {Environment.Version}\n" +
                                $"Machine: {Environment.MachineName}";
                    mail.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(fromEmail, password);
                        smtp.EnableSsl = true;  // Для Gmail обязательно true
                        smtp.Timeout = 10000;    // Таймаут 10 секунд
                        smtp.Send(mail);

                        Log.Information("Email уведомление отправлено на {Email}", toEmail);
                    }
                }
            }
            catch (Exception emailEx)
            {
                Log.Error(emailEx, "Не удалось отправить email уведомление о критической ошибке");
            }
        }

        /// <summary>
        /// Обёртка для безопасного выполнения операций (без возврата значения)
        /// </summary>
        public static void ExecuteWithHandling(string operationName, Action action, LogEventLevel level = LogEventLevel.Error)
        {
            try
            {
                Log.Debug("Начало операции: {Operation}", operationName);
                action();
                Log.Debug("Успешное завершение операции: {Operation}", operationName);
            }
            catch (Exception ex)
            {
                HandleException(ex, operationName, level);
            }
        }

        /// <summary>
        /// Обёртка для безопасного выполнения операций (с возвратом значения)
        /// </summary>
        public static T ExecuteWithHandling<T>(string operationName, Func<T> func, T? defaultValue = default, LogEventLevel level = LogEventLevel.Error)
        {
            try
            {
                Log.Debug("Начало операции: {Operation}", operationName);
                T result = func();
                Log.Debug("Успешное завершение операции: {Operation}", operationName);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex, operationName, level);
                return defaultValue!;
            }
        }   
    }
}