using Avalonia;
using System;
using Avalonia.Controls.Shapes;
using Avalonia.Logging;
using Microsoft.Win32;
using Serilog;
using Velopack;
using LogEventLevel = Serilog.Events.LogEventLevel;
using Path = System.IO.Path;

namespace Flow.Player;

sealed class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.MinimumLevel.Override("Microsoft", (LogEventLevel)Avalonia.Logging.LogEventLevel.Information)
			.Enrich.FromLogContext()
			.WriteTo.Console()
			.WriteTo.File(
				path: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Flow", "logs", "app-.log"),
				rollingInterval: RollingInterval.Day,
				outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}",
				retainedFileCountLimit: 30)
			.CreateLogger();

		try
		{
			Log.Information("Starting application");

			VelopackApp.Build().OnFirstRun(v => RegisterAsMusicPlayerApp()).Run();
			BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
		}
		catch (Exception e)
		{
			Log.Fatal(e, "Application terminated unexpectedly");
		}
		finally
		{
			Log.CloseAndFlush();
		}
		
		
	}

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace();
	
	private static void RegisterAsMusicPlayerApp()
	{
		if (!OperatingSystem.IsWindows())
			return;

		string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Flow", "current", "Flow.Player.exe");
		string appName = "Flow";

		using RegistryKey mediaKey = Registry.LocalMachine.CreateSubKey($@"Software\Clients\Media\{appName}");
		mediaKey.SetValue(null, appName);

		using RegistryKey caps = mediaKey.CreateSubKey("Capabilities");
		caps.SetValue("ApplicationName", appName);
		caps.SetValue("ApplicationDescription", appName);

		using RegistryKey assoc = caps.CreateSubKey("FileAssociations");
		string[] formats = ["aac", "flac", "mp3", "ogg", "opus", "oga", "wav", "wma"];

		foreach (string format in formats)
		{
			assoc.SetValue($".{format}", $"{appName}.{format}");
		}

		using (RegistryKey regApps = Registry.LocalMachine.CreateSubKey(@"Software\RegisteredApplications"))
		{
			regApps.SetValue(appName, $@"Software\Clients\Media\{appName}\Capabilities");
		}

		foreach (string format in formats)
		{
			using RegistryKey key = Registry.ClassesRoot.CreateSubKey($"{appName}.{format}\\shell\\open\\command");
			key.SetValue(null, $"\"{appPath}\" \"%1\"");
		}
	}
}