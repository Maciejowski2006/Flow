using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Flow.Player;

public interface IFilePickerService
{
	public Task<IStorageFile?> OpenFileAsync();
	public Task<IReadOnlyList<IStorageFile>> OpenFilesAsync();
}