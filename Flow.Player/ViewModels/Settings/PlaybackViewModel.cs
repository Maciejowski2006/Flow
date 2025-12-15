using System.Collections.ObjectModel;
using System.Linq;
using Flow.Player.Services.AudioEngineService;
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
			_audioEngineService.SetOutputDevice(value.Id);
			SetProperty(ref _selectedDevice, value);
		}
	}

	private readonly IAudioEngineService _audioEngineService;
	public PlaybackViewModel()
	{
		_audioEngineService = App.AppServices.GetRequiredService<IAudioEngineService>();
		AudioOutputDevices = new(_audioEngineService.AudioOutputDevices.Select(device => device.Group is null ? new AudioOutputDevice($"{device.Name}", device.Id) : new AudioOutputDevice($"{device.Name} ({device.Group})", device.Id)));

		_selectedDevice = _audioEngineService.CurrentOutputDevice;
	}
}