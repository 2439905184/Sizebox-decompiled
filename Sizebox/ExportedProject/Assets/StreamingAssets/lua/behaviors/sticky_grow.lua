Gizmo = RegisterBehavior("sticky_grow")
Gizmo.data =  {
    menuEntry = "Size/Sticky Grow",
    flags = { "grow" },
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "oneself" }
    }
}

growthModes = require "growth_modes"

period = 2                  -- how many seconds between checks
playerModifier = 2          -- how much player counts as
aliveModifier = 2           -- how much living micros count as
microBase = 0.08            -- amount of growth per micro
mode = growthModes.QUADRIC  -- change this to select growth mode (from growth_modes.lua)

function Gizmo:Start()
    self.nextCheckTime = Time.time
end

function Gizmo:Update()
    if self.nextCheckTime > Time.time then
        return
    end
    
    self.nextCheckTime = Time.time + period
    local growAmount = 0
    for _,micro in pairs(micros.list) do
        if micro.isStuck() and micro.transform.root == self.agent.transform then
            local factor = growthModes.getFactor(mode, micro.scale, self.agent.scale)

            if micro.isPlayer() then
                factor = factor * playerModifier
            elseif not micro.isDead() then
                factor = factor * aliveModifier
            end

            growAmount = growAmount + microBase * factor
        end
    end

    self.agent.Grow(growAmount)
end
