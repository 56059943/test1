--[[
	常用工具模块
	提供：
	lua对象转字符串
	table转字符串
	字符串转table
	
	business_util.ToStringEx
	business_util.TableToString
	business_util.StringToTable
]]

CONST_TABLE = {
	account = 1
	, client_version = 2
	, client_build = 3
	, package_name = 4
	, wshsdkv = 5
	, ext = 6
	, cp_order = 7
	
	--c++在处理os的时候特别注意，需要转成os_id
	, os_id = 100
	, game_id = 101
	, channel_id = 102
	, advertiser_id = 103
	, platform_id = 104
	, sdk_id = 105
	
	, ta_type = 201
	
	, token = 301
	, business = 302
	, CALLER = 303
	, APIKEY = 304
	, AESKEY = 305
	, sid = 306
	, sign = 307
	, third_account = 308
	, apple_game_center = 309
	, facebook_account = 310
	, google_account = 311
	, mobile_phone = 312
	, mobile_sms_code = 313
	, sqq = 314
	, weixin = 315
	, email = 316
	, valid_time = 317
	, update_time = 318
	, create_time = 319
	, delete_time = 320
}

local business_util = {}

function business_util.CheckLogin(value)
    if type(value)=='table' then
       return business_util.CheckTable(value)
    end
	return 1
end

function business_util.CheckTable(t)
    for key,value in pairs(t) do
        if CONST_TABLE[key] == nil then
			return 1;
		end
    end
     return 0
end

return business_util
