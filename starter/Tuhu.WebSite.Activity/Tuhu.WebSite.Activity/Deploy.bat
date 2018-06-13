@echo off
set a=%cd: =:%
set a=%a:\= %
for %%j in (%a%) do (set a=%%j)
set a=%a::= %

set baseDir=..\Deploy\
set time1=%time:~0,2%%time:~3,2%%time:~6,2%

echo %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild /m "%a%.csproj" /t:_WPPCopyWebApplication;Rebuild /p:Configuration=Release;WebProjectOutputDir="Obj\Deploy"
echo.
%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild /m "%a%.csproj" /t:_WPPCopyWebApplication;Rebuild /p:Configuration=Release;WebProjectOutputDir="Obj\Deploy"

echo %windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler -v \ -p "%cd%\Obj\Deploy" "%baseDir%\%a%" -f -fixednames
echo.
%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler -v \ -p "%cd%\Obj\Deploy" "%baseDir%\%a%" -f -fixednames

if exist "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\aspnet_merge.exe" (
	echo "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\aspnet_merge" "%baseDir%\%a%" -o "%a%.Pages"
	echo.
	"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\aspnet_merge" "%baseDir%\%a%" -o "%a%.Pages"
) else (
	echo "C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\aspnet_merge" "%baseDir%\%a%" -o "%a%.Pages"
	echo.
	"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\aspnet_merge" "%baseDir%\%a%" -o "%a%.Pages"
)

set time2=%time:~0,2%%time:~3,2%%time:~6,2%
set /a time3=%time2%-%time1%
echo.
echo º‰∏Ù%time3%√Î
xcopy /y /e ..\Deploy\Tuhu.WebSite.Web.Activity\*.* C:\svn\Deploy\huodong
pause