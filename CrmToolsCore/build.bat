set msbuild=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
set PointVersion=1.0.0

:: define CRM2011 for crm5, otherwise don't set any defines
:: /define:CRM2011


set outpathprop=/property:OutputPath=release\%PointVersion%\3.5
%msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v3.5 /t:rebuild %outpathprop%\CRM4\Debug EntityWrapCRM4.csproj
%msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v3.5 /t:rebuild %outpathprop%\CRM4\Release EntityWrapCRM4.csproj

:: note that we can't build 2011 against the 3.5 framework.

set outpathprop=/property:OutputPath=release\%PointVersion%\4.0
%msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v4.0 /t:rebuild %outpathprop%\CRM4\Debug EntityWrapCRM4.csproj
%msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v4.0 /t:rebuild %outpathprop%\CRM4\Release EntityWrapCRM4.csproj
%msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v4.0 /t:rebuild %outpathprop%\CRM2011\Debug EntityWrap.csproj
%msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v4.0 /t:rebuild %outpathprop%\CRM2011\Release EntityWrap.csproj