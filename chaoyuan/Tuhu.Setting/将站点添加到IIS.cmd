set appcmd=%windir%\system32\inetsrv\appcmd

%appcmd% delete site setting.tuhutech.cn
%appcmd% delete apppool setting.tuhutech.cn
%appcmd% add apppool /name:setting.tuhutech.cn
%appcmd% add site /name:setting.tuhutech.cn /bindings:http://setting.tuhutech.cn:80 /physicalPath:"%~dp0%src\Tuhu.Setting\Tuhu.Provisioning"
%appcmd% set app "setting.tuhutech.cn/" /applicationPool:setting.tuhutech.cn

%appcmd% delete site mms.tuhu.dev
%appcmd% delete apppool mms.tuhu.dev
%appcmd% add apppool /name:mms.tuhu.dev
%appcmd% add site /name:mms.tuhu.dev /bindings:http://mms.tuhu.dev:80 /physicalPath:"%~dp0%src\Tuhu.MMS\Tuhu.MMS.Web"
%appcmd% set app "mms.tuhu.dev/" /applicationPool:mms.tuhu.dev

pause
