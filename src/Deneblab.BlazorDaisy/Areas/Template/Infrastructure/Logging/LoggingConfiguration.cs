using NLog;
using NLog.Targets;
using NLog.Web;
using LogLevel = NLog.LogLevel;
using NLogConfig = NLog.Config.LoggingConfiguration;

namespace Deneblab.BlazorDaisy.Infrastructure.Logging;

/// <summary>
///     NLog configuration for structured logging.
/// </summary>
public static class LoggingSetup
{
    /// <summary>
    ///     Configures NLog with console and file targets.
    /// </summary>
    public static void Configure(WebApplicationBuilder builder, string? logDirectory = null)
    {
        var config = new NLogConfig();

        // Default log directory
        logDirectory ??= Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDirectory);

        // Console target
        var consoleTarget = new ConsoleTarget("console")
        {
            Layout =
                "${longdate}|${level:uppercase=true}|${logger:shortName=true}|${message} ${exception:format=tostring}"
        };
        config.AddTarget(consoleTarget);

        // File target - daily rolling
        var fileTarget = new FileTarget("file")
        {
            FileName = Path.Combine(logDirectory, "app-${shortdate}.log"),
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}",
            KeepFileOpen = false,
            AutoFlush = true,
            ArchiveEvery = FileArchivePeriod.Day,
            ArchiveNumbering = ArchiveNumberingMode.Date,
            MaxArchiveFiles = 30
        };
        config.AddTarget(fileTarget);

        // Error file target - separate file for errors
        var errorFileTarget = new FileTarget("errorFile")
        {
            FileName = Path.Combine(logDirectory, "error-${shortdate}.log"),
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}",
            KeepFileOpen = false,
            AutoFlush = true
        };
        config.AddTarget(errorFileTarget);

        // Null target to discard unwanted logs
        var nullTarget = new NullTarget("blackhole");
        config.AddTarget(nullTarget);

        // Rules - order matters! More specific rules first

        // Discard Debug/Info from noisy Microsoft loggers
        config.AddRule(LogLevel.Trace, LogLevel.Info, nullTarget, "Microsoft.AspNetCore.*", true);
        config.AddRule(LogLevel.Trace, LogLevel.Info, nullTarget, "Microsoft.EntityFrameworkCore.*", true);
        config.AddRule(LogLevel.Trace, LogLevel.Info, nullTarget, "Microsoft.WebTools.BrowserLink.*", true);

        // General rules
        config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);
        config.AddRule(LogLevel.Error, LogLevel.Fatal, errorFileTarget);

        LogManager.Configuration = config;

        // Configure ASP.NET Core logging
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();
    }
}