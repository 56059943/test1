local init = require "init"
local util = require('business_util')


--[[
	来自登录服的验证登录，异步
	
]]
function login(t)
	if util.CheckLogin(t) then
		return 1
	end
	
end