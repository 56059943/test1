::----------------- 例子开始 -----------------------------------------

@echo off

:::::::::::::: 参数设置：：：：：：：：：：：：：：

set from=

set user=

set pass=

rem 多个邮件用逗号分隔
rem ,blat@sdf.lonestar.org
set to=%1
::模板，例如：template_1001
set tmp=%2
set code=%3

rem echo %to% %tmp% %code%

set name=NO
set subj=五色花游戏注册验证

set "body=以下是您的验证码：%code%"

set server=smtp.163.com

rem set attach=F:\WSH\Charge\Cons\blat3219\attach.zip

rem set debug=-debug -log blat.log -timestamp

rem echo %to% %subj% %body%

::::::::::::::::: 运行blat :::::::::::::::::

::blat.exe %mail% -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% %debug%
rem F:\WSH\Charge\Cons\blat3219\blat.exe - -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% -body %body%
rem F:\WSH\Charge\Cons\blat3219\blat.exe - -to %to% -base64 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% -body %body%
F:\WSH\Charge\Cons\blat3219\blat.exe - -to %to% -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -body %body%
rem F:\WSH\Charge\Cons\blat3219\blat.exe %body% -to %to% -base64 -subject %subj% -server %server% -f %from% -u %user% -pw %pass%

rem pause