@echo off

set body=""
set tmp=template_1001

for /f "tokens=*" %%i in ("template_1001") do (
		if "%%i"=="" (
			
		) else (set "ccc=%code%" & set "nnn=%name%" & set "line=%%i" & call :chg)
	)
pause
echo %tmp%
echo %body%	
pause
	
:chg
rem 这个是保留空行
set "line=!line:$1=%ccc%!"
rem 第一个替换。。。1变成a
set "line=!line:$2=%nnn%!"
rem 第二个替换。。。2换成B.....
set "line=!line:\t=    !"
rem 第三个。
rem set "line=!line:4=D!"
rem 第四个。。。
echo !line!
goto :eof