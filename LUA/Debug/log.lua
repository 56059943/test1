--[[
	日志模块
	提供打印日志到文件，文件名格式20180101.log，每日一个日志文件
	log.d  	打印调试日志，发布后不显示
	log.e	打印错误日志
]]

local utils = require("utils")

local log = {}

local PATH = "../Debug/"

function isArrayTable(t)  
    if type(t) ~= "table" then  
        return false  
    end  
  
    local n = #t  
    for i,v in pairs(t) do  
        if type(i) ~= "number" then  
            return false  
        end  
          
        if i > n then  
            return false  
        end   
    end  
  
    return true   
end  

function isTable(t)  
    if type(t) ~= "table" then  
        return false  
    end  
    return true   
end  

function randomParameter(...)
    arg = { ... }-- 获取可变参数集合
    local count = 0-- 记录可变参数总数量
    --local sum = 0-- 记录可变参数总和
	local str = ""
	for k,v in pairs(arg) do-- 通过forIn循环迭代arg可变参数集合的键(k)值(v)对
        count = count + 1;-- 计数
        --sum = sum + v;-- 累加值
		if (isTable(v)) then
			str = str .. "{table}" .. ", "
		else
			str = str .. v .. ", "
		end
	end
	
	str = "size:" .. count .. " msg:" .. str;
    return str
	--print(id.."-->"..sum.."|共"..count.."个可变参数");-- 输出结果
end

function getParameter(...)
    arg = { ... }-- 获取可变参数集合
	local str = ""
	for k,v in pairs(arg) do-- 通过forIn循环迭代arg可变参数集合的键(k)值(v)对
		str = str .. utils.ToStringEx(v) .. ", "
	end
    return str
end

function dddddd(...)
	print("[debug] log d start")
	local date = os.date("%Y-%m-%d %H:%M:%S")
    local tableP = { ... }
    local filename = PATH .. "print_log.txt"
    local file
	if initFile1 == nil then
		initFile1 = io.open(filename,"a+")
		file = initFile1
		print(file)

	else
		file = io.open(filename,"a")  
		print("else")		
		print(file)		
	end


	file:write("\n["..date.."]")
	file:write(...)

	file:close()
	print("[debug] log d end")
end


function writeToFile(...)
	--[2018-02-26 14:37:22]
	--local date = os.date("%Y-%m-%d %H:%M:%S")
	local date = os.date("%Y%m%d")
	local datetime = os.date("%Y-%m-%d %H:%M:%S")
    local tableP = { ... }
    local filename = PATH .. date .. ".log"
    local file
	if initFile1 == nil then
		initFile1 = io.open(filename,"a+")
		file = initFile1
	else
		file = io.open(filename,"a")                
	end


	file:write("\n["..datetime.."]")
	file:write(getParameter(...))
	--file:write(randomParameter(...))
	--file:write(...)
	file:close()
end

--打印日志，每天一个日志文件，格式20180101.log
function log.d(...)
	writeToFile("[debug]", ...)
end

function log.e(...)
	writeToFile("[error]", ...)
end


return log
