#include <iostream>
#include <string>
#include <time.h>  

#include "my.h"

#include "utils/base64_util.h"

void test()
{
	std::string jdata = "{\"hank\":\"hello world!\", level:99}";

	/*
	lua_State * L = luaL_newstate();
	
	lua_pushstring(L, jdata.c_str());

	if (lua_isstring(L, 1))
	{
		std::cout << lua_tostring(L, 1) << std::endl;
	}
	lua_close(L);
	*/

	//std::string fileName = "../Debug/hello_world.lua";
	std::string fileName = "../Debug/index.lua";
	lua_State * L = luaL_newstate();
	if (luaL_loadfile(L, fileName.c_str()))
	{
		std::cout << "load file failed." << std::endl;
		lua_close(L);
		return;
	}
	return;

	//luaopen_mylib(L);

	//lua_pushstring(L, "{\"hank\":\"hello world!\", level:99}");
	if (lua_pcall(L, 0, 0, 0))
	{
		std::cout << "lua call failed." << std::endl;

		lua_close(L);                                 
		return;
	}
	return;


	lua_getglobal(L, "str");
	std::string str = lua_tostring(L, -1);
	std::cout << str << std::endl;

	//6.读取函数  
	lua_getglobal(L, "add");        // 获取函数，压入栈中  
	lua_pushnumber(L, 10);          // 压入第一个参数  
	lua_pushnumber(L, 20);          // 压入第二个参数  
	int iRet = lua_pcall(L, 2, 1, 0);// 调用函数，调用完成以后，会将返回值压入栈中，2表示参数个数，1表示返回结果个数。  
	if (iRet)                       // 调用出错  
	{
		const char *pErrorMsg = lua_tostring(L, -1);
		std::cout << pErrorMsg << std::endl;
		lua_close(L);
		return;
	}
	if (lua_isnumber(L, -1))        //取值输出  
	{
		double fValue = lua_tonumber(L, -1);
		std::cout << "Result is " << fValue << std::endl;
	}

	lua_getglobal(L, "run");
	lua_pushstring(L, jdata.c_str());
	lua_pushstring(L, jdata.c_str());
	if (lua_pcall(L, 2, 0, 0))
	{
		lua_close(L);
		std::cout << "lua call run failed." << std::endl;
		return;
	}


	lua_close(L);
}

//待Lua调用的c注册函数  
static int add2Lua(lua_State* L)  //需要注意：注册到lua中的c函数一定要是  int (*func)(lua_State*)  这样类型的  
{
	//检查栈中的参数是否合法，1表示Lua调用时的第一个参数(从左到右)，依此类推。  
	//如果Lua代码在调用时传递的参数不为number，该函数将报错并终止程序的执行。  
	double op1 = luaL_checknumber(L, 1);
	double op2 = luaL_checknumber(L, 2);
	//将函数的结果压入栈中。如果有多个返回值，可以在这里多次压入栈中。  
	lua_pushnumber(L, op1 + op2);
	//返回值用于提示该C函数的返回值数量，即压入栈中的返回值数量。  
	return 1;
}

//另一个待Lua调用的C注册函数。  
static int sub2Lua(lua_State* L)
{
	double op1 = luaL_checknumber(L, 1);
	double op2 = luaL_checknumber(L, 2);
	lua_pushnumber(L, op1 - op2);
	return 1;
}

//另一个待Lua调用的C注册函数。  
static int base64Encode(lua_State* L)
{
	const char * msg = luaL_checkstring(L, 1);
	size_t msg_size = strlen(msg);
	char * buf = new char[msg_size * 2];
	int size = Base64Util::Base64EncodeQuick(buf, (const unsigned char *)msg, msg_size);
	buf[size] = '\0';
	lua_pushstring(L, buf);
	delete[] buf;
	return 1;
}

int ccalllua()
{
	std::string fileName = "../Debug/index.lua";
	lua_State *L = luaL_newstate();
	luaL_openlibs(L);

	//将指定的函数注册为Lua的全局函数变量，其中第一个字符串参数为注册到Lua中函数名，第二个参数为实际C函数的指针。  
	lua_register(L, "add2Lua", add2Lua);
	lua_register(L, "sub2Lua", sub2Lua);
	lua_register(L, "base64Encode", base64Encode);
	//在注册完所有的C函数之后，即可在Lua的代码块中使用这些已经注册的C函数了。  
	if (luaL_loadfile(L, fileName.c_str()) || lua_pcall(L, 0, 0, 0)) {
		printf("error %s\n", lua_tostring(L, -1));
		return -1;
	}
	lua_close(L);
	return 0;
}

/**/
void main()
{
	while (1)
	{
		CLOCKS_PER_SEC;
		double start, end, cost;
		start = clock();
		ccalllua();
		end = clock();
		cost = end - start;
		printf("%fms\n", cost);
		//test();
		int i;
		std::cin >> i;
	}
}

