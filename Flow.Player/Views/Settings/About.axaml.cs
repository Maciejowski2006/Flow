using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Flow.Player.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.Views.Settings;

public partial class About : UserControl
{
	public About()
	{
		InitializeComponent();
		Version.Text = $"Version: {App.AppServices.GetRequiredService<UpdateManagerService>().Version ?? "Not available"}";
	}
}