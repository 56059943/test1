::----------------- 例子开始 -----------------------------------------

@echo on

:::::::::::::: 参数设置：：：：：：：：：：：：：：

set from=wshsuzhou@163.com

set user=wshsuzhou

set pass=Five5color

rem 多个邮件用逗号分隔
rem ,blat@sdf.lonestar.org
set to=56059943@qq.com

set subj=%1

set mail=F:\WSH\Charge\Cons\blat3219\body.txt

set server=smtp.163.com

set attach=F:\WSH\Charge\Cons\blat3219\attach.zip

set debug=-debug -log blat.log -timestamp

::::::::::::::::: 运行blat :::::::::::::::::

::blat.exe %mail% -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% %debug%
F:\WSH\Charge\Cons\blat3219\blat.exe %mail% -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach%

pause