using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using Flow.Player.Messages;
using Flow.Player.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.Views;

public partial class PlayerView : UserControl
{
	private Slider? _slider;
	public PlayerView()
	{
		InitializeComponent();
		DataContext = App.AppServices.GetRequiredService<PlayerViewModel>();
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);

		_slider = this.FindControl<Slider>("SeekSlider");
		if (_slider == null)
			return;
		
		_slider.AddHandler(PointerPressedEvent, OnSeekBarPointerPressed, RoutingStrategies.Tunnel);
		_slider.AddHandler(PointerReleasedEvent, OnSeekBarPointerReleased, RoutingStrategies.Tunnel);
	}

	protected override void OnSizeChanged(SizeChangedEventArgs e)
	{
		base.OnSizeChanged(e);

		CoverArt.IsVisible = e.NewSize.Width > CoverArt.Bounds.Width + 300;
	}

	private void OnSeekBarPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (DataContext is not PlayerViewModel vm)
			return;
		
		vm.IsSeeking = true;
	}

	private void OnSeekBarPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (DataContext is not PlayerViewModel vm || _slider == null || !vm.IsSeeking)
			return;

		vm.IsSeeking = false;
		vm.Seek(TimeSpan.FromSeconds(_slider.Value), SeekOrigin.Begin);
	}
}