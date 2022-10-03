
-- This function runs at the start of the scene
function Start()
    Log("Hello World") -- Log can print messages in game    
end

-- This function will be called once per frame
function Update()
    local currentTime = time.time -- the time in seconds since the beginning of the scene
    local timeSinceLastFrame = time.delta_time -- the time in seconds since the last frame
end

-- Every code that is not inside of a function will be called at the start of the scene
-- You can set initial settings here, and also change them in the update loop

-- player settings
-- movement
player.walkSpeed = 0.2
player.runSpeed = 1
player.sprintSpeed = 3
player.flySpeed = 8
player.superFlySpeed = 1000.0
player.climbSpeed = 70
player.jumpPower = 8
player.autowalk = false

-- size
player.scale = 10
player.sizeChangeSpeed = 0.7 -- size change speed using the Z and X keys
player.minSize = 0.001 -- min and max size clamps the scale value
player.maxSize = 1000

-- world functions
world.gravity = 9.8 -- set the gravity of the map

-- giantess functions
gts.globalSpeed = 1 -- global speed, as in the main menu
gts.minSize = 0.001 -- note, gts size is 1000 bigger than micros, 
gts.maxSize = 1000 -- so scale 1 in a gts is equivalent to scale 1000 in a micro
-- to access a giantess you can use the list
firstGts = gts.list[1] -- access the giantess with the id number 1: the first giantess loaded
if firstGts != nil then -- check for nil to avoid errors.. nil means that is empty, no model found
    firstGts.scale = 10; -- the scale can be access like the player
    Log(firstGts.name) -- will print the name of the model
    firstGts.Delete() -- deletes the giantess
end



-- player.Crush() -- this kills the player

