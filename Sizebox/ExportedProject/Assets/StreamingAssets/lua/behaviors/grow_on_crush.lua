Gizmo = RegisterBehavior("grow_on_crush")
Gizmo.data =  {
    menuEntry = "Size/Grow On Crush",
    secondary = true,
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "oneself" }
    }
}

growthModes = require "growth_modes"

MODE = growthModes.QUADRIC -- change this to select growth mode (from growth_modes.lua)
GROW_DURATION = 1 -- in seconds

-- this function is not called unless you hook it to a event
-- OnCrush event will pass folowing data to this function:
-- crusher - crusher entity
-- victim - victim entity
function Gizmo:Listener(data)
    -- We need make sure crusher.id is the same as self.agent.id otherwise all event listeners will grow
    if data.crusher and data.crusher.id == self.agent.id and not data.victim.IsCrushed() then
        local factor = growthModes.getFactor(MODE, data.victim.scale, data.crusher.scale)
        self.agent.grow(factor, GROW_DURATION)
    end
end

function Gizmo:Start()
    -- subscribe Listener() to a "OnCrush" event
    self.agent.dict.OnCrush = Event.Register(self, EventCode.OnCrush, self.Listener)
end
