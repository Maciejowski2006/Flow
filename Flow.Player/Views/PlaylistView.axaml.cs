using Avalonia.Controls;
using Flow.Player.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.Views;

public partial class PlaylistView : UserControl
{
	public PlaylistView()
	{
		InitializeComponent();
		DataContext = App.AppServices.GetRequiredService<PlaylistViewModel>();
	}
}
