using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.IO;

namespace Revit_AIPlugin.Logging
{
    /// <summary>
    /// Servicio centralizado de logging para toda la aplicación Revit AI.
    /// Usa Serilog para estructurado, multi-destino y fácil filtrado.
    /// </summary>
    public static class RevitAILogger
    {
        private static ILogger _logger;
        private static readonly object _lock = new object();

        /// <summary>
        /// Inicializa el logger una sola vez (Singleton).
        /// Se llama desde App.xaml.cs al startup.
        /// </summary>
        public static void Initialize(string logDirectory = null)
        {
            if (_logger != null)
                return; // Ya inicializado

            lock (_lock)
            {
                if (_logger != null)
                    return;

                // Ruta de logs: %LocalAppData%\RevitAIPlugin\logs\
                logDirectory ??= Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RevitAIPlugin", "logs");

                Directory.CreateDirectory(logDirectory);

                _logger = new LoggerConfiguration()
                    // Nivel mínimo: Information (excluye Debug en producción)
                    .MinimumLevel.Information()

                    // Console: solo en desarrollo
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")

                    // Archivo diario: logs/revit-ai-2025-01-15.log
                    .WriteTo.File(
                        path: Path.Combine(logDirectory, "revit-ai-.log"),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                        fileSizeLimitBytes: 10_485_760, // 10MB
                        retainedFileCountLimit: 7) // Mantén últimos 7 días

                    // Archivo de errores solamente: logs/errors-2025-01-15.log
                    .WriteTo.File(
                        path: Path.Combine(logDirectory, "errors-.log"),
                        rollingInterval: RollingInterval.Day,
                        restrictedToMinimumLevel: LogEventLevel.Error,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {Message:lj}{NewLine}{Exception}",
                        fileSizeLimitBytes: 5_242_880) // 5MB

                    // Enriquece logs con contexto
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "RevitAIPlugin")

                    .CreateLogger();

                // Log del startup
                _logger.Information("═══════════════════════════════════════");
                _logger.Information("RevitAI Logger inicializado exitosamente");
                _logger.Information("Directorio de logs: {LogDirectory}", logDirectory);
                _logger.Information("═══════════════════════════════════════");
            }
        }

        /// <summary>
        /// Obtiene el logger (lazy-initialized).
        /// </summary>
        public static ILogger Get() => _logger ?? throw new InvalidOperationException(
            "Logger no inicializado. Llama a RevitAILogger.Initialize() primero.");

        /// <summary>
        /// Alias rápido para información general.
        /// </summary>
        public static void Info(string message, params object[] args) =>
            Get().Information(message, args);

        /// <summary>
        /// Alias rápido para warnings (situaciones atípicas pero recuperables).
        /// </summary>
        public static void Warn(string message, params object[] args) =>
            Get().Warning(message, args);

        /// <summary>
        /// Alias rápido para errores con excepción.
        /// </summary>
        public static void Error(Exception ex, string message, params object[] args) =>
            Get().Error(ex, message, args);

        /// <summary>
        /// Alias para debug (solo en desarrollo).
        /// </summary>
        public static void Debug(string message, params object[] args) =>
            Get().Debug(message, args);

        /// <summary>
        /// Cierra y flushea los logs (llamar al shutdown).
        /// </summary>
        public static void Close()
        {
            _logger?.Information("Cerrando RevitAI Logger...");
            Serilog.Log.CloseAndFlush();
        }
    }
}
