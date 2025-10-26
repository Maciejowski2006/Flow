using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Flow.Player.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainWindowViewModel();
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		
		PlayerViewModel vm = App.AppServices.GetRequiredService<PlayerViewModel>();
		switch (e.Key)
		{
			case Key.Space or Key.MediaPlayPause:
				vm.IsPlaying = !vm.IsPlaying;
				break;
			case Key.Right:
				vm.Seek(5);
				break;
			case Key.Left:
				vm.Seek(-5);
				break;
		}
	}

	private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			return;

		Visual? element = e.Source as Visual;
		while (element != null)
		{
			if (element is MenuItem)
				return;
				
			if (element == sender)
				break;
				
			element = element.Parent as Visual;
		}
        
		BeginMoveDrag(e);
		e.Handled = true;
	}
	private void VolumeSliderScroll(object? sender, PointerWheelEventArgs e)
	{
		PlayerViewModel vm = App.AppServices.GetRequiredService<PlayerViewModel>();

		switch (e.Delta.Y)
		{
			case > 0:
				vm.Volume += 5;
				break;
			case < 0:
				vm.Volume -= 5;
				break;
		}
	}
}