@echo off&setlocal EnableDelayedExpansion   
rem   
rem (1)  
rem src_str.substring(start_pos, length)  
rem set result=%src_str:~start_pos,length%   
rem   
set str1=This is string1  
set str2=%str1:~8,6%  
set str3=%str1:~0,4%  
set str4=%str1:~5%  
  
echo src_str:  
echo str1=%str1%  
  
echo result strings:  
echo str2=%str2%  
echo str3=%str3%  
echo str4=%str4%  
  
echo/  
rem   
rem (2)  
rem strlen()  
rem   
set str1=This is a test string  
set str2=Hello World  
  
set num=0  
set str_tmp=%str1%  
:next1  
if not "%str_tmp%"=="" (  
set /a num+=1  
set "str_tmp=%str_tmp:~1%"  
goto next1  
)  
echo str1=%str1%  
echo str1's length is %num%  
  
set num=0  
set str_tmp=%str2%  
:next2  
if not "%str_tmp%"=="" (  
set /a num+=1  
set "str_tmp=%str_tmp:~1%"  
goto next2  
)  
echo str2=%str2%  
echo str2's length is %num%  
  
echo/  
rem   
rem (3)  
rem strchr()  
rem return 1~length, 0 indicate the char doesn't exist  
rem   
rem setlocal EnableDelayedExpansion  
set str1=This is a test string  
set ch1=t  
  
set num=0  
set str_tmp=%str1%  
:next  
if not "%str_tmp%"=="" (  
set /a num+=1  
rem check the fisrt char of this string  
if "!str_tmp:~0,1!"=="%ch1%" goto last  
rem get remaining string  
set "str_tmp=%str_tmp:~1%"  
goto next  
)  
set /a num=0  
:last  
echo the first occurrence of char '%ch1%' in "%str1%" is %num%  
set pre_str=!str1:~0,%num%!  
echo then we get prefix str: "%pre_str%".  
set suf_str=!str1:~%num%!  
echo then we get suffix str: "%suf_str%".  
call :strchr str1 %ch1% index  
echo strchr() '%ch1%' in "%str1%" is %index%  
set ch1=a  
call :strchr str1 %ch1% index 2  
echo strchr() '%ch1%' in "%str1%" is %index%  
  
echo/  
rem   
rem (4)  
rem lastIndex()  
rem return 1~length, 0 indicate the char doesn't exist  
rem   
set str1=This is a test string  
set ch1=t  
call :last_index str1 %ch1% last_index  
echo the last occurrence of char '%ch1%' in "%str1%" is %last_index%.  
set pre_str=!str1:~0,%last_index%!  
echo then we get prefix str: "%pre_str%".  
set suf_str=!str1:~%last_index%!  
echo then we get suffix str: "%suf_str%".  
set ch1=s  
call :last_index str1 %ch1% last_index22  
echo the last occurrence of char '%ch1%' in "%str1%" is %last_index22%.  
goto finish  
  
  
rem =======================================================================================  
rem   
rem the follow code: i have extracted strlen(), strchr(), lastIndex() into method(or funtion)  
rem   
rem =======================================================================================  
  
rem ----Begin strlen()---------------------------------------------------------  
rem   
rem strlen()  
rem   
goto finish  
:strlen src_str len  
rem here we transfer src_str via reference, becall this way allow space chars  
setlocal  
set num=0  
rem the follow call must start with call  
call set str_tmp=%%%1%%  
:_strlen_next1  
if not "%str_tmp%"=="" (  
set /a num+=1  
set "str_tmp=%str_tmp:~1%"  
goto _strlen_next1  
)  
endlocal & set "%2=%num%"  
goto :EOF  
rem ----End strlen()-----------------------------------------------------------  
  
  
rem ----Begin strchr()---------------------------------------------------------  
rem   
rem strchr() or first_index() method  
rem return 1~length, 0 indicate the char doesn't exist  
rem   
goto finish  
:strchr src_str %char% result %from%  
rem here we transfer src_str via reference, becall this way allow space chars  
setlocal EnableDelayedExpansion  
set num=0  
rem the follow call must start with call  
call set str_tmp=%%%1%%  
set target_ch=%2  
set from_pos=0  
if not "%4"=="" set /a from_pos=%4  
if %from_pos% GTR 1 set /a from_pos-=1  
if %from_pos% GEQ 1 set str_tmp=!str_tmp:~%from_pos%!  
set /a num+=%from_pos%  
  
:_first_index_next1  
if not "%str_tmp%"=="" (  
set /a num+=1  
rem check the fisrt char of this string  
if "!str_tmp:~0,1!"=="%target_ch%" goto _first_index_last  
rem get remaining string  
set "str_tmp=%str_tmp:~1%"  
goto _first_index_next1  
)  
set /a num=0  
:_first_index_last  
endlocal & set "%3=%num%"  
goto :EOF  
rem ----End strchr()-----------------------------------------------------------  
  
  
rem ----Begin last_index-------------------------------------------------------  
rem   
rem last_index()  
rem return 1~length, 0 indicate the char doesn't exist  
rem   
goto finish  
:last_index src_str %char% result  
rem here we transfer src_str via reference, becall this way allow space chars  
setlocal EnableDelayedExpansion  
rem the follow call must start with call  
call set str_tmp=%%%1%%  
set len=0  
rem fisrt get the length of this string  
call :strlen str_tmp len  
rem echo the length is %len%  
  
set target_ch=%2  
:_last_index_next1  
if not "%str_tmp%"=="" (  
rem check the last char of this string  
if "!str_tmp:~-1!"=="%target_ch%" goto _last_index_last  
rem get remaining string  
set /a len-=1  
set "str_tmp=%str_tmp:~0,-1%"  
goto _last_index_next1  
)  
set /a len=0  
:_last_index_last  
endlocal & set "%3=%len%"  
goto :EOF  
rem ----End last_index---------------------------------------------------------  
  
  
:finish  

pause