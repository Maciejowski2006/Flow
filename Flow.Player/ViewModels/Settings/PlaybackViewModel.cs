using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.ViewModels.Settings;

public class FlatAudioOutputDevice(string id, string name, bool isGroup)
{
	public string Id { get; set; } = id;
	public string Name { get; set; } = name;
	public bool IsGroup { get; set; } = isGroup;
}

public class PlaybackViewModel : ViewModelBase
{
	public ObservableCollection<FlatAudioOutputDevice> AudioOutputDevices { get; } = [];
	private FlatAudioOutputDevice _selectedDevice;
	public FlatAudioOutputDevice SelectedDevice
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
		foreach (AudioOutputGroup group in _mediaPlayerService.AudioOutputGroups)
		{
			AudioOutputDevices.Add(new(group.Id, group.Name, true));
			foreach (AudioOutputDevice device in group.Devices)
			{
				AudioOutputDevices.Add(new(device.Id, device.Name, false));
			}
		}
		_selectedDevice = _mediaPlayerService.CurrentOutputDeviceId is not null
			? AudioOutputDevices.First(x => x.Id == _mediaPlayerService.CurrentOutputDeviceId)
			: AudioOutputDevices.First(x => x.Id == "");
	}
}