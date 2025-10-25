using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Flow.Player;

public interface IFilePickerService
{
	public Task<IStorageFile?> OpenFileAsync();
}