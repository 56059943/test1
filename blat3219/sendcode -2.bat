@echo off & setlocal enabledelayedexpansion
echo. > tmp/56059943@qq.com

for /f "tokens=*" %%i in (template_1001) do (
		if "%%i"=="" (echo.) else (set "line=%%i" & call :chg)
	)>>tmp/56059943@qq.com
pause
exit
 
:chg
rem ����Ǳ�������
set "line=!line:$1=ABCDEF!"
rem ��һ���滻������1���a
set "line=!line:$2=Hank!"
rem �ڶ����滻������2����B.....
set "line=!line:\t=    !"
rem ��������
rem set "line=!line:4=D!"
rem ���ĸ�������
echo !line!
goto :eof