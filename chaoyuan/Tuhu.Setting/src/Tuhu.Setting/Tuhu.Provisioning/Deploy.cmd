@echo off
for /f "tokens=2*" %%i in ('reg query HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion^|findstr "ProgramFilesDir (x86)"') do set p=%%j

set a=%cd: =:%
set a=%a:\= %
for %%j in (%a%) do (set a=%%j)
set a=%a::= %

set baseDir=obj

if exist "%p:~10%\MSBuild\14.0\Bin\MSBuild.exe" (
	set MSBuild="%p:~10%\MSBuild\14.0\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" (
	set MSBuild="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
) else (
	set MSBuild=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
)
set MSBuild=%MSBuild% /m /v:m "%a%.csproj" /t:_WPPCopyWebApplication /p:Configuration=Release;WebProjectOutputDir="%baseDir%\Precompile";ExcludeGeneratedDebugSymbol="False"

set aspnet_compiler=%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler -v / -p "%baseDir%\Precompile" -d "%baseDir%\Deploy" -f -fixednames

if exist "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\aspnet_merge.exe" (
	set aspnet_merge="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\aspnet_merge.exe"
) else if exist "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\aspnet_merge.exe" (
	set aspnet_merge="C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\aspnet_merge.exe"
) else if exist "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.5 Tools\aspnet_merge.exe" (
	set aspnet_merge="C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.5 Tools\aspnet_merge.exe"
) else (
	set aspnet_merge="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\aspnet_merge.exe"
)
set aspnet_merge=%aspnet_merge% "%baseDir%\Deploy" -o "%a%.Pages" -a -debug

if exist "%baseDir%\Deploy" (
	rmdir /S /Q "%baseDir%\Deploy"
)
echo %MSBuild%
echo.
%MSBuild%
echo.
echo %aspnet_compiler%
echo.
%aspnet_compiler%
echo.
echo %aspnet_merge%
echo.
%aspnet_merge%
pause
