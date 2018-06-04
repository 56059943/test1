::----------------- 例子开始 -----------------------------------------

@echo on

:::::::::::::: 参数设置：：：：：：：：：：：：：：

set from=

set user=

set pass=

rem 多个邮件用逗号分隔
rem ,blat@sdf.lonestar.org
set to=56059943@qq.com

set subj=%1

set body=%2
set rem mail=F:\WSH\Charge\Cons\blat3219\sendcode.txt

set server=smtp.163.com

set attach=F:\WSH\Charge\Cons\blat3219\attach.zip

set debug=-debug -log blat.log -timestamp

::::::::::::::::: 运行blat :::::::::::::::::

::blat.exe %mail% -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% %debug%
rem F:\WSH\Charge\Cons\blat3219\blat.exe - -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% -body %body%
F:\WSH\Charge\Cons\blat3219\blat.exe - -to %to% -base64 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% -body %body%

pause