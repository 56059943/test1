local l = require "log"


CallFunc()
d2("test", "test2", 1, 2, 3, 4, 5, 6, 7 ,8 , 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20)

print(a)

function a()


	local cjson = require "cjson"
	local func = loadfile("/path/to/hello_world.lua")
	local status,err = pcall(func) -- 把整个脚本当作函数来执行
	if not status then
		local code = err.code and tonumber(err.code) or 500
		local msg = err.msg and tostring(err.msg) or "Unknown error occurred"
		--print(cjson.encode({code = code,msg = msg})) -- 输出{"code":10001,"msg":"发生错误了！"}
	end

end