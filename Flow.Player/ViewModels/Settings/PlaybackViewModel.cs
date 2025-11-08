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
			_mediaPlayerService.SetOutputDevice(value.Id);

			SetProperty(ref _selectedDevice, value);
		}
	}

	private readonly IMediaPlayerService _mediaPlayerService;
	public PlaybackViewModel()
	{
		_mediaPlayerService = App.AppServices.GetRequiredService<IMediaPlayerService>();
		AudioOutputDevices = new(_mediaPlayerService.AudioOutputDevices.Select(device => device.Group is null ? new AudioOutputDevice($"{device.Name}", device.Id) : new AudioOutputDevice($"{device.Name} ({device.Group})", device.Id)));

		_selectedDevice = _mediaPlayerService.CurrentOutputDevice;
	}
}