using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flow.Player.Models;
using Flow.Player.Services;
using Flow.Player.Services.MediaPlayerService;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.ViewModels;

public partial class PlayerViewModel() : ViewModelBase
{
	public bool IsSeeking { get; set; }
	[ObservableProperty] private Track? _playingTrack;
	[ObservableProperty] private TimeSpan _duration;
	[ObservableProperty] private TimeSpan _time;
	[ObservableProperty] private float _sliderDuration;
	private float _sliderTime;
	public float SliderTime
	{
		get => _sliderTime;
		set
		{
			if (IsSeeking)
				return;
			
			SetProperty(ref _sliderTime, value);
		}
	}
	private float _volume = 1;
	public float Volume
	{
		get => _volume;
		set
		{
			float v = Math.Clamp(value, 0, 1);
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
	private readonly IMediaPlayerService _player = null!;

	public PlayerViewModel(IMediaPlayerService playerService, CommandLineArgumentsService commandLineArgumentsService) : this()
	{
		_player = playerService;

		string[] args = commandLineArgumentsService.Arguments;
		if (commandLineArgumentsService.Arguments.Length == 0)
			return;

		IEnumerable<string> formats = FilePickerService.AudioAll.Patterns!.Select(x => x.Substring(1, x.Length - 1));
		if (formats.Any(x => args[0].EndsWith(x)))
			_ = LoadTrack(args[0]);
	}

	public async Task LoadTrack(string filePath)
	{
		PlayingTrack = new(filePath);
		await _player.LoadFile(filePath);
		_player.Volume = Volume;
		
		Duration = TimeSpan.FromSeconds(_player.Duration);
		SliderDuration = _player.Duration;
		DispatcherTimer timer = new()
		{
			Interval = TimeSpan.FromMilliseconds(100)
		};
		timer.Tick += (_, _) => UpdateTimers();
		timer.Start();

		IsPlaying = true;
		OnPropertyChanged(nameof(SliderTime));
		OnPropertyChanged(nameof(PlayingTrack));
	}

	private void UpdateTimers()
	{
		Time = TimeSpan.FromSeconds(_player.Time);
		SliderTime = _player.Time;
	}
	
	[RelayCommand]
	public void ToggleMute() => _player.SetMute(Muted);

	public void Seek(TimeSpan value, SeekOrigin origin = SeekOrigin.Current)
	{
		_player.Seek(value, origin);
		UpdateTimers();
	}
	

	[RelayCommand]
	private void PlayPrevious()
	{
		// TODO: Implement logic for playlist support
		Seek(TimeSpan.FromSeconds(0), SeekOrigin.Begin);
		OnPropertyChanged(nameof(SliderTime));
	}
}