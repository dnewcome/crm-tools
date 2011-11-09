:: Install dependencies from assembly cache.
:: set the variable assemblycache to the local or remote cache
set assemblycache=..\..\..\altai-dev\assemblycache

xcopy /sqy %assemblycache%\Altai.Commons.Crm2011 Altai.Commons.Crm2011\
xcopy /sqy %assemblycache%\Altai.Commons.Crm4 Altai.Commons.Crm4\

xcopy /sqy %assemblycache%\Crm2011Sdk Crm2011Sdk\
xcopy /sqy %assemblycache%\Crm4Sdk Crm4Sdk\

:: xcopy /sqy %assemblycache%\CrmQuery CrmQuery\
xcopy /sqy %assemblycache%\Djn.Commons Djn.Commons\
xcopy /sqy %assemblycache%\FakeCrm FakeCrm\
