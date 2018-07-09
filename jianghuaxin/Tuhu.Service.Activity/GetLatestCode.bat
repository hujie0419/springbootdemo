@echo off
setlocal EnableDelayedExpansion

set "output_cnt=0"
for /f "delims=" %%b in ('git branch') do (
    set /a output_cnt+=1
    set "branche=%%b"
    set branches[!output_cnt!]=!branche:~2,20!
    if "!branche:~0,2!" == "* " (
        set currentBranch=!branche:~2,20!
    )
)

set "gt=>"
set prefix=%~dp0
set prefix=!prefix:~0,-1!
set prefix=!prefix!!gt!
echo !prefix!git checkout master
git checkout master
for /f %%d in ('git diff') do GOTO exit
echo.
echo !prefix!git pull tuhu master
git pull tuhu master
for /f %%d in ('git diff') do GOTO exit
echo.
echo !prefix!git pull origin master
git pull origin master
for /f %%d in ('git diff') do GOTO exit
echo.
echo !prefix!git push origin master
git push origin master
for /f %%d in ('git diff') do GOTO exit

for /L %%n in (1 1 !output_cnt!) do (
    if "!branches[%%n]!" neq "master" (
        echo.
        echo !prefix!git checkout !branches[%%n]!
        git checkout !branches[%%n]!
        for /f %%d in ('git diff') do GOTO exit
        echo.
        echo !prefix!git merge master
        git merge master
        for /f %%d in ('git diff') do GOTO exit
        echo.
        echo !prefix!git pull tuhu !branches[%%n]!
        git pull tuhu !branches[%%n]!
        for /f %%d in ('git diff') do GOTO exit
        echo.
        echo !prefix!git pull origin !branches[%%n]!
        git pull origin !branches[%%n]!
        for /f %%d in ('git diff') do GOTO exit
        echo.
        echo !prefix!git push origin !branches[%%n]!
        git push origin !branches[%%n]!
        for /f %%d in ('git diff') do GOTO exit
    )
)

echo.
echo !prefix!git checkout !currentBranch!
git checkout !currentBranch!
Build Debug
exit
:exit
pause