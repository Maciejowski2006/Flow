using System;
using Avalonia.Controls;

namespace Flow.Player.Views;

public partial class BlurView : UserControl
{
	public BlurView()
	{
		InitializeComponent();

		bool isLinux = OperatingSystem.IsLinux();
		NonLinuxBlur.IsVisible = !isLinux;
		LinuxBlur.IsVisible = isLinux;
	}
}