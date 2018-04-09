local a = 10

function CallFunc()
--这里能调用show()，证明了_G中有show这个元素
	show()
end


function show()
	print("it is showsome")
end

function d(...)
	local date = os.date("%Y-%m-%d %H:%M:%S")
    local tableP = { ... }
    local filename = "print_log.txt"
    local file
	if initFile1 == nil then
		initFile1 = io.open(filename,"a+")
		file = initFile1
	else
		file = io.open(filename,"a")                
	end


	file:write("\n["..date.."]")
	file:write(...)

	file:close()
end

--打印日志，每天一个日志文件，格式20180101.log
--[[]]
function d2(...)
	--[2018-02-26 14:37:22]
	--local date = os.date("%Y-%m-%d %H:%M:%S")
	local date = os.date("%Y%m%d")
    local tableP = { ... }
    local filename = date .. ".log"
    local file
	if initFile1 == nil then
		initFile1 = io.open(filename,"a+")
		file = initFile1
	else
		file = io.open(filename,"a")                
	end


	file:write("\n["..date.."]")
	file:write(...)

	file:close()
end
