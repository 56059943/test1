-- "db.lua"文件中内容。
--[[ 这里看似是一张名为"entry"的表中存储了许多数据，
     实际上在转化程序中这时一个名为"entry"的函数，
     参数是下面这个存储了许多数据的"table"。
     还记得"foo({})"与"foo{}"等同吗？]]
entry{
    title = "Tecgraf",
    org = "Computer Graphics Technology Group, PUC-Rio",
    url = "http://www.tecgraf.puc-rio.br/",
    contact = "Waldemar Celes",
    description = [[
    TeCGraf is the result of a partnership between PUC-Rio,
    the Pontifical Catholic University of Rio de Janeiro,
    and <A HREF="http://www.petrobras.com.br/">PETROBRAS</A>,
    the Brazilian Oil Company.
    TeCGraf is Lua's birthplace,
    and the language has been used there since 1993.
    Currently, more than thirty programmers in TeCGraf use
    Lua regularly; they have written more than two hundred
    thousand lines of code, distributed among dozens of
    final products.]]
}

-- "main.lua"文件中内容。
function fwrite (fmt, ...)
    -- 可变参数由"..."传递。
    for i = 1, select('#', ...) do
        arg[i] = select(i, ...)    -- 这里select()返回了i之后所有的值，但多余的值被抛弃了。
    end
    return io.write(string.format(fmt, table.unpack(arg)))
end

function BEGIN()
    io.write([[
    <HTML>
    <HEAD><TITLE>Projects using Lua</TITLE></HEAD>
    <BODY BGCOLOR="#FFFFFF">
    Here are brief descriptions of some projects around the
    world that use <A HREF="home.html">Lua</A>.
    <BR>
    ]])
end

function entry0 (o)
    N=N + 1
    local title = o.title or '(no title)'
    fwrite('<LI><A HREF="#%d">%s</A>\n', N, title)
end 

function entry1 (o)
    N=N + 1
    local title = o.title or o.org or 'org'
    fwrite('<HR>\n<H3>\n')
    local href = ''

    if o.url then
        href = string.format(' HREF="%s"', o.url)
    end 
    fwrite('<A NAME="%d"%s>%s</A>\n', N, href, title)

    if o.title and o.org then
        fwrite('<BR>\n<SMALL><EM>%s</EM></SMALL>', o.org)
    end 
    fwrite('\n</H3>\n')

    if o.description then
        fwrite('%s', string.gsub(o.description,
        '\n\n\n*', '<P>\n'))
        fwrite('<P>\n')
    end 

    if o.email then
        fwrite('Contact: <A HREF="mailto:%s">%s</A>\n',
        o.email, o.contact or o.email)
    elseif o.contact then
        fwrite('Contact: %s\n', o.contact)
    end 
end 

function END()
    fwrite('</BODY></HTML>\n')
end

BEGIN()

N = 0
entry = entry0    -- 为"entry"赋值不同的逻辑，用不同的逻辑操作"db.lua"中的"table"。
fwrite('<UL>\n')
dofile('db.lua')    -- 加载"db.lua"中的内容并执行。相当于调用"entry()"，以"table"作为参数。
fwrite('</UL>\n')

N = 0
entry = entry1    -- 为"entry"赋值不同的逻辑，用不同的逻辑操作"db.lua"中的"table"。
dofile('db.lua')    -- 加载"db.lua"中的内容并执行。相当于调用"entry()"，以"table"作为参数。

END()