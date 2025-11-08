using System;
using System.Collections.ObjectModel;
using System.Linq;
using Flow.Player.Services.MediaPlayerService;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.ViewModels.Settings;

public class PlaybackViewModel : ViewModelBase
{
	public ObservableCollection<AudioOutputDevice> AudioOutputDevices { get; }
	private AudioOutputDevice _selectedDevice;
	public AudioOutputDevice SelectedDevice
	{
		get => _selectedDevice;
		set
		{
			_mediaPlayerService.Pause();
			_mediaPlayerService.SetOutputDevice(value.Id);
			_mediaPlayerService.Play();

			SetProperty(ref _selectedDevice, value);
		}
	}

	private readonly IMediaPlayerService _mediaPlayerService;
	public PlaybackViewModel()
	{
		_mediaPlayerService = App.AppServices.GetRequiredService<IMediaPlayerService>();
		AudioOutputDevices = new(_mediaPlayerService.AudioOutputDevices.Select(device => device.Group is null ? new AudioOutputDevice($"{device.Name}", device.Id) : new AudioOutputDevice($"{device.Name} ({device.Group})", device.Id)));
		
		
		_selectedDevice = _mediaPlayerService.CurrentOutputDeviceId is not null
			? AudioOutputDevices.First(x => x.Id == _mediaPlayerService.CurrentOutputDeviceId)
			: AudioOutputDevices.First(x =>
			{
				if (OperatingSystem.IsWindows()) return x.Id == "";
				if (OperatingSystem.IsLinux()) return x.Id == "default";

				return true;
			});
	}
}