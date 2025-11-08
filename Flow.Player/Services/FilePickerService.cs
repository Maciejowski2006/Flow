using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Flow.Player;

public class FilePickerService(Window target) : IFilePickerService
{
	public async Task<IStorageFile?> OpenFileAsync()
	{
		IReadOnlyList<IStorageFile> files = await target.StorageProvider.OpenFilePickerAsync(new()
		{
			Title = "Open file",
			AllowMultiple = false,
			FileTypeFilter = [AudioAll]
		});
		return files.Count >= 1 ? files[0] : null;
	}
	public async Task<IReadOnlyList<IStorageFile>> OpenFilesAsync()
	{
		IReadOnlyList<IStorageFile> files = await target.StorageProvider.OpenFilePickerAsync(new()
		{
			Title = "Open file",
			AllowMultiple = true,
			FileTypeFilter = [AudioAll]
		});
		return files;
	}

	public static FilePickerFileType AudioAll { get; } = new("All audio files")
	{
		Patterns = ["*.mp3", "*.flac", "*.aac", "*.wav", "*.ogg", "*.opus", "*.au"],
		AppleUniformTypeIdentifiers = ["public.audio"],
		MimeTypes = ["audio/*"]
	};
}