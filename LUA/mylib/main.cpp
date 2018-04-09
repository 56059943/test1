//mylib.c
#include <stdio.h>
#include <math.h>


#define DllExport   __declspec( dllexport )

extern "C"
{
#include "lua/lua.h"
#include "lua/lualib.h"
#include "lua/lauxlib.h"

//int DllExport myadd(lua_State *L);
int DllExport luaopen_mylib(lua_State *L);

}

static int myadd(lua_State *L) {
	int a = luaL_checknumber(L, 1);
	int b = luaL_checknumber(L, 2);
	lua_pushnumber(L, a + b);
	return 1;
}

static const struct luaL_Reg mylib[] = {
	{ "add", myadd },
	{ NULL, NULL }
};

int luaopen_mylib(lua_State *L) {
	luaL_newlib(L, mylib);
	return 1;
}
