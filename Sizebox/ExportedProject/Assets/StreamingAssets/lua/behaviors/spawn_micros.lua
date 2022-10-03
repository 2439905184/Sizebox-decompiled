behavior = RegisterBehavior("Spawn micros")
behavior.data = {
    menuEntry = "Spawn/Micros",
    secondary = true,
    agent = {
        type = { "micro" }
    },
    target = {
        type = { "none" }
    }
}

-- Usage: Decide where do you want to spawn micros - where to center the spawning area and what radius should it have
-- Once you decide go to the edge of your desired spawning area, place a micro there and select it
-- Then go to the center of the area (it can be centered on a point on the ground or a character, in which case the area will move with character)
-- Click on the center and then on Spawn micros to enable spawner. Click again to disable.
-- Remember - the spawner area is centered on where you click and it's edge is where selected micro is - not the other way around!
function behavior:Start()
    local spawner = globals["microSpawner"]

    if not spawner then
        spawner = {}
        globals["microSpawner"] = spawner
    elseif spawner.active then
        spawner.active = false
        print("Micro spawner disabled")
        return
    end

    spawner.scale = self.agent.scale
    spawner.active = true
    spawner.time = Time.time

    if self.target then
        spawner.radius = self.agent.DistanceTo(self.target)
        spawner.target = self.target
        spawner.cursorPoint = nil
    elseif self.cursorPoint then
        spawner.radius = self.agent.DistanceTo(self.cursorPoint)
        spawner.target = nil
        spawner.cursorPoint = self.cursorPoint
    else
        spawner.radius = 100
        spawner.target = self.agent
        spawner.cursorPoint = nil
    end
    
    print("Micro spawner enabled")
    -- print("scale: " .. spawner.scale)
    -- print("radius: " .. spawner.radius)
    -- print("active: " .. tostring(spawner.active))

end

