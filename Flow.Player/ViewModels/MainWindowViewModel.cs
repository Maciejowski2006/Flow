using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Player.ViewModels;

public partial class MainWindowViewModel() : ViewModelBase
{
	[RelayCommand]
	private async Task OpenFile()
	{
		IStorageFile? file = await App.Services.GetRequiredService<IFilePickerService>().OpenFileAsync();
		if (file is null)
			return;

		PlayerViewModel pvm = App.AppServices.GetRequiredService<PlayerViewModel>();
		
		await pvm.LoadTrack(file.Path.LocalPath);
	}
}