::----------------- 例子开始 -----------------------------------------

@echo on

:::::::::::::: 参数设置：：：：：：：：：：：：：：

set from=wshsuzhou@163.com

set user=写上自己邮箱账号，没有@和之后

set pass=写上自己邮箱密码

rem 多个邮件用逗号分隔
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

::::::::::::::::: 运行blat :::::::::::::::::

::blat.exe %mail% -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% %debug%
rem F:\WSH\Charge\Cons\blat3219\blat.exe - -to %to% -base64 -charset Gb2312 -subject %subj% -server %server% -f %from% -u %user% -pw %pass% -attach %attach% -body %body%
F:\WSH\Charge\Cons\blat3219\blat.exe %mail% -to %to% -base64 -subject %subj% -server %server% -f %from% -u %user% -pw %pass%

pause