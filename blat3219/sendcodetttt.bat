::----------------- ���ӿ�ʼ -----------------------------------------

@echo off

:::::::::::::: �������ã���������������������������

set from=

set user=

set pass=

rem ����ʼ��ö��ŷָ�
rem ,blat@sdf.lonestar.org
set to=%1
::ģ�壬���磺template_1001
set tmp=%2
set code=%3

rem echo %to% %tmp% %code%

set name=NO
set subj=��ɫ����Ϸע����֤

set "body=������������֤�룺%code%"

set server=smtp.163.com

rem set attach=F:\WSH\Charge\Cons\blat3219\attach.zip

rem set debug=-debug -log blat.log -timestamp

rem echo %to% %subj% %body%

::::::::::::::::: ����blat :::::::::::::::::

::blat.exe %mail% -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% %debug%
rem F:\WSH\Charge\Cons\blat3219\blat.exe - -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% -body %body%
rem F:\WSH\Charge\Cons\blat3219\blat.exe - -to %to% -base64 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% -body %body%
F:\WSH\Charge\Cons\blat3219\blat.exe - -to %to% -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -body %body%
rem F:\WSH\Charge\Cons\blat3219\blat.exe %body% -to %to% -base64 -subject %subj% -server %server% -f %from% -u %user% -pw %pass%

rem pause