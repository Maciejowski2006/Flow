using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Flow.Player;

public class FilePickerService(Window target) : IFilePickerService
{
	public async Task<IStorageFile?> OpenFileAsync(string title, params IReadOnlyList<FilePickerFileType> fileType)
	{
		IReadOnlyList<IStorageFile> files = await target.StorageProvider.OpenFilePickerAsync(new()
		{
			Title = title,
			AllowMultiple = false,
			FileTypeFilter = fileType
		});
		return files.Count >= 1 ? files[0] : null;
	}
	public async Task<IReadOnlyList<IStorageFile>> OpenFilesAsync(string title, params IReadOnlyList<FilePickerFileType> fileType)
	{
		IReadOnlyList<IStorageFile> files = await target.StorageProvider.OpenFilePickerAsync(new()
		{
			Title = title,
			AllowMultiple = true,
			FileTypeFilter = fileType
		});
		return files;
	}

	public async Task<IStorageFile?> SaveFileAsync(string title, params IReadOnlyList<FilePickerFileType> fileType)
	{
		IStorageFile? file = await target.StorageProvider.SaveFilePickerAsync(new()
		{
			Title = title,
			FileTypeChoices = fileType
		});
		return file;
	}


	public static FilePickerFileType AudioAll { get; } = new("All audio files")
	{
		Patterns = ["*.mp3", "*.flac", "*.aac", "*.wav", "*.ogg", "*.opus", "*.au"],
		AppleUniformTypeIdentifiers = ["public.audio"],
		MimeTypes = ["audio/*"]
	};

	public static FilePickerFileType Json { get; } = new("JSON playlist")
	{
		Patterns = ["*.json"],
		AppleUniformTypeIdentifiers = ["public.json"],
		MimeTypes = ["application/json"]
	};
}