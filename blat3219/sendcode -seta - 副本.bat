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
rem ����Ǳ�������
set "line=!line:$1=%ccc%!"
rem ��һ���滻������1���a
set "line=!line:$2=%nnn%!"
rem �ڶ����滻������2����B.....
set "line=!line:\t=    !"
rem ��������
rem set "line=!line:4=D!"
rem ���ĸ�������
echo !line!
goto :eof