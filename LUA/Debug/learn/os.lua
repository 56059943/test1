1、时间戳转换成时间

local t = 1412753621000

function getTimeStamp(t)
    return os.date("%Y%m%d%H",t/1000)
end
print(getTimeStamp(t))
 
2、得时间戳
 
os.time() -- 当前时间戳
os.time({day=17, month=5, year=2012, hour=0, minute=0, second=0}) -- 指定时间的时间戳

3、时间格式 yyyyMMddHHmmss

print(os.date("%Y%m%d%H%M%S", os.time()))