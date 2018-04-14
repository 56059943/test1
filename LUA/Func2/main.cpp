//mylib.c
#include "my.h"

int GameLogic1(lua_State *L)
{
	int n = lua_gettop(L);
	double sum = 0;
	int i;

	for (i = 1; i <= n; i++)
	{

		sum += lua_tonumber(L, i);
	}

	lua_pushnumber(L, sum / n);

	lua_pushnumber(L, 1);

	return 2;
}
const struct luaL_Reg Func1lib[] = {
	{ "GameLogic1", GameLogic1 },
	{ NULL, NULL }
};


int luaopen_Func1lib(lua_State* L)
{
#if LUA_VERSION_NUM == 501
	luaL_openlib(L, "Func1lib", Func1lib, 0);
#elif LUA_VERSION_NUM == 503
	lua_getglobal(L, "Func1lib");
	if (lua_isnil(L, -1)) {
		lua_pop(L, 1);
		lua_newtable(L);
	}
	luaL_setfuncs(L, Func1lib, 0);
	lua_setglobal(L, "Func1lib");


	/*
	��luaL_register������API�Ͳ��Ƽ����ˣ���Ϊ����Ⱦȫ�����֣�����һ������д��
	lua_newtable(L);
	luaL_setfuncs(L, mylib, 0);
	return 1;
	��Lua�õ�ʱ�򣬾��������ú��ˣ�
	local mylib= require "mylib"
	*/
#endif
	return 1;
}