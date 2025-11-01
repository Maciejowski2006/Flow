using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.ViewModels;

public class AudioMenuItem
{
	public string Name { get; set; }
	public string Id { get; set; }
	public bool IsGroup { get; set; }
	public ICommand? Command { get; set; }
	public bool IsSelected { get; set; }
}

public partial class MainWindowViewModel : ViewModelBase
{
	public bool IsLinux => OperatingSystem.IsLinux();
	private readonly PlayerViewModel _pvm = App.AppServices.GetRequiredService<PlayerViewModel>();
	public ObservableCollection<AudioMenuItem> AudioOutputGroups { get; } = [];

	public MainWindowViewModel()
	{
		IMediaPlayerService media = App.AppServices.GetRequiredService<IMediaPlayerService>();
		FillAudioOutputGroups(media);
	}

	public void FillAudioOutputGroups(IMediaPlayerService media)
	{
		AudioOutputGroups.Clear();
		foreach (AudioOutputGroup group in media.AudioOutputGroups)
		{
			AudioOutputGroups.Add(new()
			{
				Name = group.Name,
				Id = group.Id,
				IsGroup = true
			});
			foreach (AudioOutputDevice device in group.Devices)
			{
				AudioOutputGroups.Add(new()
				{
					Name = device.Name,
					Id = device.Id,
					IsGroup = false,
					Command = new RelayCommand(() =>
					{
						media.Pause();
						media.SetOutputDevice(device.Id);
						media.Play();
						FillAudioOutputGroups(media);
					}),
					IsSelected = media.CurrentOutputDeviceId == device.Id || (media.CurrentOutputDeviceId == null && string.IsNullOrEmpty(device.Id))
				});
			}
		}
	}
	
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