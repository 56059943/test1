         local date = os.date("%Y-%m-%d %H:%M:%S")
         local tableP = { ... }
         local filename = "print_log.txt"
         local file
         if initFile1 == nil then
             initFile1 = io.open(filename,"w+")
             file = initFile1
         else
             file = io.open(filename,"a")                
         end


        file:write("["..date.."]")

        file:close()