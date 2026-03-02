using Serilog;
using System;
using System.Diagnostics;
using System.IO;

namespace TaskManager
{
    public class Logger
    {
        public Logger()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()                               // уровень логирования: Debug и выше
            .WriteTo.Console()                                  // пишем логи в консоль
            .WriteTo.File("logs\\myapp-.log",                   // пишем логи в файлы
                rollingInterval: RollingInterval.Day,           // ежедневная ротация
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

            //// 2. Пример использования
            //Log.Debug("Отладочное сообщение (Debug).");
            //Log.Information("Приложение запущено (Information).");
            //Log.Warning("Внимание! (Warning)");
            //Log.Error("Ошибка! (Error)");
        }
    }
}