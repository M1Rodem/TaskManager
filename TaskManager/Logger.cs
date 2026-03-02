using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TaskManager
{
    public class Logger
    {
        public Logger()
        {
            string dateTimeFolder = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string logDirectory = Path.Combine("logs", dateTimeFolder);
            Directory.CreateDirectory(logDirectory);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(ev => ev.Level == LogEventLevel.Debug)
                    .WriteTo.File(
                        path: Path.Combine(logDirectory, "log_DBG.log"),
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message:lj}{NewLine}{Exception}",
                        encoding: Encoding.UTF8
                    ))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(ev => ev.Level == LogEventLevel.Information)
                    .WriteTo.File(
                        path: Path.Combine(logDirectory, "log_INF.log"),
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message:lj}{NewLine}{Exception}",
                        encoding: Encoding.UTF8
                    ))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(ev => ev.Level == LogEventLevel.Warning)
                    .WriteTo.File(
                        path: Path.Combine(logDirectory, "log_WRN.log"),
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message:lj}{NewLine}{Exception}",
                        encoding: Encoding.UTF8
                    ))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(ev => ev.Level >= LogEventLevel.Error)
                    .WriteTo.File(
                        path: Path.Combine(logDirectory, "log_ERR.log"),
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message:lj}{NewLine}{Exception}",
                        encoding: Encoding.UTF8
                    ))
                .CreateLogger();

            //// 2. Пример использования
            //Log.Debug("Отладочное сообщение (Debug).");
            //Log.Information("Приложение запущено (Information).");
            //Log.Warning("Внимание! (Warning)");
            //Log.Error("Ошибка! (Error)");
        }
    }
}