function fwrite (fmt, ...)
	return io.write(string.format(fmt, unpack(arg)))
end

function print (...)
	print("");
end

function g (a, b, ...) end