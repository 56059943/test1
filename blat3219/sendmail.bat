::----------------- ���ӿ�ʼ -----------------------------------------

@echo on

:::::::::::::: �������ã���������������������������

set from=

set user=

set pass=

rem ����ʼ��ö��ŷָ�
rem ,blat@sdf.lonestar.org
set to=56059943@qq.com

set subj=%1

set mail=F:\WSH\Charge\Cons\blat3219\body.txt

set server=smtp.163.com

set attach=F:\WSH\Charge\Cons\blat3219\attach.zip

set debug=-debug -log blat.log -timestamp

::::::::::::::::: ����blat :::::::::::::::::

::blat.exe %mail% -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% %debug%
F:\WSH\Charge\Cons\blat3219\blat.exe %mail% -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach%

pause