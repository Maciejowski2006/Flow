using Avalonia;
using System;
using Avalonia.Controls.Shapes;
using Microsoft.Win32;
using Velopack;
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
		VelopackApp.Build().OnFirstRun(v => RegisterAsMusicPlayerApp()).Run();

		BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
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