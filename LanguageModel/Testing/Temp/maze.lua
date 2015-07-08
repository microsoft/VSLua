
Stack = require "stack"

WEST = 0
SOUTH = 1
EAST = 2
NORTH = 3


generateMaze = {width = Generator:getWidth(), height = Generator:getHeight()}

function generateMaze:setMaze()
    self:setBorders();
    self:setWalls();
end

function generateMaze:setBorders()
    for i = 0, (generateMaze.height - 1) do
        Generator.maze:GetValue(0, i):setBorder(WEST);
        Generator.maze:GetValue(generateMaze.width - 1, i):setBorder(EAST);
    end
    for j = 0, (generateMaze.width - 1) do
        Generator.maze:GetValue(j, 0):setBorder(NORTH);
        Generator.maze:GetValue(j, generateMaze.height - 1):setBorder(SOUTH);
    end
end

function generateMaze:setWalls()
    --Calculate total number of rooms
    roomNum = generateMaze.width * generateMaze.height;
    --Select a random room in the maze, make sure it is not the starting point or destination
    math.randomseed(os.time());
    currentRoom = {x = math.random(generateMaze.width) - 1, y = math.random(generateMaze.height) - 1}
    while ((currentRoom.x == 0 and currentRoom.y == 0) or (currentRoom.x == generateMaze.width - 1 and currentRoom.y == generateMaze.height - 1)) do
        currentRoom = {x = math.random(generateMaze.width) - 1, y = math.random(generateMaze.height) - 1};
    end

    print("initial co-ordinates:", currentRoom.x, currentRoom.y)


    --Counter for number of visited cells
    vistedRooms = 1;
    --Create a stack for rooms
    roomStack = Stack:Create();

    while vistedRooms < roomNum do
        directionIndex = self:findUnvisitedRoom(currentRoom);
        if directionIndex then
            self:knockDownWall(directionIndex, currentRoom);
            stackCopyOfRoom = {x = currentRoom.x, y = currentRoom.y}; -- Error: stack changes x value from number to string
            roomStack:push(stackCopyOfRoom)
            currentRoom = self:moveRooms(directionIndex, currentRoom);
            vistedRooms = vistedRooms + 1;
        else
            currentRoom = roomStack:pop();
        end
    end
    Generator:printMaze();
end

function generateMaze:checkIfUnvisited(x, y)
    unvisited = true;
    tempWalls = Generator.maze:GetValue(x, y):getWalls();
    for i = 0,3 do
        if tempWalls:GetValue(i) == false then
            unvisited = false;
        end
    end
    return unvisited;
end

function generateMaze:findUnvisitedRoom(targetRoom)
    --find room with all walls up (adjacent) randomly pick one if more than one
    unvisitedList = {[0] = false, false, false, false}
    roomBorders = Generator.maze:GetValue(targetRoom.x, targetRoom.y):getBorders();
    --Check WEST if not a border
    if roomBorders:GetValue(WEST) == false then
        unvisitedList[WEST] = self:checkIfUnvisited(targetRoom.x - 1, targetRoom.y)
    end
    --Check South if not a border
    if roomBorders:GetValue(SOUTH) == false then
        unvisitedList[SOUTH] = self:checkIfUnvisited(targetRoom.x, targetRoom.y + 1)
    end
    --Check East if not a border
    if roomBorders:GetValue(EAST) == false then
        unvisitedList[EAST] = self:checkIfUnvisited(targetRoom.x + 1, targetRoom.y)
    end
    --Check North if not a border
    if roomBorders:GetValue(NORTH) == false then
        unvisitedList[NORTH] = self:checkIfUnvisited(targetRoom.x, targetRoom.y - 1)
    end

    --Calculate number of unvisited rooms
    numberOfUnvisited = 0;
    for i = 0,3 do
        if unvisitedList[i] then
            numberOfUnvisited = numberOfUnvisited + 1;
        end
    end

    --for key,value in pairs(unvisitedList) do print(key,value) end
    if numberOfUnvisited == 0 then 
        return nil;
    else 
        if numberOfUnvisited == 1 then
            for index = 0,3 do
                if unvisitedList[index] then
                    return index;
                end
            end
        else
            selectedRoom = math.random(numberOfUnvisited); -- random room out of available
            boolCtr = 0;
            for index = 0,3 do 
                if unvisitedList[index] then
                    boolCtr = boolCtr + 1;
                end
                if boolCtr == selectedRoom then
                    return index;
                end
            end
        end
    end

end

function generateMaze:moveRooms(moveDirection, room)
    if moveDirection == WEST then
        room.x = room.x - 1;
    else 
        if moveDirection == SOUTH then
            room.y = room.y + 1;
        else 
            if moveDirection == EAST then
                room.x = room.x + 1;
            else 
                if moveDirection == NORTH then
                    room.y = room.y - 1;
                end
            end
        end
    end
    return room;
end

function generateMaze:knockDownWall(direction, coordinates)
    originRoom = Generator.maze:GetValue(coordinates.x, coordinates.y)

    if direction == WEST then
        originRoom:removeWall(WEST);
        Generator.maze:GetValue(coordinates.x - 1, coordinates.y):removeWall(EAST)
    else 
        if direction == SOUTH then
            originRoom:removeWall(SOUTH);
            Generator.maze:GetValue(coordinates.x, coordinates.y + 1):removeWall(NORTH)
        else 
            if direction == EAST then
                originRoom:removeWall(EAST);
                Generator.maze:GetValue(coordinates.x + 1, coordinates.y):removeWall(WEST)          
            else 
                if direction == NORTH then
                    originRoom:removeWall(NORTH);
                    Generator.maze:GetValue(coordinates.x, coordinates.y - 1):removeWall(SOUTH)
                end
            end
        end
    end
end

generateMaze:setMaze();

[=[adfasdfaasdf]=]

'3445'

x - 1


