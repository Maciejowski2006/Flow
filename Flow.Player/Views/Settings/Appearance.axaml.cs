using Avalonia.Controls;
using Flow.Player.ViewModels.Settings;

namespace Flow.Player.Views.Settings;

public partial class Appearance : UserControl
{
	public Appearance()
	{
		InitializeComponent();
		DataContext = new AppearanceViewModel();
	}
}