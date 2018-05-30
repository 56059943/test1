#include <iostream>
#include <string>
#include <time.h>  

#include "my.h"


int main(int argc, char* argv[]) {
	//批处理命令中加上 pause 暂停看运行效果

	//std::string to = "physhy@qq.com";
	std::string to = "56059943@qq.com";
	//std::string subj = "五色花游戏注册验证";
	//std::string name = "Hank";
	std::string tmp = "template_1001";
	std::string code = "111666";


	//std::string path = "F:/WSH/Charge/Cons/blat3219/sendmail.bat";
	//std::string path = "F:/WSH/Charge/Cons/blat3219/sendcode.bat";
	std::string path = "F:/WSH/Charge/Cons/blat3219/sendcodetttt.bat";

	char buf[1024] = { 0 };
	//sprintf_s(buf, sizeof(buf), "cmd.exe /c \"%s %s %s\"", path.c_str(), subject.c_str(), body.c_str());
	sprintf_s(buf, sizeof(buf), "cmd.exe /c \"%s %s %s %s\"", path.c_str(), to.c_str(), tmp.c_str(), code.c_str());

	system(buf);
	//sprintf_s(buf, sizeof(buf), "explorer.exe /c \"%s\"", path.c_str());
	//system(buf);
	//system("cmd.exe /c \"D:\\test.bat\"");
	//system("explorer.exe \"D:\\test.bat\"");

	//system("pause");
	return 0;
}