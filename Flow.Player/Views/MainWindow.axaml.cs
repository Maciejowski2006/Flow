using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using Flow.Player.Messages;
using Flow.Player.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.Views;

public partial class MainWindow : Window
{
	private readonly MainWindowViewModel _viewModel = new();
	public MainWindow()
	{
		InitializeComponent();
		DataContext = _viewModel;
		
		WeakReferenceMessenger.Default.Register<MainWindow, ShowDialogMessage>(this, static (w, m) =>
		{
			MessageBoxWindow dialog = new(m.Message, m.Buttons);
			m.Reply(dialog.ShowDialog<MessageBoxReturn?>(w));
		});
	}
	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);
		_viewModel.CheckForUpdatesCommand.Execute(null);
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
				vm.Seek(TimeSpan.FromSeconds(5));
				break;
			case Key.Left:
				vm.Seek(TimeSpan.FromSeconds(-5));
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
				vm.Volume += .05f;
				break;
			case < 0:
				vm.Volume -= .05f;
				break;
		}
	}
	private void Exit_Click(object? sender, RoutedEventArgs e)
	{
		Close();
	}
}