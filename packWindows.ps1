if ($args[0] -eq $null)
{
    Write-Host "Version is null. Set version as first argument (eg. ./packWindows.ps1 1.0.0)"
    return
}

vpk pack --packId Flow --packVersion $args[0] --packDir .\Flow.Player\bin\Release\net8.0\win-x64\publish --mainExe Flow.Player.exe