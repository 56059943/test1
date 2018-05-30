@echo off & setlocal enabledelayedexpansion
echo. > tmp/56059943@qq.com

for /f "tokens=*" %%i in (template_1001) do (
		if "%%i"=="" (echo.) else (set "line=%%i" & call :chg)
	)>>tmp/56059943@qq.com
pause
exit
 
:chg
rem 这个是保留空行
set "line=!line:$1=ABCDEF!"
rem 第一个替换。。。1变成a
set "line=!line:$2=Hank!"
rem 第二个替换。。。2换成B.....
set "line=!line:\t=    !"
rem 第三个。
rem set "line=!line:4=D!"
rem 第四个。。。
echo !line!
goto :eof