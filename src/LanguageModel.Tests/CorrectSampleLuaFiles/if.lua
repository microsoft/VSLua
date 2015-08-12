

if op == "+" then
    r = a + b
elseif op == "-" then
    r = a - b
    originRoom:removeWall(WEST);
	Generator.maze:GetValue(coordinates.x - 1, coordinates.y):removeWall(EAST)
elseif op == "*" then
    r = a*b
elseif op == "/" then

	Generator.maze:GetValue(coordinates.x - 1, coordinates.y):removeWall(EAST)

    r = a/b
else
      error("invalid operation")
end

--98098098
x = 0    