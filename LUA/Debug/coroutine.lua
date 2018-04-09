function foo (a)
	print("foo", a)
	return coroutine.yield(2*a)
 end
 
co = coroutine.create(function (p, a,b)
	print("1", p, a, b)
	local r = foo(a+1)
	print("2", p, r)
	local r, s = coroutine.yield(a+b, a-b)
	print("3", p, r, s)
	return b, "end"
end)

print("main1", coroutine.resume(co, "p1", 1, 10))
print("main2", coroutine.resume(co, "pp", "r1"))
print("main3", coroutine.resume(co, "$$", "r2"))
--print("main3", coroutine.resume(co, "p3", "x", "y"))
--print("main4", coroutine.resume(co, "p4", "x", "y"))