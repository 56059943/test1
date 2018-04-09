#include <iostream>
#include <string>

extern "C"
{
#include "lua/lua.h"
#include "lua/lualib.h"
#include "lua/lauxlib.h"
}

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

	//6.��ȡ����  
	lua_getglobal(L, "add");        // ��ȡ������ѹ��ջ��  
	lua_pushnumber(L, 10);          // ѹ���һ������  
	lua_pushnumber(L, 20);          // ѹ��ڶ�������  
	int iRet = lua_pcall(L, 2, 1, 0);// ���ú�������������Ժ󣬻Ὣ����ֵѹ��ջ�У�2��ʾ����������1��ʾ���ؽ��������  
	if (iRet)                       // ���ó���  
	{
		const char *pErrorMsg = lua_tostring(L, -1);
		std::cout << pErrorMsg << std::endl;
		lua_close(L);
		return;
	}
	if (lua_isnumber(L, -1))        //ȡֵ���  
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

//��Lua���õ�cע�ắ��  
static int add2Lua(lua_State* L)  //��Ҫע�⣺ע�ᵽlua�е�c����һ��Ҫ��  int (*func)(lua_State*)  �������͵�  
{
	//���ջ�еĲ����Ƿ�Ϸ���1��ʾLua����ʱ�ĵ�һ������(������)���������ơ�  
	//���Lua�����ڵ���ʱ���ݵĲ�����Ϊnumber���ú�����������ֹ�����ִ�С�  
	double op1 = luaL_checknumber(L, 1);
	double op2 = luaL_checknumber(L, 2);
	//�������Ľ��ѹ��ջ�С�����ж������ֵ��������������ѹ��ջ�С�  
	lua_pushnumber(L, op1 + op2);
	//����ֵ������ʾ��C�����ķ���ֵ��������ѹ��ջ�еķ���ֵ������  
	return 1;
}

//��һ����Lua���õ�Cע�ắ����  
static int sub2Lua(lua_State* L)
{
	double op1 = luaL_checknumber(L, 1);
	double op2 = luaL_checknumber(L, 2);
	lua_pushnumber(L, op1 - op2);
	return 1;
}

//��һ����Lua���õ�Cע�ắ����  
static int base64Encode(lua_State* L)
{
	const char * op1 = luaL_checkstring(L, 1);
	char buf[2048] = { 0 };
	int size = Base64Util::Base64EncodeQuick(buf, (const unsigned char *)op1, strlen(op1));
	lua_pushstring(L, buf);
	return 1;
}

int ccalllua()
{
	std::string fileName = "../Debug/index.lua";
	lua_State *L = luaL_newstate();
	luaL_openlibs(L);

	//��ָ���ĺ���ע��ΪLua��ȫ�ֺ������������е�һ���ַ�������Ϊע�ᵽLua�к��������ڶ�������Ϊʵ��C������ָ�롣  
	lua_register(L, "add2Lua", add2Lua);
	lua_register(L, "sub2Lua", sub2Lua);
	lua_register(L, "base64Encode", base64Encode);
	//��ע�������е�C����֮�󣬼�����Lua�Ĵ������ʹ����Щ�Ѿ�ע���C�����ˡ�  
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
		ccalllua();
		//test();
		int i;
		std::cin >> i;
	}
}

