using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flow.Player.Models;
using Flow.Player.Services.AudioEngineService;
using Flow.Player.Services.PlaybackSubsystem;
using Flow.Player.Services.PlaylistSerializer;
using IconPacks.Avalonia.Lucide;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.ViewModels;

public partial class PlaylistViewModel : ViewModelBase
{
	[ObservableProperty] private Track? _selectedTrack;
	public FlatTreeDataGridSource<Track> PlaylistSource { get; }
	public bool IsPlaylistSelected => _playlistPath is not null;
	
	private string? _playlistPath { get; set; }

	private readonly IAudioEngineService _audioEngineService;
	private readonly IPlaybackSubsystem _playbackSubsystem;
	private readonly IPlaylistSerializer _playbackSerializer;

	public PlaylistViewModel(IAudioEngineService audioEngine, IPlaybackSubsystem playbackSubsystem, IPlaylistSerializer playbackSerializer)
	{
		_audioEngineService = audioEngine;
		_playbackSubsystem = playbackSubsystem;
		_playbackSerializer = playbackSerializer;

		PlaylistSource = new(_playbackSubsystem.Playlist)
		{
			Columns =
			{
				new TemplateColumn<Track>("", new FuncDataTemplate<Track>((track, nameScope) =>
					{
						if (track?.CoverArt is null)
							return new PackIconLucide
							{
								Kind = PackIconLucideKind.Music,
								Height = 64,
								Width = 64,
								Padding = new(8),
								Background = Brushes.DarkGray,
							};

						return new Image
						{
							Source = track.CoverArt,
							Height = 64,
							Stretch = Stretch.Uniform
						};
					}),
					width: new GridLength(64, GridUnitType.Pixel),
					options: new() { CanUserResizeColumn = false, }),
				new TextColumn<Track, string>("Title", x => x.Title),
				new TextColumn<Track, string>("Artist", x => x.Artist),
				new TextColumn<Track, string>("Album", x => x.Album),
			},
		};
		if (PlaylistSource.RowSelection != null)
		{
			PlaylistSource.RowSelection.SelectionChanged += (_, args) =>
			{
				if (args.SelectedItems.Count == 0)
				{
					SelectedTrack = null;
					return;
				}

				SelectedTrack = args.SelectedItems[0];
			};
		}
	}

	[RelayCommand]
	private async Task PlaySelectedTrack()
	{
		if (SelectedTrack is null)
			return;
		await _playbackSubsystem.PlayTrackFromPlaylistAsync(SelectedTrack);
	}

	[RelayCommand]
	private async Task AddTrack()
	{
		IReadOnlyList<IStorageFile> files = await App.Services.GetRequiredService<IFilePickerService>().OpenFilesAsync("Add songs to playlist", FilePickerService.AudioAll);
		foreach (IStorageFile file in files)
		{
			_playbackSubsystem.Playlist.Add(new(file.Path.LocalPath));
		}
	}
	[RelayCommand]
	private void RemoveSelectedTrack()
	{
		if (SelectedTrack is not null)
			_playbackSubsystem.Playlist.Remove(SelectedTrack);
	}
	[RelayCommand]
	private void DeselectItem() => PlaylistSource.RowSelection?.Clear();

	[RelayCommand]
	private async Task OpenPlaylist()
	{
		IStorageFile? file = await App.Services.GetRequiredService<IFilePickerService>().OpenFileAsync("Open playlist", FilePickerService.Json);
		if (file is null)
			return;
		
		_playbackSubsystem.Playlist.Clear();
		
		IReadOnlyList<Track>? playlist =  await _playbackSerializer.DeserializeAsync(file.Path.LocalPath);
		if (playlist is null)
			return;
		
		_playlistPath = file.Path.LocalPath;
		OnPropertyChanged(nameof(IsPlaylistSelected));
		foreach (Track track in playlist)
		{
			_playbackSubsystem.Playlist.Add(track);
		}
	}

	[RelayCommand]
	private async Task SavePlaylist()
	{
		if (_playlistPath is null)
			return;
		
		await _playbackSerializer.SerializeAsync(_playbackSubsystem.Playlist, _playlistPath);
	}
	
	[RelayCommand]
	private async Task SavePlaylistAs()
	{
		IStorageFile? file = await App.Services.GetRequiredService<IFilePickerService>().SaveFileAsync("Save playlist as", FilePickerService.Json);
		if (file is null)
			return;
		
		await _playbackSerializer.SerializeAsync(_playbackSubsystem.Playlist, file.Path.LocalPath);
		_playlistPath = file.Path.LocalPath;
		OnPropertyChanged(nameof(IsPlaylistSelected));
	}
}