@echo off
set exeName=Tuhu.YunYing.WinService.PushJobSchedulerService.exe
set BASE_DIR=%~dp0
set insPath=%windir%\Microsoft.NET\Framework64\v4.0.30319
echo ############ ��ʼ��װ��̨����=>%exeName% 
echo.
%BASE_DIR:~0,2%
cd %BASE_DIR%
echo. 
echo ############ 1.����ԭ�з����� ���������ʼ
pause
installutil.exe /U %exeName%
echo. 
echo ############ 2.������ϣ����������ʼ��װ��̨����  
pause
echo.
installutil.exe %exeName%
echo.  
echo ############ 3.�������������� InstallUtil.InstallLog �в鿴����Ĳ��������   
echo.
pause