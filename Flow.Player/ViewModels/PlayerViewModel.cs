using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flow.Player.Models;
using Flow.Player.Services;

namespace Flow.Player.ViewModels;

public partial class PlayerViewModel() : ViewModelBase
{
	public bool IsSeeking { get; set; }
	[ObservableProperty] private Track? _playingTrack;
	[ObservableProperty] private long _duration;
	[ObservableProperty] private long _time;
	private long _sliderTime;
	public long SliderTime
	{
		get => _sliderTime;
		set
		{
			if (IsSeeking)
				return;
			
			SetProperty(ref _sliderTime, value);
		}
	}
	private int _volume = 80;
	public int Volume
	{
		get => _volume;
		set
		{
			int v = Math.Clamp(value, 0, 100);
			_player.SetMute(false);
			_player.Volume = v;
			Muted = false;
			SetProperty(ref _volume, v);
		}
	}
	private bool _isPlaying;
	public bool IsPlaying
	{
		get => _isPlaying;
		set
		{
			if (value)
				_player.Play();
			else
				_player.Pause();
			
			SetProperty(ref _isPlaying, value);
		}
	}

	[ObservableProperty] private bool _muted;
	private readonly IMediaPlayerService _player;

	public PlayerViewModel(IMediaPlayerService playerService, CommandLineArgumentsService commandLineArgumentsService) : this()
	{
		_player = playerService;

		string[] args = commandLineArgumentsService.Arguments;
		if (commandLineArgumentsService.Arguments.Length == 0)
			return;

		_ = LoadTrack(args[0]);
	}

	public async Task LoadTrack(string filePath)
	{
		PlayingTrack = new(filePath);
		await _player.LoadFile(filePath);
		Volume = _player.Volume;
		Duration = _player.GetDuration();
		_player.SetTimeCallback((_, args) => { 
			Time = args.Time;
			SliderTime = args.Time;
		});
		IsPlaying = true;
		OnPropertyChanged(nameof(SliderTime));
		OnPropertyChanged(nameof(PlayingTrack));
	}
	
	
	[RelayCommand]
	public void ToggleMute() => _player.SetMute(Muted);
	
	
	public void SeekTo(long value) => _player.Time = value;
	
	/// <summary>
	/// Seeks by defined amount of seconds
	/// </summary>
	/// <param name="value">Time in seconds</param>
	public void Seek(long value) => _player.Time += value * 1000;
	

	[RelayCommand]
	private void PlayPrevious()
	{
		// TODO: Implement logic for playlist support
		SeekTo(0);
		OnPropertyChanged(nameof(SliderTime));
	}
}