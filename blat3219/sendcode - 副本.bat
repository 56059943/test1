::----------------- ���ӿ�ʼ -----------------------------------------

@echo on

:::::::::::::: �������ã���������������������������

set from=wshsuzhou@163.com

set user=д���Լ������˺ţ�û��@��֮��

set pass=д���Լ���������

rem ����ʼ��ö��ŷָ�
rem ,blat@sdf.lonestar.org
set to=56059943@qq.com

set subj=111

set body=F:\WSH\Charge\Cons\blat3219\tmp\1.txt
set mail=F:\WSH\Charge\Cons\blat3219\tmp\physhy@qq.com.txt
set mail=F:\WSH\Charge\Cons\blat3219\tmp\1.txt
set mail<F:\WSH\Charge\Cons\blat3219\tmp\1.txt

set rem mail=F:\WSH\Charge\Cons\blat3219\sendcode.txt

set server=smtp.163.com

set attach=F:\WSH\Charge\Cons\blat3219\attach.zip

set debug=-debug -log blat.log -timestamp

::::::::::::::::: ����blat :::::::::::::::::

::blat.exe %mail% -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% %debug%
rem F:\WSH\Charge\Cons\blat3219\blat.exe - -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% -body %body%
F:\WSH\Charge\Cons\blat3219\blat.exe %mail% -to %to% -base64 -subject %subj% -server %server% -f %from% -u %user% -pw %pass%

pause