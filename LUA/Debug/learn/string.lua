
string.gsub(mainString, findString, replaceString, num)

字符串替换操作，mainString为要替换的字符串，findString为被替换的字符，replaceString要替换的字符，num替换次数（可以忽略则全部替换）



string.format

%d

十进制整数

%o

八进制整数

%x

十六进制整数，大写的话为 %X

%f

浮点型 格式 [-]nnnn.nnnn

%e

科学表示法 格式 [-]n.nnnn e [+|-]nnn, 大写的话为 %E

%g

floating-point as %e if exp. < -4 or >= precision, else as %f ; uppercase if %G .

%c

character having the (system-dependent) code passed as integer

%s

没有\0的字符串

%q

双引号间的string, with all special characters escaped

%%

' % ' 字符

%a 字母

%c控制字符

%d多个数字

%l 小写字母

%p标点符号

%s空白字符

%x十六进制

%z内部表示为0的字符