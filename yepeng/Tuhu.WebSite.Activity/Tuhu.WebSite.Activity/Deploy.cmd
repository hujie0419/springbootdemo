@echo off
for /f "tokens=2*" %%i in ('reg query HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion^|findstr "ProgramFilesDir (x86)"') do set p=%%j

set a=%cd: =:%
set a=%a:\= %
for %%j in (%a%) do (set a=%%j)
set a=%a::= %

set baseDir=obj
set programDir=%p:~10%
set SolutionDir=..\..
if exist "%programDir%\MSBuild\14.0\Bin\MSBuild.exe" (
	set MSBuild="%programDir%\MSBuild\14.0\Bin\MSBuild.exe"
) else (
	set programDir=C:\Program Files (x86)

	 if exist "%programDir%\MSBuild\14.0\Bin\MSBuild.exe" (
		set MSBuild="%programDir%\MSBuild\14.0\Bin\MSBuild.exe"
	) else (
		set MSBuild=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
	)
)
set MSBuild=%MSBuild% /m /v:m "%a%.csproj" /t:_WPPCopyWebApplication /p:Configuration=Release;WebProjectOutputDir="%baseDir%\Precompile";ExcludeGeneratedDebugSymbol="False"

set aspnet_compiler=%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler -v / -p "%baseDir%\Precompile" -d "%baseDir%\Deploy" -f -fixednames

set aspnet_merge="%programDir%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools\aspnet_merge.exe"
if exist %aspnet_merge% (
	GOTO SetMerge
)

set aspnet_merge="%programDir%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\aspnet_merge.exe"
if exist %aspnet_merge% (
	GOTO SetMerge
)

set aspnet_merge="%programDir%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\aspnet_merge.exe"
if exist %aspnet_merge% (
	GOTO SetMerge
)

set aspnet_merge="%programDir%\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\aspnet_merge.exe"
if exist %aspnet_merge% (
	GOTO SetMerge
)

set aspnet_merge="%programDir%\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.5 Tools\aspnet_merge.exe"
if not exist "%aspnet_merge%" (
	set aspnet_merge="%programDir%\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\aspnet_merge.exe"
)

:SetMerge
set aspnet_merge=%aspnet_merge% "%baseDir%\Deploy" -o "%a%.Pages" -a -debug

if exist "%baseDir%\Deploy" (
	rmdir /S /Q "%baseDir%\Deploy"
)
echo %MSBuild%
echo.
%MSBuild%
IF %ERRORLEVEL% NEQ 0 (
	GOTO End
)

echo.
echo %aspnet_compiler%
echo.
%aspnet_compiler%
IF %ERRORLEVEL% NEQ 0 (
	GOTO End
)

echo.
echo %aspnet_merge%
echo.
%aspnet_merge%
:End
pause
