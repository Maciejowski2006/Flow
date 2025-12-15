using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Flow.Player.Messages;
using Flow.Player.Services;
using Flow.Player.Services.PlaybackSubsystem;
using Flow.Player.Views;
using Microsoft.Extensions.DependencyInjection;

using Velopack;

namespace Flow.Player.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
	private readonly PlayerViewModel _pvm = App.AppServices.GetRequiredService<PlayerViewModel>();
	private readonly IPlaybackSubsystem _playbackSubsystem = App.AppServices.GetRequiredService<IPlaybackSubsystem>();
	[ObservableProperty] private bool _showPlaylistView;

	public MainWindowViewModel()
	{
		WeakReferenceMessenger.Default.Register<PlaylistViewChangedMessage>(this, (_, message) => ShowPlaylistView = message.Value);
	}
	

	[RelayCommand]
	private async Task CheckForUpdates(bool? triggeredByUser = false)
	{
		UpdateManagerService updateManager = App.AppServices.GetRequiredService<UpdateManagerService>();
		UpdateInfo? update = await updateManager.CheckForUpdatesAsync();
		switch (update)
		{
			case null when triggeredByUser ?? false:
			{
				await WeakReferenceMessenger.Default.Send(new ShowDialogMessage("You are using the latest version of Flow.", MessageBoxButtons.Ok));
				return;
			}
			case null:
				return;
		}

		MessageBoxReturn? userWishesToUpdate = await WeakReferenceMessenger.Default.Send(
			new ShowDialogMessage("There is new update available, do you want to update?\n"
			                      + $"New Version: {update.TargetFullRelease.Version}\n"
			                      + "You can keep using the app while the update is being downloaded, we'll apply the update when you're done using the app.",
				MessageBoxButtons.YesNo));

		if (userWishesToUpdate != MessageBoxReturn.Yes)
			return;

		await updateManager.DownloadUpdatesAsync(update, i =>
		{
			if (i == 100)
			{
				// TODO: Implement notifications
				// THIS IS WINDOWS ONLY CODE CREATED FOR TESTING:
				// new ToastContentBuilder()
				// 	.AddArgument("action", "update")
				// 	.AddText("Update is ready to be applied.")
				// 	.AddText("When you close the app, we'll apply the update.")
				// 	.Show();
			}
		});
		
	}

	[RelayCommand]
	private async Task OpenFile()
	{
		IStorageFile? file = await App.Services.GetRequiredService<IFilePickerService>().OpenFileAsync("Open song", FilePickerService.AudioAll);
		if (file is null)
			return;

		await _playbackSubsystem.LoadTrackAsync(file.Path.LocalPath);
	}
	[RelayCommand]
	private void OpenSettings()
	{
		SettingsWindow settingsWindow = new();
		settingsWindow.Show();
	}

	public IRelayCommand PlayPreviousCommand => _pvm.PlayPreviousCommand;

	[RelayCommand]
	private void ToggleMute()
	{
		_pvm.Muted = !_pvm.Muted;
		_pvm.ToggleMute();
	}

	[RelayCommand]
	private void SeekBack() => _pvm.Seek(TimeSpan.FromSeconds(-5));
	[RelayCommand]
	private void SeekForward() => _pvm.Seek(TimeSpan.FromSeconds(5));

	[RelayCommand]
	private void TogglePlaylistView()
	{
		ShowPlaylistView = !ShowPlaylistView;
		WeakReferenceMessenger.Default.Send(new PlaylistViewChangedMessage(ShowPlaylistView));
	}
}