using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Velopack;
using Velopack.Locators;
using Velopack.Sources;

namespace Flow.Player.Services;

public class UpdateManagerService
{
	private readonly UpdateManager _updateManager;
	public UpdateManagerService()
	{
		TestVelopackLocator test = new TestVelopackLocator("Flow", "1.0.0", "");
		GithubSource source = new("https://github.com/Maciejowski2006/Flow", null, false);
		_updateManager = new(source, new(), test);
	}
	public async Task<UpdateInfo?> CheckForUpdatesAsync()
	{
		
		UpdateInfo? newVersion = await _updateManager.CheckForUpdatesAsync();
		return newVersion;
	}

	public async Task DownloadUpdatesAsync(UpdateInfo updateInfo, Action<int>? progress = null) => await _updateManager.DownloadUpdatesAsync(updateInfo, progress);
	public void UpdateEndExit()
	{
		if (_updateManager.UpdatePendingRestart is { } asset)
			_updateManager.ApplyUpdatesAndExit(asset);
	}
	
}