local i = 0
-- This function runs at the start of the scene
function Start()
    Log("Hello") -- Log can print messages in game
    Log("World")
end

local passed = false
-- This function will be called every frame
function Update()
    if(time.time > 2 and not passed) then
        Log("2 seconds")
        passed = true
    end
end



player.flySpeed = 100

function Coroutine()
    while true do
        Log(time.time)
        Wait(2)
    end
end

function Wait(seconds) 
    local start = time.time
    local waitedTime = 0
    while waitedTime < seconds do
        coroutine.yield(seconds)
        waitedTime = time.time - start
    end
    return
end