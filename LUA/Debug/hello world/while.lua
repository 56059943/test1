-- While循环方法
function doWhileLoop()
    local i = 10;-- index变量
    while i > 0 do-- 执行循环体
        print("这是由while循环遍历出的字符串"..i);-- 循环体输出语句
        i = i - 1;-- 切记循环体需要有结束的可能，否则死循环得话程序就崩溃了
    end-- 循环体结束
end
doWhileLoop();-- 执行方法

-- Repeat循环方法
-- 和C/C++还有Java的do..while类似
function doRepeat()
    local i = 10;-- index变量
    repeat-- 执行循环体
        print("这是由repeat循环遍历出的字符串"..i);-- 循环体输出语句
        i = i - 1;-- 切记循环体需要有结束的可能，否则死循环得话程序就崩溃了
    until i < 0-- 设置循环体结束条件
end
doRepeat();-- 执行方法

-- for..do循环方法
-- For内参数分别是起始值，终止值，步长
function doForDo()
    local i = 0;-- index变量
    for i=0, 10, 1 do-- 设置执行从0~10次
        print("这是由doFor循环遍历出的字符串"..i);-- 循环体输出语句
        i = i + 1;-- 每次迭代增加i的值，注意这里修改的i值并不会影响for循环体中的各条件参数
    end
end
doForDo();-- 执行方法

-- 可变参数方法&for...in...do循环方法
-- 可以添加指定参数在可变参数之前，可变参数用{ ... }获取
function randomParameter(id,...)
    arg = { ... };-- 获取可变参数集合
    local count = 0;-- 记录可变参数总数量
    local sum = 0;-- 记录可变参数总和
    for k,v in pairs(arg) do-- 通过forIn循环迭代arg可变参数集合的键(k)值(v)对
        count = count + 1;-- 计数
        sum = sum + v;-- 累加值
    end
    print(id.."-->"..sum.."|共"..count.."个可变参数");-- 输出结果
end
randomParameter("固定参数", 12, 7, 67, 42, 22, 56);-- 执行方法