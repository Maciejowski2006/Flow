using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flow.Player.Models;
using IconPacks.Avalonia.Lucide;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.ViewModels;

public partial class PlaylistViewModel : ViewModelBase
{
	[ObservableProperty] private Track? _selectedTrack;
	private ObservableCollection<Track> _tracks;
	public FlatTreeDataGridSource<Track> PlaylistSource { get; }

	private IFilePickerService _filePickerService { get; set; }
	private readonly IMediaPlayerService _mediaPlayerService;

	public PlaylistViewModel(IMediaPlayerService mediaPlayer)
	{
		_mediaPlayerService = mediaPlayer;
		_tracks =
		[
			// Mock data TODO: Remove
			new(@"C:\Users\Maciej\Downloads\Linksy @ Furality Umbra 2024.flac"),
			new(@"C:\Users\Maciej\Music\ValueFactory Sets\ValueFactory @ Furality Umbra 2024.flac"),
			new(@"C:\Users\Maciej\Music\ValueFactory Sets\ValueFactory @ ZRave 2023-08-12.mp3"),
		];
		PlaylistSource = new(_tracks)
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
	
	public void Setup() { _filePickerService = App.Services.GetRequiredService<IFilePickerService>(); }

	[RelayCommand]
	private async Task AddTrack()
	{
		IReadOnlyList<IStorageFile> files = await _filePickerService.OpenFilesAsync();
		foreach (IStorageFile file in files)
		{
			_tracks.Add(new(file.Path.LocalPath));
		}
	}
	[RelayCommand]
	private void RemoveSelectedTrack()
	{
		if (SelectedTrack is not null)
			_tracks.Remove(SelectedTrack);
	}
	[RelayCommand]
	private void DeselectItem() => PlaylistSource.RowSelection?.Clear();
}