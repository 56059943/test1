--[[
	常用工具模块
	提供：
	lua对象转字符串
	table转字符串
	字符串转table
	
	utils.ToStringEx
	utils.TableToString
	utils.StringToTable
]]

local utils = {}

function utils.ToStringEx(value)
    if type(value)=='table' then
       return utils.TableToStr(value)
    elseif type(value)=='string' then
        return "\'"..value.."\'"
    else
       return tostring(value)
    end
end

function utils.TableToStr(t)
    if t == nil then return "" end
    local retstr= "{"

    local i = 1
    for key,value in pairs(t) do
        local signal = ","
        if i==1 then
          signal = ""
        end

        if key == i then
            retstr = retstr..signal..utils.ToStringEx(value)
        else
            if type(key)=='number' or type(key) == 'string' then
                retstr = retstr..signal..'['..utils.ToStringEx(key).."]="..utils.ToStringEx(value)
            else
                if type(key)=='userdata' then
                    retstr = retstr..signal.."*s"..utils.TableToStr(getmetatable(key)).."*e".."="..utils.ToStringEx(value)
                else
                    retstr = retstr..signal..key.."="..utils.ToStringEx(value)
                end
            end
        end

        i = i+1
    end

     retstr = retstr.."}"
     return retstr
end

function utils.StrToTable(str)
    if str == nil or type(str) ~= "string" then
        return
    end
    
    return loadstring("return " .. str)()
end

return utils
