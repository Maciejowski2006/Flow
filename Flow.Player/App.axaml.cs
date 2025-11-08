using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Flow.Player.Services;
using Flow.Player.ViewModels;
using Flow.Player.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Flow.Player;

public partial class App : Application
{
	public static ServiceProvider Services { get; private set; }
	public static ServiceProvider AppServices { get; private set; }
	
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
#if DEBUG
		this.AttachDeveloperTools();
#endif
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.Exit += OnExit;
			DisableAvaloniaDataAnnotationValidation();
			ServiceCollection appServices = new();
			appServices.AddSingleton<IMediaPlayerService, MediaPlayerService>();
			appServices.AddSingleton<CommandLineArgumentsService>(_ => new(desktop.Args));
			appServices.AddSingleton<UpdateManagerService>();
			appServices.AddSingleton<PlayerViewModel>();
			appServices.AddSingleton<PlaylistViewModel>();
			appServices.AddSingleton<MainWindowViewModel>();
			
			AppServices = appServices.BuildServiceProvider();
			
			desktop.MainWindow = new MainWindow();
			
			ServiceCollection services = new();
			services.AddSingleton<IFilePickerService>(_ => new FilePickerService(desktop.MainWindow));
			
			Services = services.BuildServiceProvider();
		}


		base.OnFrameworkInitializationCompleted();
	}

	private void DisableAvaloniaDataAnnotationValidation()
	{
		// Get an array of plugins to remove
		var dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		// remove each entry found
		foreach (var plugin in dataValidationPluginsToRemove)
		{
			BindingPlugins.DataValidators.Remove(plugin);
		}
	}

	private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
	{
		AppServices.GetRequiredService<UpdateManagerService>().UpdateEndExit();
	}
}