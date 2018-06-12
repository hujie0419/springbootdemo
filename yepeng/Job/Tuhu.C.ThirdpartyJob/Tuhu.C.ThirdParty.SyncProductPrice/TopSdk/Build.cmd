@echo off
for /f "tokens=2*" %%i in ('reg query HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion^|findstr "ProgramFilesDir (x86)"') do set p=%%j

set a=%cd: =:%
set a=%a:\= %
for %%j in (%a%) do (set a=%%j)
set a=%a::= %

if exist "%p:~10%\MSBuild\14.0\Bin\MSBuild.exe" (
	set MSBuild="%p:~10%\MSBuild\14.0\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" (
	set MSBuild="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
) else (
	set MSBuild=%windir%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
)
if exist "%a%.csproj" (
	set MSBuild=%MSBuild% /m /v:m "%a%.csproj" /p:Configuration=Release
) else if exist "%a%.sln" (
	set MSBuild=%MSBuild% /m /v:m "%a%.sln" /p:Configuration=Release
)

echo %MSBuild%
echo.
%MSBuild%

if exist "%a%.csproj" (
	set MSBuild=%MSBuild% /m /v:m "%a%.csproj" /p:Configuration=Release
	if exist "nuget.exe" (
		if not exist "%a%.nuspec" (
			nuget spec
		)

		echo.
		nuget pack "%a%.csproj" -Prop Configuration=Release
	)
)
pause
