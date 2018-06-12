@echo off
for /f "tokens=2*" %%i in ('reg query HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion^|findstr "ProgramFilesDir (x86)"') do set p=%%j

set a=%cd: =:%
set a=%a:\= %
for %%j in (%a%) do (set a=%%j)
set a=%a::= %

if exist "%p:~10%\MSBuild\14.0\Bin\MSBuild.exe" (
	set MSBuild="%p:~10%\MSBuild\14.0\Bin\MSBuild.exe"
) else (
	set MSBuild=%windir%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
)
set MSBuild=%MSBuild% /m /v:m "%a%.csproj" /p:Configuration=Release

echo %MSBuild%
echo.
%MSBuild%
pause
