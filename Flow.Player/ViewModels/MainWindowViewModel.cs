using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.ViewModels;

public partial class MainWindowViewModel() : ViewModelBase
{
	public bool IsLinux => OperatingSystem.IsLinux();
	private readonly PlayerViewModel _pvm = App.AppServices.GetRequiredService<PlayerViewModel>();
	
	[RelayCommand]
	private async Task OpenFile()
	{
		IStorageFile? file = await App.Services.GetRequiredService<IFilePickerService>().OpenFileAsync();
		if (file is null)
			return;

		await _pvm.LoadTrack(file.Path.LocalPath);
	}
	
	public IRelayCommand PlayPreviousCommand => _pvm.PlayPreviousCommand;

	[RelayCommand]
	private void ToggleMute()
	{
		_pvm.Muted = !_pvm.Muted;
		_pvm.ToggleMute();
	}

	[RelayCommand]
	private void SeekBack() => _pvm.Seek(-5);
	[RelayCommand]
	private void SeekForward() => _pvm.Seek(5);
}