using DAL.UnitOfWork;
using Utils;

namespace Tests
{
    public class TestSettingsHandler : ISettingsHandler
    {
        ISettings Settings = null;
        public TestSettingsHandler(ISettings settings)
        {
            this.Settings = settings;
        }

        public async Task Load()
        {
            Settings.LoggerSettings = "{\"Using\": [ \"Serilog.Sinks.Console\", \"Serilog.Sinks.File\", \"Serilog.Enrichers.Environment\" ],\r\n    \"MinimumLevel\": {\r\n      \"Default\": \"Verbose\",\r\n      \"Override\": {\r\n        \"Microsoft\": \"Warning\",\r\n        \"System\": \"Warning\",\r\n        \"Context.DataAcquisition\": \"Warning\"\r\n      }\r\n    },\r\n    \"WriteTo\": [\r\n      {\r\n        \"Name\": \"Console\",\r\n        \"Args\": {\r\n          \"theme\": \"Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console\",\r\n          \"outputTemplate\": \"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}\"\r\n        }\r\n      },\r\n      {\r\n        \"Name\": \"File\",\r\n        \"Args\": {\r\n          \"path\": \"\\\\Logs\\\\log.log\",\r\n          \"rollingInterval\": \"Day\"\r\n        }\r\n      }\r\n    ],\r\n    \"Enrich\": [ \"FromLogContext\", \"WithMachineName\", \"WithThreadId\" ]}";

            TimeScaleDBSettings db = new TimeScaleDBSettings();
            db.Server = "127.0.0.1";
            db.DatabaseName = "AquariumData";
            db.Username = "admin";
            db.Password = "pass";
            db.Port = 5433;

            Settings.TimeScaleDBSettings = db;

        }
    }
}