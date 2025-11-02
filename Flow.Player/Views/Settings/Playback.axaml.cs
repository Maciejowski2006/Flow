using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Flow.Player.ViewModels.Settings;

namespace Flow.Player.Views.Settings;

public partial class Playback : UserControl
{
	public Playback()
	{
		InitializeComponent();
		DataContext = new PlaybackViewModel();
	}
}