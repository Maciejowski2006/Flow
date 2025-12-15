using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Flow.Player;

public interface IFilePickerService
{
	public Task<IStorageFile?> OpenFileAsync(string title, params IReadOnlyList<FilePickerFileType> fileType);
	public Task<IReadOnlyList<IStorageFile>> OpenFilesAsync(string title, params IReadOnlyList<FilePickerFileType> fileType);
	public Task<IStorageFile?> SaveFileAsync(string title, params IReadOnlyList<FilePickerFileType> fileType);
}