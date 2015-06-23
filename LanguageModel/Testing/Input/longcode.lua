require "TypeTable"

function table_to_ordered_table(t)
    local n = {}

    for _, v in pairs(t) do table.insert(n, v) end

    return n
end

function count_elements(t)
    local count = 0

    for _,_ in pairs(t) do count = count + 1 end

    return count
end


function Array(t)


    local self = TypeTable("ARRAY")

    self.arr = t and table_to_ordered_table(t) or {}
    self.len = t and count_elements(t) or 0



    local get = function(i) return self.arr[i] end

    local set = function(i, v) self.arr[i] = v end

    local add = function(v)
        table.insert(self.arr, v)
        self.len = self.len + 1
    end

    local pop = function()
        if self.len > 0 then
            self.len = self.len - 1
            return table.remove(self.arr)
        end
        return nil
    end

    local length = function()
        return self.len
    end

    local iter = function()
        local i = 0
        local n = self.len
        return function()
            i = i + 1
            if i<=n then return self.arr[i] end
        end
    end

    local shuffle = function(times)
        if self.len == 0 then return end
        times = times or self.len*self.len
        for _=1, times do
            local ri = math.random(1, self.len)
            local temp = get(1)
            set(1, get(ri))
            set(ri, temp)
        end
    end

    local join = function(str)
        local s = tostring(get(1))

        for i=2, self.len do
            s = s..str..tostring(get(i))
        end

        return s

    end


    -- metatable stuff

    local mult = function(b)
        local t = {}
        for _=1, b do
            for p in iter() do
                table.insert(t, p)
            end
        end
        return Array(t)
    end

    local myadd = function(a, b)
        local n = Array()

        for p in a.iter() do n.add(p) end
        for p in b.iter() do n.add(p) end

        return n

    end

    local tostring = function()
        local s = "("
        for i=1, self.len do
            s = s..tostring(self.arr[i])..", "
        end
        return string.sub(s, 1, string.len(s)-2)..")"
    end

    local copy = function()
        return Array(self.arr)
    end

    local new = {
        type = self.type(),
        get = get,
        set = set,
        add = add,
        pop = pop,
        length = length,
        shuffle = shuffle,
        iter = iter,
        join = join,
        copy = copy,
    }

    new.mt = {}
    setmetatable(new, new.mt)
    new.mt.__mul = mult
    new.mt.__tostring = tostring
    new.mt.__add = myadd

    return new

end