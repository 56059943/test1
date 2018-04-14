--[[
	所有业务处理的基础脚本
	
	LUA 动态脚本设计思路
	作为动态脚本，lua只处理业务逻辑
	一切固定的算法，都用c++实现，提供接口给lua调用，例如：base64 md5 RSA256
	关于JSON，刚开始，我想直接提供json给lua处理，发现效率底，使用起来也没有table方便
	。既然lua不适合做解析，就用c++将json（包括xml，urlparams）转成table给lua处理。
	有了上面的经验，接着开始设计lua。
	1，每个业务脚本独立
	2，支持多线程调用lua执行
	3，已知的放在c++，未知的，可扩展的放在lua
]]

package.path = package.path .. ';D:/GitHub/test/LUA/Debug/test/?.lua;'
package.cpath = package.cpath .. ';D:/GitHub/test/LUA/Debug/?.dll;'


