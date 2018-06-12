%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe "%~dp0\BaoYangRefreshCacheService.exe"
Net Start RefreshService
sc config RefreshService start= auto
pause