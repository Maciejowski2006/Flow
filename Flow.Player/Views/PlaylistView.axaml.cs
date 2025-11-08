using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Flow.Player.Messages;
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

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);
		if (DataContext is PlaylistViewModel vm)
			vm.Setup();
	}
}