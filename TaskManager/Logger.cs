using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TaskManager
{
    public class Logger
    {
        private static bool _initialized = false;
        private static readonly object _lock = new object();
        private static string _logDirectory = null!;

        public Logger()
        {
            lock (_lock)
            {
                if (_initialized) return;

                string dateTimeFolder = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                _logDirectory = Path.Combine("logs", dateTimeFolder);
                Directory.CreateDirectory(_logDirectory);

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(new JsonFormatter())
                    .WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(ev => ev.Level == LogEventLevel.Debug)
                        .WriteTo.File(
                            path: Path.Combine(_logDirectory, "log_DBG.json"),
                            formatter: new JsonFormatter(),
                            rollingInterval: RollingInterval.Day,
                            fileSizeLimitBytes: 10_485_760,
                            rollOnFileSizeLimit: true,
                            encoding: Encoding.UTF8
                        ))
                    .WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(ev => ev.Level == LogEventLevel.Information)
                        .WriteTo.File(
                            path: Path.Combine(_logDirectory, "log_INF.json"),
                            formatter: new JsonFormatter(),
                            rollingInterval: RollingInterval.Day,
                            fileSizeLimitBytes: 10_485_760,
                            rollOnFileSizeLimit: true,
                            encoding: Encoding.UTF8
                        ))
                    .WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(ev => ev.Level == LogEventLevel.Warning)
                        .WriteTo.File(
                            path: Path.Combine(_logDirectory, "log_WRN.json"),
                            formatter: new JsonFormatter(),
                            rollingInterval: RollingInterval.Day,
                            fileSizeLimitBytes: 10_485_760,
                            rollOnFileSizeLimit: true,
                            encoding: Encoding.UTF8
                        ))
                    .WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(ev => ev.Level >= LogEventLevel.Error)
                        .WriteTo.File(
                            path: Path.Combine(_logDirectory, "log_ERR.json"),
                            formatter: new JsonFormatter(),
                            rollingInterval: RollingInterval.Day,
                            fileSizeLimitBytes: 10_485_760,
                            rollOnFileSizeLimit: true,
                            encoding: Encoding.UTF8
                        ))
                    .WriteTo.File(
                        path: Path.Combine(_logDirectory, "taskmanager.log"),
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: 10_485_760,
                        rollOnFileSizeLimit: true,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        encoding: Encoding.UTF8
                    )
                    .CreateLogger();

                Tracer.TaskManagerTrace.Listeners.Clear();
                Tracer.TaskManagerTrace.Switch = new SourceSwitch("TaskManagerSwitch", "Verbose");

                var consoleListener = new ConsoleTraceListener();
                consoleListener.Filter = new EventTypeFilter(SourceLevels.Verbose);
                Tracer.TaskManagerTrace.Listeners.Add(consoleListener);

                string traceLogPath = Path.Combine(_logDirectory, "taskmanager_trace.log");
                var fileListener = new TextWriterTraceListener(traceLogPath);
                fileListener.Filter = new EventTypeFilter(SourceLevels.Verbose);
                Tracer.TaskManagerTrace.Listeners.Add(fileListener);

                Trace.AutoFlush = true;

                Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Information, 0, "=== ТРАССИРОВКА ВКЛЮЧЕНА ===");
                Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Verbose, 0, "Трассировка инициализирована и работает");

                _initialized = true;
            }
        }
        
        public static string GetLogDirectory()
        {
            return _logDirectory;
        }
    }
}