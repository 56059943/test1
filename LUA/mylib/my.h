#pragma once

extern "C"
{
#include "../SDK/lua/lua.h"
#include "../SDK/lua/lualib.h"
#include "../SDK/lua/lauxlib.h"
}

//#pragma comment(lib, "lua5.1.lib")  

#if defined(_WIN32)  
extern "C" _declspec(dllexport)  int luaopen_Func1lib(lua_State* L);

#else  
extern "C" int luaopen_Func1lib(lua_State* L);
#endif  