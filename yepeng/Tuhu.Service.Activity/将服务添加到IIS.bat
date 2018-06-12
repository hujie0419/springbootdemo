set appcmd=%windir%\system32\inetsrv\appcmd

%appcmd% delete site Tuhu.Service.Activity
%appcmd% delete apppool Tuhu.Service.Activity
%appcmd% add apppool /name:Tuhu.Service.Activity
%appcmd% add site /name:"Tuhu.Service.Activity" /bindings:http://*:9066 /physicalPath:"%~dp0%Tuhu.Service.Activity\Tuhu.Service.Activity.Hosting"
%appcmd% set app "Tuhu.Service.Activity/" /applicationPool:Tuhu.Service.Activity
%appcmd% set site /site.name:Tuhu.Service.Activity /+bindings.[protocol='net.tcp',bindinginformation='9067:*']


pause
