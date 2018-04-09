


local t = {}
t["code"] = 1;
t["msg"] = "init failed.";
log.e(t)

--为何lua需要json，答案肯定是不需要，将json转table后，传过来
local r = 	{
				["code"]=0
				, ["msg"]="ok"
			}
log.e(r)

local r2 = 	{
				["code"]=0
				, ["msg"]="ok"
				, ["data"]={
								["status"]=0
								, ["account"]="acc001"
								, ["amount"]="0.01"
								, ["price"]=2.01
							}
			}
log.e(r2)

local r3 = 	{
				code=0
				, msg=ok
				, data={
							status=0
							, account="acc001"
							, amount="0.01"
							, price=2.01
						}
			}
log.e(r3)