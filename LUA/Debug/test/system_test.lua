
function CallFunc()
--这里能调用show()，证明了_G中有show这个元素
	show()
end


function show()
	print("it is showsome")
end




CallFunc()
a = 10
print(a)