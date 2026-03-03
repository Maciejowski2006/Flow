using System;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Messaging;
using Flow.Player.Messages;

namespace Flow.Player.Views;

public partial class BlurView : UserControl
{
	private readonly SolidColorBrush _backgroundBrush = new(Color.FromRgb(30, 30, 30));
	public BlurView()
	{
		InitializeComponent();

		bool isLinux = OperatingSystem.IsLinux();
		NonLinuxBlur.IsVisible = !isLinux;
		LinuxBlur.IsVisible = isLinux;
		Background = Models.Settings.Instance.EnableBlur ? Brushes.Transparent : _backgroundBrush;
		
		WeakReferenceMessenger.Default.Register<BlurView, SettingsUpdatedMessage>(this, static (w, _) =>
		{
			Models.Settings settings = Models.Settings.Instance;
			w.Background = settings.EnableBlur ? Brushes.Transparent : w._backgroundBrush;
		});
	}
}