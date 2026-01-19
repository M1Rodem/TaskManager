using System;
using System.Diagnostics;
using System.IO;

namespace TaskManager
{
    public class Logger
    {
        private TraceSource traceSource;

        public Logger()
        {
            traceSource = new TraceSource("TaskManager");
            SetupListeners();
        }

        private void SetupListeners()
        {
            // Очищаем слушатели
            traceSource.Listeners.Clear();

            // Добавляем консольный слушатель
            traceSource.Listeners.Add(new ConsoleTraceListener());

            // Добавляем файловый слушатель для логов
            var logListener = new TextWriterTraceListener("taskmanager.log");
            traceSource.Listeners.Add(logListener);

            // Добавляем текстовый файл для данных
            var dataListener = new TextWriterTraceListener("tasks.txt");
            traceSource.Listeners.Add(dataListener);

            traceSource.Switch = new SourceSwitch("TaskManagerSwitch", "All");
        }

        public void Info(string message)
        {
            traceSource.TraceEvent(TraceEventType.Information, 0, $"[INFO] {DateTime.Now:HH:mm:ss} {message}");
            traceSource.Flush();
        }

        public void Warning(string message)
        {
            traceSource.TraceEvent(TraceEventType.Warning, 0, $"[WARN] {DateTime.Now:HH:mm:ss} {message}");
            traceSource.Flush();
        }

        public void Error(string message)
        {
            traceSource.TraceEvent(TraceEventType.Error, 0, $"[ERROR] {DateTime.Now:HH:mm:ss} {message}");
            traceSource.Flush();
        }

        public void Trace(string message)
        {
            traceSource.TraceEvent(TraceEventType.Verbose, 0, $"[TRACE] {DateTime.Now:HH:mm:ss} {message}");
            traceSource.Flush();
        }

        public void Close()
        {
            traceSource.Close();
        }
    }
}